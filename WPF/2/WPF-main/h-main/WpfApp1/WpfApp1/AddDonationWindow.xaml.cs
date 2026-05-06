using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddDonationWindow.xaml
    /// </summary>
    public partial class AddDonationWindow : Window
    {
        public AddDonationWindow()
        {
            InitializeComponent();
            Loaded += AddDonationWindow_Loaded;
        }
        private void AddDonationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            dpDateSbora.SelectedDate = DateTime.Today;
            cbBloodGroup.SelectedIndex = 0;
        }

        // Метод валидации
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtDonorID.Text))
            {
                MessageBox.Show("Введите ID донора",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(txtDonorID.Text, out int donorID) || donorID <= 0)
            {
                MessageBox.Show("ID донора должен быть положительным числом",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (dpDateFreeze.SelectedDate.HasValue)
            {
                DateTime dateSbora = dpDateSbora.SelectedDate ?? DateTime.Today;
                DateTime dateFreeze = dpDateFreeze.SelectedDate.Value;

                if (dateFreeze < dateSbora)
                {
                    MessageBox.Show("Дата консервации не может быть раньше даты сбора",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                ComboBoxItem componentItem = cbComponent.SelectedItem as ComboBoxItem;
                if (componentItem != null)
                {
                    string component = componentItem.Content.ToString();
                    TimeSpan difference = dateFreeze - dateSbora;
                    if (component == "Эритроциты" && difference.TotalDays > 1)
                    {
                        MessageBox.Show("Для эритроцитов консервация должна быть в течение 24 часов (1 дня)",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (component == "Плазма" && difference.TotalDays > 7)
                    {
                        MessageBox.Show("Для плазмы консервация должна быть в течение 7 дней",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
            }

            return true;
        }
        //Кнопка выход
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //Кнопка сохранить
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
            {
                return;
            }

            try
            {
                int donorID = int.Parse(txtDonorID.Text);

                ComboBoxItem componentItem = cbComponent.SelectedItem as ComboBoxItem;
                ComboBoxItem bloodGroupItem = cbBloodGroup.SelectedItem as ComboBoxItem;

                if (componentItem == null || bloodGroupItem == null)
                {
                    MessageBox.Show("Выберите компонент и группу крови",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                string component = componentItem.Content.ToString();
                string bloodGroup = bloodGroupItem.Content.ToString();
                bool? rh = cbRh.IsChecked;
                DateTime dateSbora = dpDateSbora.SelectedDate ?? DateTime.Today;
                DateTime? dateFreeze = dpDateFreeze.SelectedDate;
                int statusId = 1;
                Edinica newEdinica = new Edinica
                {
                    DonorID = donorID,
                    Component = component,
                    FK_Status = statusId,
                    Date_Sbora = dateSbora,
                    Date_Freeze = dateFreeze,
                    BloodGroup = bloodGroup,
                    Rh = rh
                };
                bool success = DatabaseHelper.AddEdinica(newEdinica);
                if (success)
                {
                    MessageBox.Show($"Донация успешно добавлена!\nID донора: {donorID}",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
