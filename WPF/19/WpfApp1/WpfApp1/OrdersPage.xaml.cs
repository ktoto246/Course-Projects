using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class OrdersPage : Page
    {
        private Order _editingOrder;

        public OrdersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            using (var context = new ElkiTorgContext())
            {
                var query = context.Orders.Include(o => o.Client).AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(o => o.Client.CompanyName.Contains(searchText) ||
                                             o.OrderStatus.Contains(searchText));
                }

                OrdersGrid.ItemsSource = query.ToList();
            }
        }

        private void LoadClients()
        {
            using (var context = new ElkiTorgContext())
            {
                CmbClients.ItemsSource = context.Clients.ToList();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _editingOrder = null;
            PanelTitle.Text = "Новый заказ";

            LoadClients();
            CmbClients.SelectedItem = null;
            DpOrderDate.SelectedDate = DateTime.Now;
            CmbStatus.Text = "Новый";

            EditPanel.Visibility = Visibility.Visible;
            OrdersGrid.IsEnabled = false;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                _editingOrder = selectedOrder;
                PanelTitle.Text = "Редактирование";

                LoadClients();
                CmbClients.SelectedValue = selectedOrder.ClientID;
                DpOrderDate.SelectedDate = selectedOrder.OrderDate;
                CmbStatus.Text = selectedOrder.OrderStatus;

                EditPanel.Visibility = Visibility.Visible;
                OrdersGrid.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Сначала выберите заказ в таблице.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                if (MessageBox.Show("Удалить выбранный заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var context = new ElkiTorgContext())
                    {
                        var orderToDelete = context.Orders.Find(selectedOrder.OrderID);
                        if (orderToDelete != null)
                        {
                            context.Orders.Remove(orderToDelete);
                            context.SaveChanges();
                            LoadData(TxtSearch.Text);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите заказ.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbClients.SelectedValue == null || !DpOrderDate.SelectedDate.HasValue || string.IsNullOrWhiteSpace(CmbStatus.Text))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new ElkiTorgContext())
            {
                if (_editingOrder == null)
                {
                    var newOrder = new Order
                    {
                        ClientID = (int)CmbClients.SelectedValue,
                        OrderDate = DpOrderDate.SelectedDate.Value,
                        OrderStatus = CmbStatus.Text
                    };
                    context.Orders.Add(newOrder);
                }
                else
                {
                    var orderToUpdate = context.Orders.Find(_editingOrder.OrderID);
                    if (orderToUpdate != null)
                    {
                        orderToUpdate.ClientID = (int)CmbClients.SelectedValue;
                        orderToUpdate.OrderDate = DpOrderDate.SelectedDate.Value;
                        orderToUpdate.OrderStatus = CmbStatus.Text;
                    }
                }
                context.SaveChanges();
            }

            ClosePanel();
            LoadData(TxtSearch.Text);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel();
        }

        private void ClosePanel()
        {
            EditPanel.Visibility = Visibility.Collapsed;
            OrdersGrid.IsEnabled = true;
        }
    }
}