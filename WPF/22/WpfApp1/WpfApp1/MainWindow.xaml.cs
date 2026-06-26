using System.Windows;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BtnClients.Click += BtnClients_Click;
            BtnEmployees.Click += BtnEmployees_Click;
            BtnServices.Click += BtnServices_Click;
            BtnOrders.Click += BtnOrders_Click;

            BtnAdd.Click += BtnAdd_Click;
            BtnEdit.Click += BtnEdit_Click;
            BtnDelete.Click += BtnDelete_Click;
            BtnStats.Click += BtnStats_Click;
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientsPage());
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new EmployeesPage());
        }

        private void BtnServices_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ServicesPage());
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());
        }

        private void BtnStats_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StatsPage());
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ClientsPage clientsPage) clientsPage.AddClient();
            else if (MainFrame.Content is EmployeesPage empPage) empPage.AddEmployee();
            else if (MainFrame.Content is ServicesPage servPage) servPage.AddService();
            else if (MainFrame.Content is OrdersPage ordPage) ordPage.AddOrder();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ClientsPage clientsPage) clientsPage.EditClient();
            else if (MainFrame.Content is EmployeesPage empPage) empPage.EditEmployee();
            else if (MainFrame.Content is ServicesPage servPage) servPage.EditService();
            else if (MainFrame.Content is OrdersPage ordPage) ordPage.EditOrder();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is ClientsPage clientsPage) clientsPage.DeleteClient();
            else if (MainFrame.Content is EmployeesPage empPage) empPage.DeleteEmployee();
            else if (MainFrame.Content is ServicesPage servPage) servPage.DeleteService();
            else if (MainFrame.Content is OrdersPage ordPage) ordPage.DeleteOrder();
        }
    }
}