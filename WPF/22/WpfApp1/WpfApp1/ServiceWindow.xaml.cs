using System.Windows;

namespace WpfApp1
{
    public partial class ServiceWindow : Window
    {
        private Service _service;

        public ServiceWindow(Service service)
        {
            InitializeComponent();
            _service = service;

            TxtServiceName.Text = _service.ServiceName;
            if (_service.ServiceId != 0)
            {
                TxtBasePrice.Text = _service.BasePrice.ToString("0.00");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtServiceName.Text) || string.IsNullOrWhiteSpace(TxtBasePrice.Text))
            {
                MessageBox.Show("Все поля обязательны для заполнения!", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtBasePrice.Text.Replace(".", ","), out decimal parsedPrice))
            {
                MessageBox.Show("В поле цены нужно вводить только цифры!", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _service.ServiceName = TxtServiceName.Text;
            _service.BasePrice = parsedPrice;

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}