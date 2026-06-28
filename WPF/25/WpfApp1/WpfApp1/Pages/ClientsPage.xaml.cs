using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientsPage.xaml
    /// </summary>
    public partial class ClientsPage : Page
    {
        private Client? _selectedClient;
        public ClientsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var context = new BmtcContext())
            {
                ClientsGrid.ItemsSource = context.Clients.ToList();
            }
        }

        private void ClientsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Client client)
            {
                _selectedClient = client;
                TxtFullName.Text = client.FullName;
                TxtPhone.Text = client.Phone;
                TxtDriverLicense.Text = client.DriverLicense;
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                MessageBox.Show("ФИО не может быть пустым!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtPhone.Text))
            {
                MessageBox.Show("Телефон не может быть пустым!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            string phoneRegex = @"^(?:\+7|8)\d{10}$";
            if (!Regex.IsMatch(TxtPhone.Text.Trim(), phoneRegex))
            {
                MessageBox.Show("Некорректный формат телефона!\nИспользуйте формат: +7XXXXXXXXXX или 8XXXXXXXXXX", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtDriverLicense.Text))
            {
                MessageBox.Show("Укажите документ или водительское удостоверение!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            using (var db = new BmtcContext())
            {
                var newClient = new Client
                {
                    FullName = TxtFullName.Text.Trim(),
                    Phone = TxtPhone.Text.Trim(),
                    DriverLicense = TxtDriverLicense.Text.Trim()
                };

                db.Clients.Add(newClient);
                db.SaveChanges();
            }

            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if(_selectedClient == null)
            {
                MessageBox.Show("Выберите клиента из списка для изменения!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateInput()) return;

            using (var db = new BmtcContext())
            {
                var clientToUpdate = db.Clients.FirstOrDefault(x => x.Id == _selectedClient.Id);
                if (clientToUpdate != null)
                {
                    clientToUpdate.FullName = TxtFullName.Text.Trim();
                    clientToUpdate.Phone = TxtPhone.Text.Trim();
                    clientToUpdate.DriverLicense = TxtDriverLicense.Text.Trim();
                    db.SaveChanges();
                }
            }

            LoadData();
            ClearForm();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedClient == null)
            {
                MessageBox.Show("Выберите клиента для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить клиента {_selectedClient.FullName}?\nВсе связанные с ним автомобили и осмотры будут удалены!", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var db = new BmtcContext())
                {
                    var clientToDelete = db.Clients.FirstOrDefault(x => x.Id == _selectedClient.Id);
                    if (clientToDelete != null)
                    {
                        db.Clients.Remove(clientToDelete);
                        db.SaveChanges();
                    }
                }

                LoadData();
                ClearForm();
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedClient = null;
            ClientsGrid.SelectedItem = null;
            TxtFullName.Clear();
            TxtPhone.Clear();
            TxtDriverLicense.Clear();
        }
    }
}
