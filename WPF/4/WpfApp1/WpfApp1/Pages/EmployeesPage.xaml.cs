using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Model;

namespace WpfApp1.Pages
{
    public partial class EmployeesPage : Page
    {
        private RetailDbContext _context;
        private int _selectedId = 0;

        public EmployeesPage()
        {
            InitializeComponent();
            _context = new RetailDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => LoadData();

        private void LoadData()
        {
            var query = _context.Employees.AsQueryable();
            string search = TxtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(emp => emp.FullName.ToLower().Contains(search));
            }
            GridEmployees.ItemsSource = query.ToList();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text))
            {
                MessageBox.Show("ФИО не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            _context.Employees.Add(new Employee
            {
                FullName = TxtFullName.Text.Trim(),
                Position = TxtPosition.Text.Trim()
            });
            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0 || !ValidateForm()) return;
            var emp = _context.Employees.Find(_selectedId);
            if (emp != null)
            {
                emp.FullName = TxtFullName.Text.Trim();
                emp.Position = TxtPosition.Text.Trim();
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0) return;
            if (MessageBox.Show("Уволить сотрудника?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var emp = _context.Employees.Find(_selectedId);
                if (emp != null)
                {
                    _context.Employees.Remove(emp);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
            }
        }

        private void GridEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridEmployees.SelectedItem is Employee emp)
            {
                _selectedId = emp.Id;
                TxtFullName.Text = emp.FullName;
                TxtPosition.Text = emp.Position;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            _selectedId = 0;
            TxtFullName.Clear();
            TxtPosition.Clear();
            GridEmployees.SelectedItem = null;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
    }
}