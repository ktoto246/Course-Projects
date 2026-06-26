using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void NavDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void NavSuppliers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SuppliersPage());
        }

        private void NavStaff_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StaffPage());
        }

        private void NavIndicators_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new IndicatorsPage());
        }

        private void NavBatches_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BatchesPage());
        }

        private void NavAnalysis_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AnalysisPage());
        }
    }
}