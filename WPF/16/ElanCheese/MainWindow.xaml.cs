using System;
using System.Windows;
using System.Windows.Controls;

namespace ElanCheeseApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void OnReportsClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.ReportsPage());
        }
        
        private void OnCheeseBatchesClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.CheeseBatchesPage());
        }
        
        private void OnCheeseVarietiesClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.CheeseVarietiesPage());
        }
        
        private void OnStorageChambersClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.StorageChambersPage());
        }
        
        private void OnEmployeesClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.EmployeesPage());
        }
        
        private void OnQualityInspectionsClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.QualityInspectionsPage());
        }
    }
}
