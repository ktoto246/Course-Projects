using System.Windows;
using WpfApp1.Models;
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (AppSession.CurrentRole == "Viewer")
            {
                LblSpravochniki.Visibility = Visibility.Collapsed;
                BtnEmployees.Visibility = Visibility.Collapsed;
                BtnDictionaries.Visibility = Visibility.Collapsed;
                BtnStatuses.Visibility = Visibility.Collapsed;
            }
            else if (AppSession.CurrentRole == "Operator")
            {
                LblSpravochniki.Visibility = Visibility.Collapsed;
                BtnEmployees.Visibility = Visibility.Collapsed;
                BtnDictionaries.Visibility = Visibility.Collapsed;
                BtnStatuses.Visibility = Visibility.Collapsed;
            }

            MainFrame.Navigate(new Pages.EquipmentsPage());
        }

        private void BtnEquipments_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.EquipmentsPage());
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.EmployeesPage());
        }

        private void BtnStatuses_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.StatusesPage());
        }

        private void BtnDictionaries_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.CategoriesPage());
        }

        private void BtnMovementHistory_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.MovementHistoryPage());
        }

        private void BtnRepairHistory_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.RepairHistoryPage());
        }
        private void BtnStatusChart_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.StatusAnalyticsPage());
        }

        private void BtnCategoryChart_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Pages.CategoryAnalyticsPage());
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти из системы?",
                                         "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                AppSession.CurrentRole = null;

        LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                this.Close();
            }
        }

        private void OpenFinancial_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new FinancialAnalyticsPage());
        }

        private void OpenDeadlines_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DeadlinesAnalyticsPage()); 
        }

        private void OpenRepairCosts_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RepairCostsAnalyticsPage());
        }

        private void OpenWorkload_Click(object sender, RoutedEventArgs e)
        {

            MainFrame.Navigate(new WorkloadAnalyticsPage());
        }
    }
}