using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class ClientPage : Page
    {
        private AppDbContext _context;
        private List<Client> _clients = new List<Client>();
        private int currentId = 0;

        public ClientPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _clients = _context.Clients.ToList();
            dataGrid.ItemsSource = _clients;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox searchBox = sender as TextBox;
            if (searchBox == null) return;

            string filter = searchBox.Text.ToLower();
            var filtered = _clients.Where(c =>
                c.ClientName.ToLower().Contains(filter) ||
                (c.Inn != null && c.Inn.Contains(filter)) ||
                (c.Phone != null && c.Phone.ToLower().Contains(filter))).ToList();

            dataGrid.ItemsSource = filtered;
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is Client selectedClient)
            {
                txtName.Text = selectedClient.ClientName;
                txtInn.Text = selectedClient.Inn;
                txtPhone.Text = selectedClient.Phone ?? "";
                txtAddress.Text = selectedClient.LegalAddress ?? "";
                currentId = selectedClient.ClientId;
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtInn.Text))
            {
                MessageBox.Show("Название и ИНН обязательны для заполнения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var client = new Client
            {
                ClientName = txtName.Text,
                Inn = txtInn.Text,
                Phone = txtPhone.Text,
                LegalAddress = txtAddress.Text
            };

            _context.Clients.Add(client);
            _context.SaveChanges();
            LoadData();
            ClearFields();
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (currentId > 0)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtInn.Text))
                {
                    MessageBox.Show("Название и ИНН обязательны для заполнения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var client = _context.Clients.Find(currentId);
                if (client != null)
                {
                    client.ClientName = txtName.Text;
                    client.Inn = txtInn.Text;
                    client.Phone = txtPhone.Text;
                    client.LegalAddress = txtAddress.Text;

                    _context.SaveChanges();
                    LoadData();
                    ClearFields();
                }
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (currentId > 0)
            {
                var result = MessageBox.Show("Точно удалить клиента?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var client = _context.Clients.Find(currentId);
                    if (client != null)
                    {
                        _context.Clients.Remove(client);
                        _context.SaveChanges();
                        LoadData();
                        ClearFields();
                    }
                }
            }
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtInn.Text = "";
            txtPhone.Text = "";
            txtAddress.Text = "";
            currentId = 0;
            dataGrid.SelectedItem = null;
        }
    }
}