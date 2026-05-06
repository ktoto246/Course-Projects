using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private List<Edinica> allEdinici = new List<Edinica>();
        private List<Edinica> filteredEdinici = new List<Edinica>();
        private AddDonationWindow currentAddWindow = null;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            UpdateStatusBar("Данные загружены.");
        }
        //Обработчик закрытия окна
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Закрыть приложение?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }
        //Загрузка данных
        private void LoadData()
        {
            try
            {
                allEdinici = DatabaseHelper.GetEdinici();
                ApplyFilter();
                UpdateStatusBar($"Загружено {allEdinici.Count} записей");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Фильтр по данным
        private void ApplyFilter()
        {
            if (allEdinici == null) return;
            filteredEdinici = allEdinici.ToList();
            if (cbFilterComponent?.SelectedIndex > 0)
            {
                string selectedComponent = (cbFilterComponent.SelectedItem as ComboBoxItem)?.Content?.ToString();
                if (!string.IsNullOrEmpty(selectedComponent))
                {
                    filteredEdinici = filteredEdinici
                        .Where(e => e.Component == selectedComponent)
                        .ToList();
                }
            }
            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string searchText = txtSearch.Text.ToLower();
                filteredEdinici = filteredEdinici
                    .Where(e => e.DonorID.ToString().Contains(searchText) ||
                               e.BloodGroup.ToLower().Contains(searchText) ||
                               e.StatusObj?.StatusName.ToLower().Contains(searchText) == true)
                    .ToList();
            }

            dgUnits.ItemsSource = filteredEdinici;
            tbRecordsCount.Text = $"Записей: {filteredEdinici.Count} (Всего: {allEdinici.Count})";
        }
        //Обновление статусной строки
        private void UpdateStatusBar(string message)
        {
            tbStatus.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
        }
        //Обновить
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
        //Изменение выбора в Фильтре
        private void CbFilterComponent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilter();
        }
        //Изменение текста в поле поика
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }
        //Кнопка очистить
        private void BtnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            ApplyFilter();
        }
        //Кнопка добавить донацию
        private void BtnAddDonation_Click(object sender, RoutedEventArgs e)
        {
            if (currentAddWindow != null && currentAddWindow.IsLoaded)
            {
                MessageBox.Show("Окно добавления уже открыто. Закройте его перед открытием нового.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                currentAddWindow.Focus();
                return;
            }
            currentAddWindow = new AddDonationWindow();
            currentAddWindow.Owner = this;
            currentAddWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            currentAddWindow.Closed += (s, args) =>
            {
                currentAddWindow = null;
                LoadData();
            };
            currentAddWindow.Show();
        }
        //Кнопка поиск по совместимости
        private void BtnSearchCompatibility_Click(object sender, RoutedEventArgs e)
        {
            SearchWindow searchWindow = new SearchWindow();
            searchWindow.Owner = this;
            searchWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            searchWindow.Show();
        }
        //Кнопка обновить статус
        private void BtnUpdateStatuses_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DatabaseHelper.ExecuteUpdateStatusesAutomatically();
                LoadData();
                UpdateStatusBar("Статусы обновлены вручную");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления статусов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Кнопка экспорт
        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (allEdinici == null || allEdinici.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Диалог выбора типа экспорта
                var result = MessageBox.Show("Экспортировать все данные или только отфильтрованные?",
                    "Экспорт", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                if (result == MessageBoxResult.Cancel) return;

                List<Edinica> dataToExport = (result == MessageBoxResult.Yes) ? allEdinici : filteredEdinici;

                if (dataToExport.Count == 0)
                {
                    MessageBox.Show("Нет данных для экспорта",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
                saveFileDialog.FileName = $"Донор21_Экспорт_{DateTime.Now:yyyyMMdd_HHmmss}";
                saveFileDialog.DefaultExt = ".csv";

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    ExportToCSV(dataToExport, filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Экспорт данных
        private void ExportToCSV(List<Edinica> data, string filePath)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath, false, System.Text.Encoding.UTF8))
                {
                    // Заголовки CSV
                    writer.WriteLine("ID записи;ID донора;Компонент;Статус;Дата сбора;Дата консервации;Группа крови;Резус-фактор;Срок годности");

                    // Данные
                    foreach (var item in data)
                    {
                        // Рассчитываем срок годности
                        string expiryInfo = "";
                        if (item.StatusObj != null && item.StatusObj.StatusName == "Пригодна для использования")
                        {
                            DateTime expiryDate = item.Date_Sbora.AddDays(
                                item.Component == "Эритроциты" ? 180 : 240);
                            int daysLeft = (int)(expiryDate - DateTime.Now).TotalDays;
                            expiryInfo = daysLeft > 0 ? $"Осталось {daysLeft} дней" : "Просрочено";
                        }

                        writer.WriteLine(
                            $"{item.ID_Edinica};" +
                            $"{item.DonorID};" +
                            $"{EscapeCSV(item.Component)};" +
                            $"{EscapeCSV(item.Status)};" +
                            $"{item.Date_Sbora:dd.MM.yyyy};" +
                            $"{EscapeCSV(item.DateFreezeString)};" +
                            $"{EscapeCSV(item.BloodGroup)};" +
                            $"{EscapeCSV(item.RhString)};" +
                            $"{EscapeCSV(expiryInfo)}"
                        );
                    }
                }

                // Показываем информацию о файле
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{filePath}\"");

                MessageBox.Show($"Экспорт завершен!\n\n" +
                               $"Файл: {System.IO.Path.GetFileName(filePath)}\n" +
                               $"Записей: {data.Count}\n" +
                               $"Путь: {filePath}\n\n" +
                               $"Файл CSV можно открыть в:\n" +
                               $"• Microsoft Excel\n" +
                               $"• Google Таблицы\n" +
                               $"• LibreOffice Calc",
                    "Экспорт завершен", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта в CSV: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        //Экранирование сторок для CSV формата
        private string EscapeCSV(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            if (value.Contains(";") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }

            return value;
        }
        //Загрузки строк в таблицы
        private void DgUnits_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            Edinica item = e.Row.DataContext as Edinica;
            if (item != null && item.StatusObj != null)
            {
                switch (item.StatusObj.StatusName)
                {
                    case "Выдана":
                    case "Бракованная":
                        e.Row.Background = new SolidColorBrush(Color.FromRgb(0x8C, 0x8C, 0x8C));
                        e.Row.Foreground = Brushes.White;
                        break;

                    case "Пригодна для использования":
                        DateTime expiryDate = item.Date_Sbora.AddDays(
                            item.Component == "Эритроциты" ? 180 : 240);

                        if ((expiryDate - DateTime.Now).TotalDays <= 7)
                        {
                            e.Row.Background = Brushes.LightYellow;
                            e.Row.Foreground = Brushes.DarkRed;
                        }
                        break;
                }
            }
        }
        //Открывает окно редактирования статуса
        private void DgUnits_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = (DependencyObject)e.OriginalSource;

            while (dependencyObject != null && !(dependencyObject is DataGridRow))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            if (dependencyObject is DataGridRow row)
            {
                if (row.DataContext is Edinica selected)
                {
                    EditStatusWindow editWindow = new EditStatusWindow(selected);
                    editWindow.Owner = this;
                    editWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    editWindow.Closed += (s, args) => LoadData();
                    editWindow.Show();
                }
            }
        }
    }
}
