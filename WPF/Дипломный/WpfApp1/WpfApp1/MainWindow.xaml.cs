using System.Data.SqlTypes;
using System.Windows;
using WpfApp1.Helpers;
using WpfApp1.Models;
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            btnDashboard.Click += (s, e) => {
                string r = CurrentSession.CurrentUser?.Role;
                if (r == "Администратор") MainFrame.Navigate(new AdminDashboardPage());
                else if (r == "Весовщик") MainFrame.Navigate(new WeigherDashboardPage());
                else if (r == "Лаборант") MainFrame.Navigate(new LabDashboardPage());
                else if (r == "Менеджер") MainFrame.Navigate(new ManagerDashboardPage());
            };

            btnEmployees.Click += (s, e) => MainFrame.Navigate(new EmployeesPage());
            btnClients.Click += (s, e) => MainFrame.Navigate(new ClientsPage());
            btnCrops.Click += (s, e) => MainFrame.Navigate(new CropsPage());
            btnStorages.Click += (s, e) => MainFrame.Navigate(new StoragesPage());
            btnServices.Click += (s, e) => MainFrame.Navigate(new ServicesPage());
            btnBatches.Click += (s, e) => MainFrame.Navigate(new BatchesPage());
            btnLabTests.Click += (s, e) => MainFrame.Navigate(new LabTestsPage());
            btnRenderedServices.Click += (s, e) => MainFrame.Navigate(new RenderedServicesPage());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentSession.CurrentUser != null)
            {
                txtUserName.Text = CurrentSession.CurrentUser.FullName;
                txtUserRole.Text = CurrentSession.CurrentUser.Role;
            }
            ApplyRoleRestrictions();
        }

        private void ApplyRoleRestrictions()
        {
            if (CurrentSession.CurrentUser == null)
                return;

            string role = CurrentSession.CurrentUser.Role;

            if (role == "Весовщик")
            {
                btnEmployees.Visibility = Visibility.Collapsed;
                btnStorages.Visibility = Visibility.Collapsed;
                btnServices.Visibility = Visibility.Collapsed;
                btnLabTests.Visibility = Visibility.Collapsed;
                btnRenderedServices.Visibility = Visibility.Collapsed;
                btnClients.Visibility = Visibility.Collapsed;
                btnCrops.Visibility = Visibility.Collapsed;

                MainFrame.Navigate(new WeigherDashboardPage());
            }
            else if (role == "Лаборант")
            {
                btnEmployees.Visibility = Visibility.Collapsed;
                btnClients.Visibility = Visibility.Collapsed;
                btnStorages.Visibility = Visibility.Collapsed;
                btnServices.Visibility = Visibility.Collapsed;
                btnRenderedServices.Visibility = Visibility.Collapsed;
                btnCrops.Visibility = Visibility.Collapsed;

                MainFrame.Navigate(new LabDashboardPage());
            }
            else if (role == "Менеджер")
            {
                btnEmployees.Visibility = Visibility.Collapsed;
                btnLabTests.Visibility = Visibility.Collapsed;
                btnCrops.Visibility = Visibility.Collapsed;
                btnStorages.Visibility = Visibility.Collapsed;
                btnBatches.Visibility = Visibility.Collapsed;

                MainFrame.Navigate(new ManagerDashboardPage());
            }
            else if (role == "Администратор")
            {
                MainFrame.Navigate(new AdminDashboardPage());
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentSession.Logout();
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}