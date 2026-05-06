using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class ClientsPage : Page
    {
        private readonly CarRentalContext _context;
        private Клиент? _currentClient;
        private ICollectionView? _clientsView;

        public ClientsPage()
        {
            InitializeComponent();
            _context = new CarRentalContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Клиенты.Load();
            ClientsGrid.ItemsSource = _context.Клиенты.Local.ToObservableCollection();
            _clientsView = CollectionViewSource.GetDefaultView(ClientsGrid.ItemsSource);
            _clientsView.Filter = FilterClients;
            ClearForm();
        }

        private bool FilterClients(object obj)
        {
            if (obj is not Клиент item) return false;
            string search = SearchTextBox.Text?.ToLower() ?? string.Empty;
            return string.IsNullOrWhiteSpace(search) ||
                   item.ФИО.ToLower().Contains(search) ||
                   item.Телефон.Contains(search);
        }

        private void ClientsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Клиент selected)
            {
                _currentClient = selected;
                TxtFullName.Text = selected.ФИО;
                TxtPassport.Text = selected.Паспорт;
                TxtLicense.Text = selected.ВУ;
                TxtPhone.Text = selected.Телефон;
                CbBlacklist.IsChecked = selected.Черный_Список;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDataValid()) return;
            if (string.IsNullOrWhiteSpace(TxtFullName.Text)) return;
            try
            {
                if (_currentClient == null)
                {
                    var newItem = new Клиент
                    {
                        ФИО = TxtFullName.Text,
                        Паспорт = TxtPassport.Text,
                        ВУ = TxtLicense.Text,
                        Телефон = TxtPhone.Text,
                        Черный_Список = CbBlacklist.IsChecked ?? false
                    };
                    _context.Клиенты.Add(newItem);
                }
                else
                {
                    _currentClient.ФИО = TxtFullName.Text;
                    _currentClient.Паспорт = TxtPassport.Text;
                    _currentClient.ВУ = TxtLicense.Text;
                    _currentClient.Телефон = TxtPhone.Text;
                    _currentClient.Черный_Список = CbBlacklist.IsChecked ?? false;
                }
                _context.SaveChanges();
                ClientsGrid.Items.Refresh();
                ClearForm();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentClient != null)
            {
                _context.Клиенты.Remove(_currentClient);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _clientsView?.Refresh();

        private void ClearForm()
        {
            ClientsGrid.SelectedItem = null;
            _currentClient = null;
            TxtFullName.Text = TxtPassport.Text = TxtLicense.Text = TxtPhone.Text = string.Empty;
            CbBlacklist.IsChecked = false;
        }
        private bool IsDataValid()
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                MessageBox.Show("Введите ФИО клиента!", "Ошибка");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtPassport.Text) || TxtPassport.Text.Length < 10)
            {
                MessageBox.Show("Введите корректные паспортные данные!", "Ошибка");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtLicense.Text))
            {
                MessageBox.Show("Укажите номер водительского удостоверения!", "Ошибка");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtPhone.Text) || TxtPhone.Text.Length < 10)
            {
                MessageBox.Show("Введите корректный номер телефона!", "Ошибка");
                return false;
            }

            return true;
        }
    }
}