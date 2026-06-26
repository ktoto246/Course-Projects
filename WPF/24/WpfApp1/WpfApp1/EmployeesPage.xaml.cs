using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class EmployeesPage : Page
    {
        private MagnitDbContext _db = new MagnitDbContext();

        public EmployeesPage()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            DgEmployees.ItemsSource = _db.Employees.ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string fullName = TxtFullName.Text.Trim();
            string position = TxtPosition.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(position))
            {
                MessageBox.Show("Ошибка: ФИО и должность должны быть заполнены!");
                return;
            }

            Employee employee = new Employee
            {
                FullName = fullName,
                Position = position
            };

            _db.Employees.Add(employee);
            _db.SaveChanges();

            ClearInputs();
            RefreshData();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DgEmployees.SelectedItem is not Employee selectedEmployee)
            {
                MessageBox.Show("Ошибка: выберите сотрудника из списка!");
                return;
            }

            string fullName = TxtFullName.Text.Trim();
            string position = TxtPosition.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(position))
            {
                MessageBox.Show("Ошибка: ФИО и должность должны быть заполнены!");
                return;
            }

            var employee = _db.Employees.Find(selectedEmployee.Id);
            if (employee != null)
            {
                employee.FullName = fullName;
                employee.Position = position;
                _db.SaveChanges();
            }

            ClearInputs();
            RefreshData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgEmployees.SelectedItem is not Employee selectedEmployee)
            {
                MessageBox.Show("Ошибка: выберите сотрудника из списка!");
                return;
            }

            var result = MessageBox.Show($"Удалить сотрудника {selectedEmployee.FullName}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var employee = _db.Employees.Find(selectedEmployee.Id);
                    if (employee != null)
                    {
                        _db.Employees.Remove(employee);
                        _db.SaveChanges();
                    }

                    ClearInputs();
                    RefreshData();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка: невозможно удалить сотрудника, так как он уже числится в журнале приемки. Сначала удалите связанные записи.");
                }
            }
        }

        private void DgEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgEmployees.SelectedItem is Employee selectedEmployee)
            {
                TxtFullName.Text = selectedEmployee.FullName;
                TxtPosition.Text = selectedEmployee.Position;
            }
        }

        private void ClearInputs()
        {
            TxtFullName.Clear();
            TxtPosition.Clear();
        }
    }
}