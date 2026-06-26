using System.Windows;
using WpfApp2.Pages;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new DepartmentsPage());
        }

        private void NavОтделы_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new DepartmentsPage());
        private void NavРабочиеМеста_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new WorkplacesPage());
        private void NavОборудование_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new HardwarePage());
        private void NavПО_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new SoftwarePage());
        private void NavУстановки_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new InstallationsPage());

        private void NavСтатистика_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new DashboardPage());
    }
}