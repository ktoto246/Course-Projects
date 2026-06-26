using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class OrderWindow : Window
    {
        private Order _order;

        public OrderWindow(Order order, List<Client> clients, List<Employee> employees)
        {
            InitializeComponent();
            _order = order;

            CmbClients.ItemsSource = clients;
            CmbEmployees.ItemsSource = employees;

            CmbClients.SelectedValue = _order.ClientId;
            CmbEmployees.SelectedValue = _order.EmployeeId;
            CmbStatus.Text = _order.Status;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbClients.SelectedValue == null || CmbEmployees.SelectedValue == null || string.IsNullOrEmpty(CmbStatus.Text))
            {
                MessageBox.Show("Заполните все поля!", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _order.ClientId = (int)CmbClients.SelectedValue;
            _order.EmployeeId = (int)CmbEmployees.SelectedValue;
            _order.Status = CmbStatus.Text;

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}