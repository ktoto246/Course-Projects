using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class DeliveriesPage : Page
    {
        private MagnitDbContext _db = new MagnitDbContext();

        public DeliveriesPage()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            CmbProducts.ItemsSource = _db.Products.ToList();
            CmbSuppliers.ItemsSource = _db.Suppliers.ToList();
            CmbEmployees.ItemsSource = _db.Employees.ToList();

            DgDeliveries.ItemsSource = _db.Deliveries
                .Include(d => d.Product)
                .Include(d => d.Supplier)
                .Include(d => d.Employee)
                .ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (DpDeliveryDate.SelectedDate == null ||
                string.IsNullOrWhiteSpace(TxtQuantity.Text) ||
                CmbProducts.SelectedValue == null ||
                CmbSuppliers.SelectedValue == null ||
                CmbEmployees.SelectedValue == null)
            {
                MessageBox.Show("Ошибка: заполните все поля и выберите значения из списков!");
                return;
            }

            if (!int.TryParse(TxtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Ошибка: количество должно быть целым положительным числом!");
                return;
            }

            Delivery delivery = new Delivery
            {
                DeliveryDate = DpDeliveryDate.SelectedDate.Value,
                Quantity = quantity,
                ProductId = (int)CmbProducts.SelectedValue,
                SupplierId = (int)CmbSuppliers.SelectedValue,
                EmployeeId = (int)CmbEmployees.SelectedValue
            };

            _db.Deliveries.Add(delivery);
            _db.SaveChanges();

            ClearInputs();
            RefreshData();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DgDeliveries.SelectedItem is not Delivery selectedDelivery)
            {
                MessageBox.Show("Ошибка: выберите запись о приемке из списка!");
                return;
            }

            if (DpDeliveryDate.SelectedDate == null ||
                string.IsNullOrWhiteSpace(TxtQuantity.Text) ||
                CmbProducts.SelectedValue == null ||
                CmbSuppliers.SelectedValue == null ||
                CmbEmployees.SelectedValue == null)
            {
                MessageBox.Show("Ошибка: заполните все поля!");
                return;
            }

            if (!int.TryParse(TxtQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Ошибка: количество должно быть целым положительным числом!");
                return;
            }

            var delivery = _db.Deliveries.Find(selectedDelivery.Id);
            if (delivery != null)
            {
                delivery.DeliveryDate = DpDeliveryDate.SelectedDate.Value;
                delivery.Quantity = quantity;
                delivery.ProductId = (int)CmbProducts.SelectedValue;
                delivery.SupplierId = (int)CmbSuppliers.SelectedValue;
                delivery.EmployeeId = (int)CmbEmployees.SelectedValue;

                _db.SaveChanges();
            }

            ClearInputs();
            RefreshData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgDeliveries.SelectedItem is not Delivery selectedDelivery)
            {
                MessageBox.Show("Ошибка: выберите запись о приемке из списка!");
                return;
            }

            var result = MessageBox.Show("Удалить выбранную запись о приемке товара?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var delivery = _db.Deliveries.Find(selectedDelivery.Id);
                if (delivery != null)
                {
                    _db.Deliveries.Remove(delivery);
                    _db.SaveChanges();
                }

                ClearInputs();
                RefreshData();
            }
        }

        private void DgDeliveries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgDeliveries.SelectedItem is Delivery selectedDelivery)
            {
                DpDeliveryDate.SelectedDate = selectedDelivery.DeliveryDate;
                TxtQuantity.Text = selectedDelivery.Quantity.ToString();
                CmbProducts.SelectedValue = selectedDelivery.ProductId;
                CmbSuppliers.SelectedValue = selectedDelivery.SupplierId;
                CmbEmployees.SelectedValue = selectedDelivery.EmployeeId;
            }
        }

        private void ClearInputs()
        {
            DpDeliveryDate.SelectedDate = null;
            TxtQuantity.Clear();
            CmbProducts.SelectedItem = null;
            CmbSuppliers.SelectedItem = null;
            CmbEmployees.SelectedItem = null;
        }
    }
}