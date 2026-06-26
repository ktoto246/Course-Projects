using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class SuppliersPage : Page
    {
        private MagnitDbContext _db = new MagnitDbContext();

        public SuppliersPage()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            DgSuppliers.ItemsSource = _db.Suppliers.ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string company = TxtCompanyName.Text.Trim();
            string phone = TxtPhone.Text.Trim();
            string inn = TxtInn.Text.Trim();

            if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(inn))
            {
                MessageBox.Show("Ошибка: Все поля (Компания, Телефон, ИНН) должны быть заполнены!");
                return;
            }

            Supplier supplier = new Supplier
            {
                CompanyName = company,
                Phone = phone,
                Inn = inn
            };

            _db.Suppliers.Add(supplier);
            _db.SaveChanges();

            ClearInputs();
            RefreshData();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DgSuppliers.SelectedItem is not Supplier selectedSupplier)
            {
                MessageBox.Show("Ошибка: выберите поставщика из списка!");
                return;
            }

            string company = TxtCompanyName.Text.Trim();
            string phone = TxtPhone.Text.Trim();
            string inn = TxtInn.Text.Trim();

            if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(inn))
            {
                MessageBox.Show("Ошибка: Все поля должны быть заполнены!");
                return;
            }

            var supplier = _db.Suppliers.Find(selectedSupplier.Id);
            if (supplier != null)
            {
                supplier.CompanyName = company;
                supplier.Phone = phone;
                supplier.Inn = inn;
                _db.SaveChanges();
            }

            ClearInputs();
            RefreshData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgSuppliers.SelectedItem is not Supplier selectedSupplier)
            {
                MessageBox.Show("Ошибка: выберите поставщика из списка!");
                return;
            }

            var result = MessageBox.Show($"Удалить поставщика {selectedSupplier.CompanyName}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var supplier = _db.Suppliers.Find(selectedSupplier.Id);
                    if (supplier != null)
                    {
                        _db.Suppliers.Remove(supplier);
                        _db.SaveChanges();
                    }

                    ClearInputs();
                    RefreshData();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка: невозможно удалить поставщика, так как от него уже зарегистрированы поставки. Сначала удалите связанные записи в журнале.");
                }
            }
        }

        private void DgSuppliers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgSuppliers.SelectedItem is Supplier selectedSupplier)
            {
                TxtCompanyName.Text = selectedSupplier.CompanyName;
                TxtPhone.Text = selectedSupplier.Phone;
                TxtInn.Text = selectedSupplier.Inn;
            }
        }

        private void ClearInputs()
        {
            TxtCompanyName.Clear();
            TxtPhone.Clear();
            TxtInn.Clear();
        }
    }
}