using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CategoriesPage());
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }

        private void BtnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SuppliersPage());
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsPage());
        }

        private void BtnDeliveries_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DeliveriesPage());
        }

        private void BtnCharts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ChartsPage());
        }
    }
}