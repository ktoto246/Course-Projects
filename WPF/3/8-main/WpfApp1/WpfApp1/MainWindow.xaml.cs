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
        }
        private void BtnTech_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TechPage());
        }

        private void BtnComp_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ComponentsPage());
        }

        private void BtnStaff_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StaffPage());
        }

        private void BtnTypes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new TypesPage());
        }

        private void BtnAnalytics_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }
    }
}