using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class OrdersPage : Page
    {
        private AlphabetContext _context;

        public OrdersPage()
        {
            InitializeComponent();
            _context = new AlphabetContext();
            LoadOrders();
        }

        private void LoadOrders(string searchText = "")
        {
            var query = _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Employee)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();
                query = query.Where(o =>
                    o.Status.ToLower().Contains(searchText) ||
                    (o.Client != null && o.Client.ContactName.ToLower().Contains(searchText)) ||
                    (o.Employee != null && o.Employee.FullName.ToLower().Contains(searchText)));
            }

            OrdersGrid.ItemsSource = query.ToList();
            DetailsGrid.ItemsSource = null;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadOrders(TxtSearch.Text);
        }

        private void OrdersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDetails();
        }

        private void LoadDetails()
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                DetailsGrid.ItemsSource = _context.OrderDetails
                    .Include(od => od.Service)
                    .Where(od => od.OrderId == selectedOrder.OrderId)
                    .ToList();
            }
            else
            {
                DetailsGrid.ItemsSource = null;
            }
        }

        public void AddOrder()
        {
            var newOrder = new Order { OrderDate = System.DateTime.Now, Status = "Новый" };
            var window = new OrderWindow(newOrder, _context.Clients.ToList(), _context.Employees.ToList());

            if (window.ShowDialog() == true)
            {
                _context.Orders.Add(newOrder);
                _context.SaveChanges();
                LoadOrders(TxtSearch.Text);
            }
        }

        public void EditOrder()
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                var window = new OrderWindow(selectedOrder, _context.Clients.ToList(), _context.Employees.ToList());
                if (window.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    LoadOrders(TxtSearch.Text);
                }
            }
            else
            {
                MessageBox.Show("Выберите заказ для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void DeleteOrder()
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                var result = MessageBox.Show($"Удалить заказ №{selectedOrder.OrderId} со всеми его деталями?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Orders.Remove(selectedOrder);
                    _context.SaveChanges();
                    LoadOrders(TxtSearch.Text);
                }
            }
        }

        private void BtnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersGrid.SelectedItem is Order selectedOrder)
            {
                var newDetail = new OrderDetail { OrderId = selectedOrder.OrderId };
                var window = new OrderDetailWindow(newDetail, _context.Services.ToList());

                if (window.ShowDialog() == true)
                {
                    _context.OrderDetails.Add(newDetail);
                    _context.SaveChanges();
                    LoadDetails();
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите заказ в верхней таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDeleteDetail_Click(object sender, RoutedEventArgs e)
        {
            if (DetailsGrid.SelectedItem is OrderDetail selectedDetail)
            {
                var result = MessageBox.Show("Удалить выбранную услугу из заказа?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _context.OrderDetails.Remove(selectedDetail);
                    _context.SaveChanges();
                    LoadDetails();
                }
            }
        }
    }
}