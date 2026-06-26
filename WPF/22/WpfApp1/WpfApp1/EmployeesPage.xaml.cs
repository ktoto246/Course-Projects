using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class EmployeesPage : Page
    {
        private AlphabetContext _context;

        public EmployeesPage()
        {
            InitializeComponent();
            _context = new AlphabetContext();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                searchText = searchText.ToLower();
                query = query.Where(e =>
                    e.FullName.ToLower().Contains(searchText) ||
                    e.Position.ToLower().Contains(searchText));
            }

            EmployeesGrid.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text);
        }

        public void AddEmployee()
        {
            var newEmployee = new Employee();
            var window = new EmployeeWindow(newEmployee);

            if (window.ShowDialog() == true)
            {
                _context.Employees.Add(newEmployee);
                _context.SaveChanges();
                LoadData(TxtSearch.Text);
            }
        }

        public void EditEmployee()
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                var window = new EmployeeWindow(selectedEmployee);
                if (window.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    LoadData(TxtSearch.Text);
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника в таблице для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void DeleteEmployee()
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                var result = MessageBox.Show($"Вы действительно хотите удалить сотрудника {selectedEmployee.FullName}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Employees.Remove(selectedEmployee);
                        _context.SaveChanges();
                        LoadData(TxtSearch.Text);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Не удалось удалить сотрудника. Возможно, на нем висят заказы.\nОшибка: {ex.Message}", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}