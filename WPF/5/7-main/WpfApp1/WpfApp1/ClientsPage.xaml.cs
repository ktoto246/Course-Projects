using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class ClientsPage : Page
    {
        private AppDbContext _context;

        public ClientsPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            var query = _context.Clients.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(c => c.CompanyName.Contains(searchText) ||
                                         c.ContactPerson.Contains(searchText) ||
                                         c.Phone.Contains(searchText));
            }

            DataGridClients.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text.Trim());
        }

        private void DataGridClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridClients.SelectedItem is Client selectedClient)
            {
                TxtCompanyName.Text = selectedClient.CompanyName;
                TxtContactPerson.Text = selectedClient.ContactPerson;
                TxtPhone.Text = selectedClient.Phone;
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(TxtCompanyName.Text))
            {
                MessageBox.Show("Укажите название компании.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtContactPerson.Text))
            {
                MessageBox.Show("Укажите контактное лицо.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtPhone.Text))
            {
                MessageBox.Show("Укажите номер телефона.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            var newClient = new Client
            {
                CompanyName = TxtCompanyName.Text.Trim(),
                ContactPerson = TxtContactPerson.Text.Trim(),
                Phone = TxtPhone.Text.Trim()
            };

            _context.Clients.Add(newClient);
            _context.SaveChanges();

            ClearFields();
            LoadData(TxtSearch.Text.Trim()); 
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridClients.SelectedItem is Client selectedClient)
            {
                if (!ValidateInputs()) return;

                selectedClient.CompanyName = TxtCompanyName.Text.Trim();
                selectedClient.ContactPerson = TxtContactPerson.Text.Trim();
                selectedClient.Phone = TxtPhone.Text.Trim();

                _context.SaveChanges();

                ClearFields();
                LoadData(TxtSearch.Text.Trim());
            }
            else
            {
                MessageBox.Show("Выберите клиента для изменения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridClients.SelectedItem is Client selectedClient)
            {
                if (MessageBox.Show($"Удалить клиента '{selectedClient.CompanyName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Clients.Remove(selectedClient);
                        _context.SaveChanges();
                        ClearFields();
                        LoadData(TxtSearch.Text.Trim());
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                    {
                        MessageBox.Show("Невозможно удалить клиента, так как с ним существуют оформленные сделки. Сначала удалите связанные сделки.",
                                        "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);

                        _context.Entry(selectedClient).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            TxtCompanyName.Clear();
            TxtContactPerson.Clear();
            TxtPhone.Clear();
            DataGridClients.SelectedItem = null;
        }
    }
}