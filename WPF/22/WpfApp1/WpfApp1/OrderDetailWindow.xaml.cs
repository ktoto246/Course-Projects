using System.Collections.Generic;
using System.Windows;

namespace WpfApp1
{
    public partial class OrderDetailWindow : Window
    {
        private OrderDetail _detail;

        public OrderDetailWindow(OrderDetail detail, List<Service> services)
        {
            InitializeComponent();
            _detail = detail;
            CmbServices.ItemsSource = services;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbServices.SelectedValue == null)
            {
                MessageBox.Show("Выберите услугу!", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Количество должно быть целым числом больше нуля!", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _detail.ServiceId = (int)CmbServices.SelectedValue;
            _detail.Quantity = quantity;

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}