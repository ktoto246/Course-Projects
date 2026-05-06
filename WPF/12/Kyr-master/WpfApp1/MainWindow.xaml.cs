using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Pages.WatercraftsPage());
        }
        private void BtnWatercrafts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.WatercraftsPage());
        }

        private void BtnRentals_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.RentalsPage());
        }

        private void BtnDamages_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.DamagesPage());
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.ClientsPage());
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.EmployeesPage());
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.CategoriesPage());
        }

        private void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.StatisticsPage());
        }
    }
}