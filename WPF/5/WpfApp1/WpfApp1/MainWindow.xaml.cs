using System;
using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductsPage());
        }

        private void BtnDeals_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DealsPage());
        }
        private void BtnAnalytics_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AnalyticsPage());
        }
    }
}