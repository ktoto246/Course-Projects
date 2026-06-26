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

            MainFrame.Content = new VehiclesPage(); 
        }

        private void BtnVehicles_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new VehiclesPage();
        }

        private void BtnTrips_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new TripsPage();
        }

        private void BtnRepairs_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new RepairsPage();
        }
        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new StatisticsPage();
        }
    }
}