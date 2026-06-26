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
            MainContent.Content = new EquipmentPage();
        }
        private void NavBtn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string pageName)
            {
                UserControl page = null;
                switch (pageName)
                {
                   case "ProductionUnitsPage": page = new ProductionUnitsPage(); break;
                   case "ManufacturersPage": page = new ManufacturersPage(); break;
                   case "EquipmentTypesPage": page = new EquipmentTypesPage(); break;
                   case "EquipmentStatusesPage": page = new EquipmentStatusesPage(); break;
                   case "EmployeesPage": page = new EmployeesPage(); break;
                   case "EquipmentPage": page = new EquipmentPage(); break;
                   case "MaintenancePage": page = new MaintenancePage(); break;
                   case "WriteOffPage": MainContent.Content = new WriteOffPage(); break;
                   case "ChartsPage": page = new ChartsPage(); break;
                }
                if (page != null) MainContent.Content = page;
            }
        }
    }
}