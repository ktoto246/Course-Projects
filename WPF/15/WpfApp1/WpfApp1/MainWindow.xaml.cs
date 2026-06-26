using System.Windows;
using System.Windows.Controls;
using WpfApp1.Pages;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new WeldersPage());
        }

        private void OnNavigationClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            switch (button.Content.ToString())
            {
                case "Сварщики":
                    MainFrame.Navigate(new WeldersPage());
                    break;
                case "Трубопроводы":
                    MainFrame.Navigate(new PipelinesPage());
                    break;
                case "Сварочные аппараты":
                    MainFrame.Navigate(new WeldingMachinesPage());
                    break;
                case "Сварные соединения":
                    MainFrame.Navigate(new JointsPage());
                    break;
                case "Контроль":
                    MainFrame.Navigate(new InspectionsPage());
                    break;
            }
        }

        private void AnalyticsClick(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AnalyticsPage());
        }
    }
}
