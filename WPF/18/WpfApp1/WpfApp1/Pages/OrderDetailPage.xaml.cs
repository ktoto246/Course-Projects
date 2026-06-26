using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class OrderDetailPage : Page
    {
        private AppDbContext _context;
        private List<OrderDetail> _orderDetails = new List<OrderDetail>();
        private int currentId = 0;

        public OrderDetailPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadComboBoxData();
            LoadData();
        }

        private void LoadData()
        {
            // Обязательно Include, чтобы в гриде работало Binding="{Binding Product.ProductName}"
            _orderDetails = _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .ToList();
            dataGrid.ItemsSource = _orderDetails;
        }

        private void LoadComboBoxData()
        {
            cbOrder.ItemsSource = _context.Orders.ToList();
            cbOrder.DisplayMemberPath = "OrderId"; // Показываем просто номер заказа
            cbOrder.SelectedValuePath = "OrderId";

            cbProduct.ItemsSource = _context.Products.ToList();
            cbProduct.DisplayMemberPath = "ProductName"; // Показываем название муки
            cbProduct.SelectedValuePath = "ProductId";
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox searchBox = sender as TextBox;
            if (searchBox == null) return;

            string filter = searchBox.Text.ToLower();
            var filtered = _orderDetails.Where(od =>
                (od.Product != null && od.Product.ProductName.ToLower().Contains(filter)) ||
                od.OrderId.ToString().Contains(filter)).ToList();
            dataGrid.ItemsSource = filtered;
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is OrderDetail selectedOrderDetail)
            {
                cbOrder.SelectedValue = selectedOrderDetail.OrderId;
                cbProduct.SelectedValue = selectedOrderDetail.ProductId;

                // Для корректного отображения десятичных дробей
                txtQuantityTons.Text = selectedOrderDetail.QuantityTons.ToString("0.###");
                currentId = selectedOrderDetail.OrderDetailId;
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (cbOrder.SelectedValue == null || cbProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите заказ и продукт из списков!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Заменяем запятую на точку, чтобы парсер не ебал мозги с локалью
            string qtyText = txtQuantityTons.Text.Replace('.', ',');
            if (!decimal.TryParse(qtyText, out decimal qty) || qty <= 0)
            {
                MessageBox.Show("Введите корректное количество тонн (число больше нуля)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var orderDetail = new OrderDetail
            {
                OrderId = (int)cbOrder.SelectedValue,
                ProductId = (int)cbProduct.SelectedValue,
                QuantityTons = qty
            };

            _context.OrderDetails.Add(orderDetail);
            _context.SaveChanges();
            LoadData();
            ClearFields();
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (currentId > 0)
            {
                if (cbOrder.SelectedValue == null || cbProduct.SelectedValue == null)
                {
                    MessageBox.Show("Выберите заказ и продукт из списков!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string qtyText = txtQuantityTons.Text.Replace('.', ',');
                if (!decimal.TryParse(qtyText, out decimal qty) || qty <= 0)
                {
                    MessageBox.Show("Введите корректное количество тонн (число больше нуля)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var orderDetail = _context.OrderDetails.Find(currentId);
                if (orderDetail != null)
                {
                    orderDetail.OrderId = (int)cbOrder.SelectedValue;
                    orderDetail.ProductId = (int)cbProduct.SelectedValue;
                    orderDetail.QuantityTons = qty;

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
                var result = MessageBox.Show("Точно удалить позицию из заказа?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var orderDetail = _context.OrderDetails.Find(currentId);
                    if (orderDetail != null)
                    {
                        _context.OrderDetails.Remove(orderDetail);
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
            cbOrder.SelectedValue = null;
            cbProduct.SelectedValue = null;
            txtQuantityTons.Text = "";
            currentId = 0;
            dataGrid.SelectedItem = null;
        }
    }
}