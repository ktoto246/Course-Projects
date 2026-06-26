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
    /// <summary>
    /// Логика взаимодействия для EditStatusWindow.xaml
    /// </summary>
    public partial class EditStatusWindow : Window
    {
        private Edinica _currentEdinica;
        private int _currentStatusId;
        public EditStatusWindow()
        {
            InitializeComponent();
            Loaded += EditStatusWindow_Loaded;
        }
        //Конструктор с параметрами
        internal EditStatusWindow(Edinica edinica) : this()
        {
            _currentEdinica = edinica;
            _currentStatusId = edinica.FK_Status;
        }
        //Загрузка окна
        private void EditStatusWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_currentEdinica == null)
            {
                MessageBox.Show("Ошибка: нет данных для отображения", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            LoadEdinicaData();
            LoadAvailableStatuses();
            UpdateRules();
        }
        //Заполнение полей формы
        private void LoadEdinicaData()
        {
            txtDonorID.Text = _currentEdinica.DonorID.ToString();
            txtComponent.Text = _currentEdinica.Component;
            txtBloodGroup.Text = _currentEdinica.BloodGroup;
            txtRh.Text = _currentEdinica.RhString;
            txtDateSbora.Text = _currentEdinica.Date_Sbora.ToString("dd.MM.yyyy");
            txtDateFreeze.Text = _currentEdinica.DateFreezeString;
            txtCurrentStatus.Text = _currentEdinica.StatusObj.StatusName;
        }
        //Закгрузка доступных статусов
        private void LoadAvailableStatuses()
        {
            switch (_currentStatusId)
            {
                case 1:
                    cbNewStatus.Items.Add("Бракованная");
                    break;

                case 2:
                    cbNewStatus.Items.Add("Пригодна для использования");
                    cbNewStatus.Items.Add("Бракованная");
                    break;

                case 3:
                    cbNewStatus.Items.Add("Выдана");
                    cbNewStatus.Items.Add("Бракованная");
                    break;

                case 4:
                case 5:
                    cbNewStatus.IsEnabled = false;
                    btnSave.IsEnabled = false;
                    btnDelete.IsEnabled = true;
                    break;
            }

            if (cbNewStatus.Items.Count > 0)
                cbNewStatus.SelectedIndex = 0;
        }
        //Правило изменение статуса
        private void UpdateRules()
        {
            string rules = "";
            string warning = "";

            switch (_currentStatusId)
            {
                case 1:
                    rules = "Компонент на карантине. Можно изменить статус только на 'Бракованная'.";
                    break;

                case 2:
                    rules = "Компонент законсервирован. Можно изменить статус на 'Пригодна для использования' или 'Бракованная'.";
                    DateTime expiryDate = _currentEdinica.Date_Sbora.AddDays(
                        _currentEdinica.Component == "Эритроциты" ? 180 : 240);

                    double daysLeft = (expiryDate - DateTime.Now).TotalDays;
                    if (daysLeft <= 7)
                    {
                        warning = $"Внимание! Срок хранения истекает через {daysLeft:F0} дней";
                    }
                    break;

                case 3:
                    rules = "Компонент пригоден для использования. Можно изменить статус на 'Выдана' или 'Бракованная'.";
                    break;

                case 4:
                case 5:
                    rules = "Компонент выдан или забракован. Можно только удалить запись.";
                    warning = "Запись будет безвозвратно удалена из базы данных!";
                    break;
            }

            tbRules.Text = rules;
            tbWarning.Text = warning;
        }
        //Название статусов
        private int GetNewStatusId(string statusName)
        {
            switch (statusName)
            {
                case "Бракованная": return 5;
                case "Пригодна для использования": return 3;
                case "Выдана": return 4;
                default: return 0;
            }
        }
        //Можно ли менять статус
        private bool IsStatusChangeAllowed(int currentStatusId, int newStatusId)
        {
            return (currentStatusId == 1 && newStatusId == 5) ||
                   (currentStatusId == 2 && (newStatusId == 3 || newStatusId == 5)) ||
                   (currentStatusId == 3 && (newStatusId == 4 || newStatusId == 5));
        }
        //Кнопка отмена
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Кнопка сохранить
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbNewStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите новый статус",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string newStatusName = cbNewStatus.SelectedItem.ToString();
            int newStatusId = GetNewStatusId(newStatusName);
            if (newStatusId == 0)
            {
                MessageBox.Show("Неизвестный статус",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!IsStatusChangeAllowed(_currentStatusId, newStatusId))
            {
                MessageBox.Show("Такое изменение статуса не разрешено правилами",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show($"Изменить статус на '{newStatusName}'?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                bool success = DatabaseHelper.UpdateStatus(_currentEdinica.ID_Edinica, newStatusId);
                if (success)
                {
                    MessageBox.Show("Статус успешно изменен",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при изменении статуса",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        //Кнопка удалить
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_currentStatusId != 4 && _currentStatusId != 5)
            {
                MessageBox.Show("Удаление разрешено только для компонентов со статусом 'Выдана' или 'Бракованная'",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись?\n" +
                                       "Данное действие невозможно отменить!",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                bool success = DatabaseHelper.DeleteEdinica(_currentEdinica.ID_Edinica);
                if (success)
                {
                    MessageBox.Show("Запись успешно удалена",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении записи",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
