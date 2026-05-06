using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Model;

namespace WpfApp1.Pages
{
    public partial class CustomersPage : Page
    {
        private RetailDbContext _context;
        private int _selectedId = 0;

        public CustomersPage()
        {
            InitializeComponent();
            _context = new RetailDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => LoadData();

        private void LoadData()
        {
            var query = _context.Customers.AsQueryable();
            string search = TxtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.FullName.ToLower().Contains(search) || (c.Phone != null && c.Phone.Contains(search)));
            }
            GridCustomers.ItemsSource = query.ToList();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                MessageBox.Show("ФИО не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(TxtDiscount.Text, out int discount) || discount < 0 || discount > 100)
            {
                MessageBox.Show("Скидка должна быть числом от 0 до 100!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            _context.Customers.Add(new Customer
            {
                FullName = TxtFullName.Text.Trim(),
                Phone = TxtPhone.Text.Trim(),
                DiscountLevel = int.Parse(TxtDiscount.Text)
            });
            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0 || !ValidateForm()) return;
            var customer = _context.Customers.Find(_selectedId);
            if (customer != null)
            {
                customer.FullName = TxtFullName.Text.Trim();
                customer.Phone = TxtPhone.Text.Trim();
                customer.DiscountLevel = int.Parse(TxtDiscount.Text);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0) return;
            if (MessageBox.Show("Удалить клиента?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var customer = _context.Customers.Find(_selectedId);
                if (customer != null)
                {
                    _context.Customers.Remove(customer);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
            }
        }

        private void GridCustomers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridCustomers.SelectedItem is Customer customer)
            {
                _selectedId = customer.Id;
                TxtFullName.Text = customer.FullName;
                TxtPhone.Text = customer.Phone;
                TxtDiscount.Text = customer.DiscountLevel.ToString();
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            _selectedId = 0;
            TxtFullName.Clear();
            TxtPhone.Clear();
            TxtDiscount.Text = "0";
            GridCustomers.SelectedItem = null;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
    }
}