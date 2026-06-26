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
    public partial class ProductsPage : Page
    {
        private readonly ApplicationDbContext _context;
        private Нефтепродукт _currentProduct;
        private ICollectionView _productsView;

        public ProductsPage()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Нефтепродукты.Load();
            ProductsGrid.ItemsSource = _context.Нефтепродукты.Local.ToObservableCollection();
            ClearForm();

            _productsView = CollectionViewSource.GetDefaultView(ProductsGrid.ItemsSource);
            _productsView.Filter = ProductsFilter;

            // Собираем уникальные классы опасности для фильтра
            var classes = _context.Нефтепродукты.Local
                .Select(p => p.Класс_Опасности.ToString())
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            classes.Insert(0, "Все");
            DangerClassFilterComboBox.ItemsSource = classes;
            DangerClassFilterComboBox.SelectedIndex = 0;
        }

        private void ProductsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Нефтепродукт selected)
            {
                _currentProduct = selected;
                TxtName.Text = selected.Название;
                TxtDangerClass.Text = selected.Класс_Опасности.ToString();
                TxtPrice.Text = selected.Цена_За_Тонну.ToString("0.##");
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            ProductsGrid.SelectedItem = null;
            _currentProduct = null;
            TxtName.Text = string.Empty;
            TxtDangerClass.Text = string.Empty;
            TxtPrice.Text = "0";
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtName.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Заполните название!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtDangerClass.Text, out int dangerClass) || dangerClass <= 0)
            {
                MessageBox.Show("Класс опасности должен быть целым положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtPrice.Text.Replace('.', ','), out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentProduct == null)
            {
                var newProd = new Нефтепродукт
                {
                    Название = name,
                    Класс_Опасности = dangerClass,
                    Цена_За_Тонну = price
                };
                _context.Нефтепродукты.Add(newProd);
                _currentProduct = newProd;
            }
            else
            {
                _currentProduct.Название = name;
                _currentProduct.Класс_Опасности = dangerClass;
                _currentProduct.Цена_За_Тонну = price;
            }

            try
            {
                _context.SaveChanges();
                ProductsGrid.Items.Refresh();
                UpdateFilterDropdown();
                MessageBox.Show("Нефтепродукт сохранен!", "Ок", MessageBoxButton.OK, MessageBoxImage.Information);
                ProductsGrid.SelectedItem = _currentProduct;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка базы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentProduct == null) return;

            if (MessageBox.Show($"Удалить '{_currentProduct.Название}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Нефтепродукты.Remove(_currentProduct);
                _context.SaveChanges();
                ClearForm();
                UpdateFilterDropdown();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _productsView?.Refresh();

        private void DangerClassFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => _productsView?.Refresh();

        private bool ProductsFilter(object obj)
        {
            if (obj is Нефтепродукт item)
            {
                string searchText = SearchTextBox.Text?.ToLower() ?? "";
                bool matchesSearch = string.IsNullOrWhiteSpace(searchText) ||
                                     (item.Название != null && item.Название.ToLower().Contains(searchText));

                string selectedClass = DangerClassFilterComboBox.SelectedItem as string ?? "Все";
                bool matchesClass = selectedClass == "Все" || item.Класс_Опасности.ToString() == selectedClass;

                return matchesSearch && matchesClass;
            }
            return false;
        }

        private void UpdateFilterDropdown()
        {
            string currentSelection = DangerClassFilterComboBox.SelectedItem as string;

            var classes = _context.Нефтепродукты.Local
                .Select(p => p.Класс_Опасности.ToString())
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            classes.Insert(0, "Все");
            DangerClassFilterComboBox.ItemsSource = classes;

            if (classes.Contains(currentSelection))
                DangerClassFilterComboBox.SelectedItem = currentSelection;
            else
                DangerClassFilterComboBox.SelectedIndex = 0;
        }
    }
}