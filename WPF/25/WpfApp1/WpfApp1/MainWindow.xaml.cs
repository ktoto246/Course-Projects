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
using WpfApp1.Pages;

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
        }

        private void NavInspections_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new InspectionsPage());
        }

        private void NavClients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void NavVehicles_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new VehiclesPage());
        }

        private void NavServices_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ServicesPage());
        }

        private void NavEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }

        private void NavStats_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StatsPage());
        }
    }
}