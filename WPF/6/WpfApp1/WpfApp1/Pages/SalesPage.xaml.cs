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
    public partial class SalesPage : Page
    {
        private readonly ApplicationDbContext _context;
        private Реализация _currentSale;
        private ICollectionView _salesView;
        private bool _isUpdatingForm = false;

        public SalesPage()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Реализации.Include(r => r.Нефтепродукт).Include(r => r.Клиент).Load();
            _context.Нефтепродукты.Load();
            _context.Клиенты.Load();

            SalesGrid.ItemsSource = _context.Реализации.Local.ToObservableCollection();
            
            var products = _context.Нефтепродукты.Local.ToList();
            CmbProduct.ItemsSource = products;

            var clients = _context.Клиенты.Local.ToList();
            CmbClient.ItemsSource = clients;

            var filterProducts = products.ToList();
            filterProducts.Insert(0, new Нефтепродукт { ID_Продукта = 0, Название = "Все продукты" });
            ProductFilterComboBox.ItemsSource = filterProducts;
            ProductFilterComboBox.SelectedIndex = 0;

            var filterClients = clients.ToList();
            filterClients.Insert(0, new Клиент { ID_Клиента = 0, Название_Компании = "Все клиенты" });
            ClientFilterComboBox.ItemsSource = filterClients;
            ClientFilterComboBox.SelectedIndex = 0;

            ClearForm();

            _salesView = CollectionViewSource.GetDefaultView(SalesGrid.ItemsSource);
            _salesView.Filter = SalesFilter;
        }

        private void SalesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SalesGrid.SelectedItem is Реализация selected)
            {
                _isUpdatingForm = true;
                _currentSale = selected;
                CmbProduct.SelectedValue = selected.ID_Продукта;
                CmbClient.SelectedValue = selected.ID_Клиента;
                TxtQuantity.Text = selected.Количество_Тонн.ToString("0.##");
                TxtTotalCost.Text = selected.Общая_Стоимость.ToString("0.##");
                DpDate.SelectedDate = selected.Дата_Отгрузки;
                _isUpdatingForm = false;
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            _isUpdatingForm = true;
            SalesGrid.SelectedItem = null;
            _currentSale = null;
            CmbProduct.SelectedIndex = -1;
            CmbClient.SelectedIndex = -1;
            TxtQuantity.Text = "0";
            TxtTotalCost.Text = "0";
            DpDate.SelectedDate = DateTime.Now;
            _isUpdatingForm = false;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CmbProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите нефтепродукт!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CmbClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtQuantity.Text.Replace('.', ','), out decimal qty) || qty <= 0)
            {
                MessageBox.Show("Количество должно быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtTotalCost.Text.Replace('.', ','), out decimal totalCost) || totalCost < 0)
            {
                MessageBox.Show("Общая стоимость должна быть неотрицательным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentSale == null)
            {
                var newSale = new Реализация
                {
                    ID_Продукта = (int)CmbProduct.SelectedValue,
                    ID_Клиента = (int)CmbClient.SelectedValue,
                    Количество_Тонн = qty,
                    Общая_Стоимость = totalCost,
                    Дата_Отгрузки = DpDate.SelectedDate.Value
                };
                _context.Реализации.Add(newSale);
                _currentSale = newSale;
            }
            else
            {
                _currentSale.ID_Продукта = (int)CmbProduct.SelectedValue;
                _currentSale.ID_Клиента = (int)CmbClient.SelectedValue;
                _currentSale.Количество_Тонн = qty;
                _currentSale.Общая_Стоимость = totalCost;
                _currentSale.Дата_Отгрузки = DpDate.SelectedDate.Value;
            }

            try
            {
                _context.SaveChanges();
                SalesGrid.Items.Refresh();
                MessageBox.Show("Реализация сохранена!", "Ок", MessageBoxButton.OK, MessageBoxImage.Information);
                SalesGrid.SelectedItem = _currentSale;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка базы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSale == null) return;

            if (MessageBox.Show($"Удалить запись о реализации?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Реализации.Remove(_currentSale);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => _salesView?.Refresh();

        private bool SalesFilter(object obj)
        {
            if (obj is Реализация item)
            {
                bool matchesProduct = ProductFilterComboBox.SelectedValue == null || (int)ProductFilterComboBox.SelectedValue == 0 || item.ID_Продукта == (int)ProductFilterComboBox.SelectedValue;
                bool matchesClient = ClientFilterComboBox.SelectedValue == null || (int)ClientFilterComboBox.SelectedValue == 0 || item.ID_Клиента == (int)ClientFilterComboBox.SelectedValue;

                return matchesProduct && matchesClient;
            }
            return false;
        }

        private void UpdateCost()
        {
            if (_isUpdatingForm) return;

            if (CmbProduct.SelectedItem is Нефтепродукт product)
            {
                if (decimal.TryParse(TxtQuantity.Text.Replace('.', ','), out decimal qty))
                {
                    TxtTotalCost.Text = (product.Цена_За_Тонну * qty).ToString("0.##");
                }
            }
        }

        private void CmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateCost();

        private void TxtQuantity_TextChanged(object sender, TextChangedEventArgs e) => UpdateCost();
    }
}
