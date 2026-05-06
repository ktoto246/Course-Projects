using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.ProductsPage());
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.ClientsPage());
        }

        private void BtnIncomes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.IncomesPage());
        }

        private void BtnSales_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.SalesPage());
        }

        // Обработчик для новой кнопки графиков
        private void BtnCharts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.ChartsPage());
        }
    }
}