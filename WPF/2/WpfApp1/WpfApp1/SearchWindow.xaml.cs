using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1
{
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
        }
        //Настройка начальных значений
        private void SearchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Инициализация с проверкой на null
                if (cbSearchComponent != null && cbSearchComponent.Items.Count > 0)
                    cbSearchComponent.SelectedIndex = 0;

                if (cbRecipientGroup != null && cbRecipientGroup.Items.Count > 0)
                    cbRecipientGroup.SelectedIndex = 0;

                if (cbRecipientRh != null && cbRecipientRh.Items.Count > 0)
                    cbRecipientRh.SelectedIndex = 0;

                UpdateCompatibilityInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateCompatibilityInfo()
        {
            try
            {
                if (tbRhLabel == null || tbCompatibilityInfo == null || cbSearchComponent == null)
                    return;

                if (cbSearchComponent.SelectedItem is ComboBoxItem componentItem)
                {
                    string component = componentItem.Content?.ToString() ?? "";

                    if (component == "Эритроциты")
                    {
                        tbRhLabel.Visibility = Visibility.Visible;
                        if (cbRecipientRh != null)
                            cbRecipientRh.Visibility = Visibility.Visible;
                        tbCompatibilityInfo.Text = "Для эритроцитов учитывается группа крови и резус-фактор";
                    }
                    else
                    {
                        tbRhLabel.Visibility = Visibility.Collapsed;
                        if (cbRecipientRh != null)
                            cbRecipientRh.Visibility = Visibility.Collapsed;
                        tbCompatibilityInfo.Text = "Для плазмы учитывается только группа крови (резус-фактор не важен)";
                    }
                }
            }
            catch { }
        }

        private List<Edinica> SearchCompatibleUnits()
        {
            List<Edinica> results = new List<Edinica>();

            try
            {
                // 1. Проверка выбора компонента
                if (!(cbSearchComponent?.SelectedItem is ComboBoxItem componentItem))
                {
                    MessageBox.Show("Выберите тип компонента для поиска",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return results;
                }

                string component = componentItem.Content?.ToString() ?? "";
                if (string.IsNullOrEmpty(component))
                {
                    MessageBox.Show("Выберите тип компонента для поиска",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return results;
                }

                // 2. Проверка группы крови
                if (!(cbRecipientGroup?.SelectedItem is ComboBoxItem recipientGroupItem))
                {
                    MessageBox.Show("Выберите группу крови реципиента",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return results;
                }

                string recipientGroup = recipientGroupItem.Content?.ToString() ?? "";
                if (string.IsNullOrEmpty(recipientGroup))
                {
                    MessageBox.Show("Выберите группу крови реципиента",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return results;
                }

                // 3. Для эритроцитов проверяем резус-фактор
                if (component == "Эритроциты")
                {
                    if (!(cbRecipientRh?.SelectedItem is ComboBoxItem recipientRhItem))
                    {
                        MessageBox.Show("Для поиска эритроцитов выберите резус-фактор реципиента",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return results;
                    }
                }

                // 4. Подключаемся к базе данных
                using (SqlConnection conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    conn.Open();

                    if (component == "Эритроциты")
                    {
                        // Поиск совместимых эритроцитов
                        results = SearchCompatibleErythrocytes(conn, recipientGroup);
                    }
                    else if (component == "Плазма")
                    {
                        // Поиск совместимой плазмы
                        results = SearchCompatiblePlasma(conn, recipientGroup);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return results;
        }

        private List<Edinica> SearchCompatibleErythrocytes(SqlConnection conn, string recipientGroup)
        {
            List<Edinica> results = new List<Edinica>();

            try
            {
                // Получаем резус-фактор реципиента
                if (!(cbRecipientRh?.SelectedItem is ComboBoxItem recipientRhItem))
                    return results;

                string recipientRh = recipientRhItem.Content?.ToString() ?? "";
                bool recipientRhBool = recipientRh == "Rh+";

                // Упрощенный запрос - получаем все законсервированные эритроциты
                string query = @"
                    SELECT e.*, s.Status 
                    FROM Edinica e
                    INNER JOIN Status s ON e.FK_Status = s.ID_Status
                    WHERE e.Component = 'Эритроциты'
                        AND e.FK_Status = 2 -- Законсервирована
                        AND e.BloodGroup IS NOT NULL
                        AND e.Rh IS NOT NULL
                    ORDER BY e.Date_Sbora DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Таблица совместимости эритроцитов
                    var compatibleGroups = GetCompatibleErythrocyteGroups(recipientGroup, recipientRhBool);

                    while (reader.Read())
                    {
                        Edinica ed = CreateEdinicaFromReader(reader);
                        if (ed == null) continue;

                        // Проверяем совместимость
                        string donorKey = $"{ed.BloodGroup}{(ed.Rh == true ? "+" : "-")}";

                        if (compatibleGroups.Contains(donorKey))
                        {
                            results.Add(ed);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска эритроцитов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return results;
        }

        private List<Edinica> SearchCompatiblePlasma(SqlConnection conn, string recipientGroup)
        {
            List<Edinica> results = new List<Edinica>();

            try
            {
                // Упрощенный запрос - получаем все законсервированную плазму
                string query = @"
                    SELECT e.*, s.Status 
                    FROM Edinica e
                    INNER JOIN Status s ON e.FK_Status = s.ID_Status
                    WHERE e.Component = 'Плазма'
                        AND e.FK_Status = 2 -- Законсервирована
                        AND e.BloodGroup IS NOT NULL
                    ORDER BY e.Date_Sbora DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Таблица совместимости плазмы
                    var compatibleGroups = GetCompatiblePlasmaGroups(recipientGroup);

                    while (reader.Read())
                    {
                        Edinica ed = CreateEdinicaFromReader(reader);
                        if (ed == null) continue;

                        // Проверяем совместимость
                        if (compatibleGroups.Contains(ed.BloodGroup))
                        {
                            results.Add(ed);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска плазмы: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return results;
        }

        private HashSet<string> GetCompatibleErythrocyteGroups(string recipientGroup, bool recipientRh)
        {
            // Упрощенная таблица совместимости (можно расширить)
            var compatibility = new Dictionary<string, List<string>>
            {
                // Формат: "ГруппаРезус" -> ["Донор1", "Донор2", ...]
                { "O(I)-", new List<string> { "O(I)-" } },
                { "O(I)+", new List<string> { "O(I)-", "O(I)+" } },
                { "A(II)-", new List<string> { "O(I)-", "A(II)-" } },
                { "A(II)+", new List<string> { "O(I)-", "O(I)+", "A(II)-", "A(II)+" } },
                { "B(III)-", new List<string> { "O(I)-", "B(III)-" } },
                { "B(III)+", new List<string> { "O(I)-", "O(I)+", "B(III)-", "B(III)+" } },
                { "AB(IV)-", new List<string> { "O(I)-", "A(II)-", "B(III)-", "AB(IV)-" } },
                { "AB(IV)+", new List<string> { "O(I)-", "O(I)+", "A(II)-", "A(II)+", "B(III)-", "B(III)+", "AB(IV)-", "AB(IV)+" } }
            };

            string recipientKey = $"{recipientGroup}{(recipientRh ? "+" : "-")}";

            if (compatibility.ContainsKey(recipientKey))
            {
                return new HashSet<string>(compatibility[recipientKey]);
            }

            return new HashSet<string>();
        }

        private HashSet<string> GetCompatiblePlasmaGroups(string recipientGroup)
        {
            // Таблица совместимости плазмы
            var compatibility = new Dictionary<string, List<string>>
            {
                { "O(I)", new List<string> { "O(I)", "A(II)", "B(III)", "AB(IV)" } },
                { "A(II)", new List<string> { "A(II)", "AB(IV)" } },
                { "B(III)", new List<string> { "B(III)", "AB(IV)" } },
                { "AB(IV)", new List<string> { "AB(IV)" } }
            };

            if (compatibility.ContainsKey(recipientGroup))
            {
                return new HashSet<string>(compatibility[recipientGroup]);
            }

            return new HashSet<string>();
        }

        private Edinica CreateEdinicaFromReader(SqlDataReader reader)
        {
            try
            {
                if (reader == null) return null;

                Edinica ed = new Edinica();

                // Безопасное чтение каждого поля
                if (reader["ID_Edinica"] != DBNull.Value)
                    ed.ID_Edinica = Convert.ToInt32(reader["ID_Edinica"]);

                if (reader["DonorID"] != DBNull.Value)
                    ed.DonorID = Convert.ToInt32(reader["DonorID"]);

                if (reader["Component"] != DBNull.Value)
                    ed.Component = reader["Component"].ToString();

                if (reader["FK_Status"] != DBNull.Value)
                    ed.FK_Status = Convert.ToInt32(reader["FK_Status"]);

                if (reader["BloodGroup"] != DBNull.Value)
                    ed.BloodGroup = reader["BloodGroup"].ToString();

                if (reader["Date_Sbora"] != DBNull.Value)
                    ed.Date_Sbora = Convert.ToDateTime(reader["Date_Sbora"]);

                if (reader["Date_Freeze"] != DBNull.Value)
                    ed.Date_Freeze = Convert.ToDateTime(reader["Date_Freeze"]);

                if (reader["Rh"] != DBNull.Value)
                    ed.Rh = Convert.ToBoolean(reader["Rh"]);

                // Создаем объект Status
                ed.StatusObj = new Status();
                if (reader["FK_Status"] != DBNull.Value)
                    ed.StatusObj.ID_Status = Convert.ToInt32(reader["FK_Status"]);

                if (reader["Status"] != DBNull.Value)
                    ed.StatusObj.StatusName = reader["Status"].ToString();

                return ed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка создания Edinica: {ex.Message}");
                return null;
            }
        }

        // ========== ОБРАБОТЧИКИ СОБЫТИЙ ==========

        private void CbSearchComponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateCompatibilityInfo();
        }
        //Кнопка найти
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем что все элементы инициализированы
                if (dgResults == null || tbResultsInfo == null || tbResultsCount == null || tbStatus == null)
                {
                    MessageBox.Show("Ошибка инициализации окна",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Выполняем поиск
                var results = SearchCompatibleUnits();

                // Показываем результаты
                dgResults.ItemsSource = results;
                tbResultsCount.Text = $"Найдено: {results.Count}";

                // Информационное сообщение
                if (results.Count == 0)
                {
                    tbResultsInfo.Text = "Совместимые компоненты не найдены";
                    tbResultsInfo.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    tbResultsInfo.Text = $"Найдено {results.Count} совместимых компонентов";
                    tbResultsInfo.Foreground = System.Windows.Media.Brushes.Green;

                    // Обновляем статус
                    if (cbSearchComponent?.SelectedItem is ComboBoxItem componentItem &&
                        cbRecipientGroup?.SelectedItem is ComboBoxItem recipientGroupItem)
                    {
                        string component = componentItem.Content?.ToString() ?? "";
                        string recipientGroup = recipientGroupItem.Content?.ToString() ?? "";

                        tbStatus.Text = $"Поиск завершен: {component} для {recipientGroup}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критическая ошибка: {ex.Message}\n\n{ex.StackTrace}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
