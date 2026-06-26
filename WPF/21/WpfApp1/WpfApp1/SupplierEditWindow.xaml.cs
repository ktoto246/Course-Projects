using System.Windows;

namespace WpfApp1
{
    public partial class SupplierEditWindow : Window
    {
        public Supplier CurrentSupplier { get; private set; }

        public SupplierEditWindow(Supplier supplier)
        {
            InitializeComponent();
            CurrentSupplier = supplier;
            NameBox.Text = CurrentSupplier.Name;
            PhoneBox.Text = CurrentSupplier.Phone;
            AddressBox.Text = CurrentSupplier.Address;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Наименование поставщика не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentSupplier.Name = NameBox.Text;
            CurrentSupplier.Phone = string.IsNullOrWhiteSpace(PhoneBox.Text) ? null : PhoneBox.Text;
            CurrentSupplier.Address = string.IsNullOrWhiteSpace(AddressBox.Text) ? null : AddressBox.Text;

            DialogResult = true;
        }
    }
}