using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class OrderPage : Page
    {
        private AppDbContext _context;
        private List<Order> _orders = new List<Order>();
        private int currentId = 0;

        public OrderPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadComboBoxData();
            LoadData();
        }

        private void LoadData()
        {
            _orders = _context.Orders
         .Include(o => o.Mill)
         .Include(o => o.Client)
         .Include(o => o.OrderDetails)
             .ThenInclude(od => od.Product)
         .ToList();

            dataGrid.ItemsSource = _orders;
        }

        private void LoadComboBoxData()
        {
            cbMill.ItemsSource = _context.Mills.ToList();
            cbMill.DisplayMemberPath = "MillName";
            cbMill.SelectedValuePath = "MillId";

            cbClient.ItemsSource = _context.Clients.ToList();
            cbClient.DisplayMemberPath = "ClientName";
            cbClient.SelectedValuePath = "ClientId";
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox searchBox = sender as TextBox;
            if (searchBox == null) return;

            string filter = searchBox.Text.ToLower();
            var filtered = _orders.Where(o =>
                (o.Mill != null && o.Mill.MillName.ToLower().Contains(filter)) ||
                (o.Client != null && o.Client.ClientName.ToLower().Contains(filter)) ||
                o.OrderId.ToString().Contains(filter)).ToList();

            dataGrid.ItemsSource = filtered;
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is Order selectedOrder)
            {
                cbMill.SelectedValue = selectedOrder.MillId;
                cbClient.SelectedValue = selectedOrder.ClientId;
                dpOrderDate.SelectedDate = selectedOrder.OrderDate;
                currentId = selectedOrder.OrderId;
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (cbMill.SelectedValue == null || cbClient.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента и мельницу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var order = new Order
            {
                MillId = (int)cbMill.SelectedValue,
                ClientId = (int)cbClient.SelectedValue,
                // Если дату не выбрали, ставим текущую
                OrderDate = dpOrderDate.SelectedDate ?? DateTime.Now
            };

            _context.Orders.Add(order);
            _context.SaveChanges();
            LoadData();
            ClearFields();
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (currentId > 0)
            {
                if (cbMill.SelectedValue == null || cbClient.SelectedValue == null)
                {
                    MessageBox.Show("Выберите клиента и мельницу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var order = _context.Orders.Find(currentId);
                if (order != null)
                {
                    order.MillId = (int)cbMill.SelectedValue;
                    order.ClientId = (int)cbClient.SelectedValue;

                    if (dpOrderDate.SelectedDate.HasValue)
                    {
                        order.OrderDate = dpOrderDate.SelectedDate.Value;
                    }

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
                var result = MessageBox.Show("Точно удалить заказ? Все детали этого заказа тоже удалятся!", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var order = _context.Orders.Find(currentId);
                    if (order != null)
                    {
                        _context.Orders.Remove(order);
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
            cbMill.SelectedValue = null;
            cbClient.SelectedValue = null;
            dpOrderDate.SelectedDate = null;
            currentId = 0;
            dataGrid.SelectedItem = null;
        }
    }
}