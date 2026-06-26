using System.Windows;
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NavEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }

        private void NavCounterparties_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CounterpartiesPage());
        }

        private void NavPlotsCrops_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PlotsCropsPage());
        }

        private void NavTransactions_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TransactionsPage());
        }

        private void NavReports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReportsPage());
        }
    }
}