using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class OrderDetailsPage : Page
    {
        private OrderDetail _editingDetail;

        public class OrderFilterItem
        {
            public int? OrderID { get; set; }
            public string DisplayText { get; set; }
        }

        public OrderDetailsPage()
        {
            InitializeComponent();
            LoadFilterOrders();
            LoadData();
        }

        private void LoadFilterOrders()
        {
            using (var context = new ElkiTorgContext())
            {
                var orders = context.Orders.Include(o => o.Client).ToList();
                var filterList = new List<OrderFilterItem>
                {
                    new OrderFilterItem { OrderID = null, DisplayText = "Все заказы" }
                };

                filterList.AddRange(orders.Select(o => new OrderFilterItem
                {
                    OrderID = o.OrderID,
                    DisplayText = $"Заказ №{o.OrderID} ({o.Client.CompanyName})"
                }));

                CmbFilterOrders.ItemsSource = filterList;
                CmbFilterOrders.SelectedIndex = 0;
            }
        }

        private void LoadData()
        {
            using (var context = new ElkiTorgContext())
            {
                var query = context.OrderDetails
                    .Include(od => od.Order)
                    .ThenInclude(o => o.Client)
                    .Include(od => od.Product)
                    .AsQueryable();

                if (CmbFilterOrders.SelectedValue != null)
                {
                    int selectedOrderId = (int)CmbFilterOrders.SelectedValue;
                    query = query.Where(od => od.OrderID == selectedOrderId);
                }

                string searchText = TxtSearch.Text;
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(od => od.Product.ModelName.Contains(searchText) ||
                                              od.Order.Client.CompanyName.Contains(searchText) ||
                                              od.OrderID.ToString().Contains(searchText));
                }

                OrderDetailsGrid.ItemsSource = query.ToList();
            }
        }

        private void CmbFilterOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void LoadComboBoxes()
        {
            using (var context = new ElkiTorgContext())
            {
                var orders = context.Orders.Include(o => o.Client).ToList();
                CmbOrders.ItemsSource = orders.Select(o => new
                {
                    OrderID = o.OrderID,
                    DisplayText = $"Заказ №{o.OrderID} ({o.Client.CompanyName})"
                }).ToList();

                CmbProducts.ItemsSource = context.Products.ToList();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _editingDetail = null;
            PanelTitle.Text = "Добавление товара";

            LoadComboBoxes();
            CmbOrders.SelectedItem = null;

            if (CmbFilterOrders.SelectedValue != null)
            {
                CmbOrders.SelectedValue = CmbFilterOrders.SelectedValue;
            }

            CmbProducts.SelectedItem = null;
            TxtQuantity.Text = "1";

            EditPanel.Visibility = Visibility.Visible;
            OrderDetailsGrid.IsEnabled = false;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (OrderDetailsGrid.SelectedItem is OrderDetail selectedDetail)
            {
                _editingDetail = selectedDetail;
                PanelTitle.Text = "Редактирование";

                LoadComboBoxes();
                CmbOrders.SelectedValue = selectedDetail.OrderID;
                CmbProducts.SelectedValue = selectedDetail.ProductID;
                TxtQuantity.Text = selectedDetail.Quantity.ToString();

                EditPanel.Visibility = Visibility.Visible;
                OrderDetailsGrid.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Сначала выберите позицию в таблице.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (OrderDetailsGrid.SelectedItem is OrderDetail selectedDetail)
            {
                if (MessageBox.Show("Удалить выбранную позицию из заказа?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var context = new ElkiTorgContext())
                    {
                        var detailToDelete = context.OrderDetails.Find(selectedDetail.OrderDetailID);
                        if (detailToDelete != null)
                        {
                            context.OrderDetails.Remove(detailToDelete);
                            context.SaveChanges();
                            LoadData();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите позицию.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbOrders.SelectedValue == null || CmbProducts.SelectedValue == null)
            {
                MessageBox.Show("Выберите заказ и товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество (целое число больше нуля).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new ElkiTorgContext())
            {
                if (_editingDetail == null)
                {
                    var newDetail = new OrderDetail
                    {
                        OrderID = (int)CmbOrders.SelectedValue,
                        ProductID = (int)CmbProducts.SelectedValue,
                        Quantity = quantity
                    };
                    context.OrderDetails.Add(newDetail);
                }
                else
                {
                    var detailToUpdate = context.OrderDetails.Find(_editingDetail.OrderDetailID);
                    if (detailToUpdate != null)
                    {
                        detailToUpdate.OrderID = (int)CmbOrders.SelectedValue;
                        detailToUpdate.ProductID = (int)CmbProducts.SelectedValue;
                        detailToUpdate.Quantity = quantity;
                    }
                }
                context.SaveChanges();
            }

            ClosePanel();
            LoadData();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel();
        }

        private void ClosePanel()
        {
            EditPanel.Visibility = Visibility.Collapsed;
            OrderDetailsGrid.IsEnabled = true;
        }
    }
}