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

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
           MainFrame.Navigate(new CategoriesPage());
        }

        private void BtnResidents_Click(object sender, RoutedEventArgs e)
        {
           MainFrame.Navigate(new ResidentsPage());
        }

        private void BtnItems_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ItemsPage());
        }

        private void BtnIssuanceLog_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new IssuanceLogPage());
        }

        private void BtnAnalytics_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AnalyticsPage());
        }
    }
}