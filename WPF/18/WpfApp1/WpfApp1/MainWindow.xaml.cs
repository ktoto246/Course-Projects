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
        }

        private void OnNavButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            string content = button.Content.ToString();

            switch (content)
            {
                case "Мельницы":
                    MainContentFrame.Navigate(new MillPage());
                    break;
                case "Продукты":
                    MainContentFrame.Navigate(new ProductPage());
                    break;
                case "Клиенты":
                    MainContentFrame.Navigate(new ClientPage());
                    break;
                case "Заказы":
                    MainContentFrame.Navigate(new OrderPage());
                    break;
                case "Детали заказов":
                    MainContentFrame.Navigate(new OrderDetailPage());
                    break;
                case "Графики":
                    MainContentFrame.Navigate(new DashboardPage());
                    break;
                default:
                    MainContentFrame.Navigate(null);
                    break;
            }
        }
    }
}
