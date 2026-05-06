using System.Windows;
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BtnProducts.Click += (s, e) => MainFrame.Navigate(new ProductsPage());
            BtnCategories.Click += (s, e) => MainFrame.Navigate(new CategoriesPage());
            BtnCustomers.Click += (s, e) => MainFrame.Navigate(new CustomersPage());
            BtnOrders.Click += (s, e) => MainFrame.Navigate(new OrdersPage());
            BtnAnalytics.Click += (s, e) => MainFrame.Navigate(new AnalyticsPage());
            BtnEmployees.Click += (s, e) => MainFrame.Navigate(new EmployeesPage());
            BtnOrderItems.Click += (s, e) => MainFrame.Navigate(new OrderItemsPage());

            MainFrame.Navigate(new ProductsPage());
        }
    }
}