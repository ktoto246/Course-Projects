using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Model;

namespace WpfApp1.Pages
{
    public partial class OrderItemsPage : Page
    {
        private RetailDbContext _context;
        private int _selectedId = 0;

        public OrderItemsPage()
        {
            InitializeComponent();
            _context = new RetailDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDropdowns();
            LoadData();
        }

        private void LoadDropdowns()
        {
            CmbOrder.ItemsSource = _context.Orders.ToList();
            CmbProduct.ItemsSource = _context.Products.ToList();
        }

        private void LoadData()
        {
            var query = _context.OrderItems
                                .Include(oi => oi.Order)
                                .Include(oi => oi.Product)
                                .AsQueryable();

            if (int.TryParse(TxtSearchOrder.Text.Trim(), out int orderId))
            {
                query = query.Where(oi => oi.OrderId == orderId);
            }

            GridOrderItems.ItemsSource = query.ToList();
        }

        private void CmbProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbProduct.SelectedItem is Product selectedProduct)
            {
                TxtUnitPrice.Text = selectedProduct.Price.ToString("F2");
            }
        }

        private bool ValidateForm()
        {
            if (CmbOrder.SelectedValue == null || CmbProduct.SelectedValue == null)
            {
                MessageBox.Show("Выбери чек и товар!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(TxtQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Количество должно быть больше нуля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!decimal.TryParse(TxtUnitPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Некорректная цена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var item = new OrderItem
            {
                OrderId = (int)CmbOrder.SelectedValue,
                ProductId = (int)CmbProduct.SelectedValue,
                Quantity = int.Parse(TxtQuantity.Text),
                UnitPrice = decimal.Parse(TxtUnitPrice.Text)
            };

            _context.OrderItems.Add(item);
            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0 || !ValidateForm()) return;
            var item = _context.OrderItems.Find(_selectedId);
            if (item != null)
            {
                item.OrderId = (int)CmbOrder.SelectedValue;
                item.ProductId = (int)CmbProduct.SelectedValue;
                item.Quantity = int.Parse(TxtQuantity.Text);
                item.UnitPrice = decimal.Parse(TxtUnitPrice.Text);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0) return;
            if (MessageBox.Show("Удалить позицию из чека?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var item = _context.OrderItems.Find(_selectedId);
                if (item != null)
                {
                    _context.OrderItems.Remove(item);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
            }
        }

        private void GridOrderItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridOrderItems.SelectedItem is OrderItem item)
            {
                _selectedId = item.Id;
                CmbOrder.SelectedValue = item.OrderId;
                CmbProduct.SelectedValue = item.ProductId;
                TxtQuantity.Text = item.Quantity.ToString();
                TxtUnitPrice.Text = item.UnitPrice.ToString("F2");
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            _selectedId = 0;
            CmbOrder.SelectedItem = null;
            CmbProduct.SelectedItem = null;
            TxtQuantity.Clear();
            TxtUnitPrice.Clear();
            GridOrderItems.SelectedItem = null;
        }

        private void TxtSearchOrder_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
    }
}