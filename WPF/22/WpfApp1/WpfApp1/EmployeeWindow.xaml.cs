using System.Windows;

namespace WpfApp1
{
    public partial class EmployeeWindow : Window
    {
        private Employee _employee;

        public EmployeeWindow(Employee employee)
        {
            InitializeComponent();
            _employee = employee;

            TxtFullName.Text = _employee.FullName;
            TxtPosition.Text = _employee.Position;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text) || string.IsNullOrWhiteSpace(TxtPosition.Text))
            {
                MessageBox.Show("Все поля обязательны для заполнения!", "Валидация", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _employee.FullName = TxtFullName.Text;
            _employee.Position = TxtPosition.Text;

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}