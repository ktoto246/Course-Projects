using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ElanCheeseApp.Models;

namespace ElanCheeseApp.Pages
{
    public partial class EmployeesPage : Page
    {
        private ElanCheeseDbContext _context;
        private ObservableCollection<Employee> _employees;
        private Employee? _selectedEmployee;

        public EmployeesPage()
        {
            InitializeComponent();
            _context = new ElanCheeseDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _employees = new ObservableCollection<Employee>(_context.Employees.ToList());
            EmployeesDataGrid.ItemsSource = _employees;
        }

        private void ClearForm()
        {
            FullNameTextBox.Text = string.Empty;
            PositionTextBox.Text = string.Empty;
            _selectedEmployee = null;
            EmployeesDataGrid.SelectedItem = null;
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) || string.IsNullOrWhiteSpace(PositionTextBox.Text))
            {
                MessageBox.Show("Заполни все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newEmployee = new Employee
            {
                FullName = FullNameTextBox.Text,
                Position = PositionTextBox.Text
            };

            _context.Employees.Add(newEmployee);
            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                MessageBox.Show("Сначала выбери сотрудника в таблице для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) || string.IsNullOrWhiteSpace(PositionTextBox.Text)) return;

            _selectedEmployee.FullName = FullNameTextBox.Text;
            _selectedEmployee.Position = PositionTextBox.Text;

            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null) return;

            if (MessageBox.Show($"Точно удалить сотрудника {_selectedEmployee.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.Employees.Remove(_selectedEmployee);
                _context.SaveChanges();

                LoadData();
                ClearForm();
            }
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void OnEmployeesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is Employee employee)
            {
                _selectedEmployee = employee;
                FullNameTextBox.Text = employee.FullName;
                PositionTextBox.Text = employee.Position;
            }
        }
    }
}