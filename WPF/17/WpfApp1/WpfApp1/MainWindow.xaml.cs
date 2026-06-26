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
        private void BtnDashboard_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new DashboardPage());
        private void BtnIncidents_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new IncidentsPage());
        private void BtnBrigades_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new BrigadesPage());
        private void BtnWorkLogs_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new WorkLogsPage());
        private void BtnDistricts_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new DistrictsPage());
        private void BtnEquipment_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new EquipmentPage());
        private void BtnReports_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ReportsPage());
    }
}