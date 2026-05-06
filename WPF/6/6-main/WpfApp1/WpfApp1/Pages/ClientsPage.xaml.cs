using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class ClientsPage : Page
    {
        private readonly ApplicationDbContext _context;
        private Клиент _currentClient;
        private ICollectionView _clientsView;

        public ClientsPage()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Клиенты.Load();
            ClientsGrid.ItemsSource = _context.Клиенты.Local.ToObservableCollection();
            ClearForm();

            _clientsView = CollectionViewSource.GetDefaultView(ClientsGrid.ItemsSource);
            _clientsView.Filter = ClientsFilter;
        }

        private void ClientsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Клиент selected)
            {
                _currentClient = selected;
                TxtName.Text = selected.Название_Компании;
                TxtINN.Text = selected.ИНН;
                TxtPhone.Text = selected.Контактный_Телефон;
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            ClientsGrid.SelectedItem = null;
            _currentClient = null;
            TxtName.Text = string.Empty;
            TxtINN.Text = string.Empty;
            TxtPhone.Text = string.Empty;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtName.Text.Trim();
            string inn = TxtINN.Text.Trim();
            string phone = TxtPhone.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Заполните название компании!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(inn))
            {
                MessageBox.Show("Заполните ИНН!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentClient == null)
            {
                var newClient = new Клиент
                {
                    Название_Компании = name,
                    ИНН = inn,
                    Контактный_Телефон = phone
                };
                _context.Клиенты.Add(newClient);
                _currentClient = newClient;
            }
            else
            {
                _currentClient.Название_Компании = name;
                _currentClient.ИНН = inn;
                _currentClient.Контактный_Телефон = phone;
            }

            try
            {
                _context.SaveChanges();
                ClientsGrid.Items.Refresh();
                MessageBox.Show("Клиент сохранен!", "Ок", MessageBoxButton.OK, MessageBoxImage.Information);
                ClientsGrid.SelectedItem = _currentClient;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка базы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentClient == null) return;

            if (MessageBox.Show($"Удалить '{_currentClient.Название_Компании}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Клиенты.Remove(_currentClient);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _clientsView?.Refresh();

        private bool ClientsFilter(object obj)
        {
            if (obj is Клиент item)
            {
                string searchText = SearchTextBox.Text?.ToLower() ?? "";
                return string.IsNullOrWhiteSpace(searchText) ||
                       (item.Название_Компании != null && item.Название_Компании.ToLower().Contains(searchText)) ||
                       (item.ИНН != null && item.ИНН.ToLower().Contains(searchText));
            }
            return false;
        }
    }
}
