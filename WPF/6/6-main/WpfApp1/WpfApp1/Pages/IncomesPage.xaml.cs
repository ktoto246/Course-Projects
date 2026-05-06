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
    public partial class IncomesPage : Page
    {
        private readonly ApplicationDbContext _context;
        private Поступление _currentIncome;
        private ICollectionView _incomesView;

        public IncomesPage()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Поступления.Include(p => p.Нефтепродукт).Load();
            _context.Нефтепродукты.Load();

            IncomesGrid.ItemsSource = _context.Поступления.Local.ToObservableCollection();
            
            var products = _context.Нефтепродукты.Local.ToList();
            CmbProduct.ItemsSource = products;

            var filterProducts = products.ToList();
            filterProducts.Insert(0, new Нефтепродукт { ID_Продукта = 0, Название = "Все продукты" });
            ProductFilterComboBox.ItemsSource = filterProducts;
            ProductFilterComboBox.SelectedIndex = 0;

            ClearForm();

            _incomesView = CollectionViewSource.GetDefaultView(IncomesGrid.ItemsSource);
            _incomesView.Filter = IncomesFilter;
        }

        private void IncomesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IncomesGrid.SelectedItem is Поступление selected)
            {
                _currentIncome = selected;
                CmbProduct.SelectedValue = selected.ID_Продукта;
                TxtQuantity.Text = selected.Количество_Тонн.ToString("0.##");
                DpDate.SelectedDate = selected.Дата_Поступления;
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            IncomesGrid.SelectedItem = null;
            _currentIncome = null;
            CmbProduct.SelectedIndex = -1;
            TxtQuantity.Text = "0";
            DpDate.SelectedDate = DateTime.Now;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CmbProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите нефтепродукт!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtQuantity.Text.Replace('.', ','), out decimal qty) || qty <= 0)
            {
                MessageBox.Show("Количество должно быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DpDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_currentIncome == null)
            {
                var newInc = new Поступление
                {
                    ID_Продукта = (int)CmbProduct.SelectedValue,
                    Количество_Тонн = qty,
                    Дата_Поступления = DpDate.SelectedDate.Value
                };
                _context.Поступления.Add(newInc);
                _currentIncome = newInc;
            }
            else
            {
                _currentIncome.ID_Продукта = (int)CmbProduct.SelectedValue;
                _currentIncome.Количество_Тонн = qty;
                _currentIncome.Дата_Поступления = DpDate.SelectedDate.Value;
            }

            try
            {
                _context.SaveChanges();
                IncomesGrid.Items.Refresh();
                MessageBox.Show("Поступление сохранено!", "Ок", MessageBoxButton.OK, MessageBoxImage.Information);
                IncomesGrid.SelectedItem = _currentIncome;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка базы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIncome == null) return;

            if (MessageBox.Show($"Удалить поступление?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                _context.Поступления.Remove(_currentIncome);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void ProductFilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => _incomesView?.Refresh();

        private bool IncomesFilter(object obj)
        {
            if (obj is Поступление item)
            {
                if (ProductFilterComboBox.SelectedValue == null || (int)ProductFilterComboBox.SelectedValue == 0)
                    return true;

                return item.ID_Продукта == (int)ProductFilterComboBox.SelectedValue;
            }
            return false;
        }
    }
}
