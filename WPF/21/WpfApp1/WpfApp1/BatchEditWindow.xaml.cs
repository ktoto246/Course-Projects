using System;
using System.Linq;
using System.Windows;

namespace WpfApp1
{
    public partial class BatchEditWindow : Window
    {
        public GrainBatch CurrentBatch { get; private set; }

        public BatchEditWindow(GrainBatch batch, BalashovDbContext context)
        {
            InitializeComponent();
            CurrentBatch = batch;

            SupplierComboBox.ItemsSource = context.Suppliers.ToList();

            if (CurrentBatch.SupplierId != 0)
                SupplierComboBox.SelectedValue = CurrentBatch.SupplierId;

            DateBox.Text = CurrentBatch.DeliveryDate.ToString("yyyy-MM-dd HH:mm:ss");
            VehicleBox.Text = CurrentBatch.VehicleNumber;
            WeightBox.Text = CurrentBatch.WeightTons == 0 ? "" : CurrentBatch.WeightTons.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите поставщика!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!DateTime.TryParse(DateBox.Text, out DateTime parsedDate))
            {
                MessageBox.Show("Неверный формат даты!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(WeightBox.Text.Replace(".", ","), out decimal parsedWeight))
            {
                MessageBox.Show("Неверный формат веса!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentBatch.SupplierId = (int)SupplierComboBox.SelectedValue;
            CurrentBatch.DeliveryDate = parsedDate;
            CurrentBatch.VehicleNumber = VehicleBox.Text;
            CurrentBatch.WeightTons = parsedWeight;

            DialogResult = true;
        }
    }
}