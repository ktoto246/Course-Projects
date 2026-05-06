using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Model;

namespace WpfApp1.Pages
{
    public partial class OrdersPage : Page
    {
        private RetailDbContext _context;
        private int _selectedId = 0;

        public OrdersPage()
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
            CmbEmployee.ItemsSource = _context.Employees.ToList();
            CmbCustomer.ItemsSource = _context.Customers.ToList();
        }

        private void LoadData()
        {
            var query = _context.Orders.Include(o => o.Employee).Include(o => o.Customer).AsQueryable();

            if (int.TryParse(TxtSearch.Text.Trim(), out int orderId))
            {
                query = query.Where(o => o.Id == orderId);
            }

            GridOrders.ItemsSource = query.ToList();
        }

        private bool ValidateForm()
        {
            if (CmbEmployee.SelectedValue == null)
            {
                MessageBox.Show("Выбери сотрудника, который пробивает чек!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!decimal.TryParse(TxtTotalAmount.Text, out decimal total) || total < 0)
            {
                MessageBox.Show("Некорректная сумма!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var order = new Order
            {
                EmployeeId = (int)CmbEmployee.SelectedValue,
                CustomerId = CmbCustomer.SelectedValue != null ? (int)CmbCustomer.SelectedValue : null,
                TotalAmount = decimal.Parse(TxtTotalAmount.Text),
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0 || !ValidateForm()) return;
            var order = _context.Orders.Find(_selectedId);
            if (order != null)
            {
                order.EmployeeId = (int)CmbEmployee.SelectedValue;
                order.CustomerId = CmbCustomer.SelectedValue != null ? (int)CmbCustomer.SelectedValue : null;
                order.TotalAmount = decimal.Parse(TxtTotalAmount.Text);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0) return;
            if (MessageBox.Show("Удалить чек? Это нежелательное действие для бухгалтерии, но вы можете подтвердить.", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var order = _context.Orders.Find(_selectedId);
                if (order != null)
                {
                    _context.Orders.Remove(order);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
            }
        }

        private void GridOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridOrders.SelectedItem is Order order)
            {
                _selectedId = order.Id;
                CmbEmployee.SelectedValue = order.EmployeeId;
                CmbCustomer.SelectedValue = order.CustomerId;
                TxtTotalAmount.Text = order.TotalAmount.ToString("F2");
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            _selectedId = 0;
            CmbEmployee.SelectedItem = null;
            CmbCustomer.SelectedItem = null;
            TxtTotalAmount.Clear();
            GridOrders.SelectedItem = null;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
    }
}