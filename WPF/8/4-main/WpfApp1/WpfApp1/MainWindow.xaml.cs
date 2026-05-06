using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new CarsPage());
        }

        private void BtnCars_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CarsPage());
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void BtnRent_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RentPage());
        }

        private void BtnCrash_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CrashPage());
        }

        private void BtnStatistics_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StatisticsPage());
        }
    }
}