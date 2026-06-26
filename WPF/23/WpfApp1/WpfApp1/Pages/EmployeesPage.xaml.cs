using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class EmployeesPage : Page
    {
        private AppDbContext _context;

        public EmployeesPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            EmployeesGrid.ItemsSource = _context.Employees.ToList();
        }

        private void EmployeesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                txtLastName.Text = selectedEmployee.LastName;
                txtFirstName.Text = selectedEmployee.FirstName;
                txtMiddleName.Text = selectedEmployee.MiddleName;
                txtPosition.Text = selectedEmployee.Position;
                dpHireDate.SelectedDate = selectedEmployee.HireDate;
                txtSalary.Text = selectedEmployee.Salary.ToString();
                txtPhone.Text = selectedEmployee.Phone;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var newEmployee = new Employee
            {
                LastName = txtLastName.Text,
                FirstName = txtFirstName.Text,
                MiddleName = txtMiddleName.Text,
                Position = txtPosition.Text,
                HireDate = dpHireDate.SelectedDate ?? DateTime.Now,
                Salary = decimal.Parse(txtSalary.Text),
                Phone = txtPhone.Text
            };

            try
            {
                _context.Employees.Add(newEmployee);
                _context.SaveChanges();
                MessageBox.Show("Сотрудник успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                string realError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                MessageBox.Show($"Ошибка базы данных:\n{realError}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                if (!ValidateInput()) return;

                selectedEmployee.LastName = txtLastName.Text;
                selectedEmployee.FirstName = txtFirstName.Text;
                selectedEmployee.MiddleName = txtMiddleName.Text;
                selectedEmployee.Position = txtPosition.Text;
                selectedEmployee.HireDate = dpHireDate.SelectedDate ?? DateTime.Now;
                selectedEmployee.Salary = decimal.Parse(txtSalary.Text);
                selectedEmployee.Phone = txtPhone.Text;

                try
                {
                    _context.SaveChanges();
                    MessageBox.Show("Данные сотрудника обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    EmployeesGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника из списка для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                var result = MessageBox.Show($"Вы точно хотите удалить {selectedEmployee.LastName} {selectedEmployee.FirstName}?",
                                             "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Employees.Remove(selectedEmployee);
                        _context.SaveChanges();
                        ClearForm();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении (возможно есть связанные записи): {ex.Message}",
                                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            EmployeesGrid.SelectedItem = null;
            txtLastName.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtPosition.Clear();
            dpHireDate.SelectedDate = null;
            txtSalary.Clear();
            txtPhone.Clear();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtPosition.Text) ||
                dpHireDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните все обязательные поля (*).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtSalary.Text, out _))
            {
                MessageBox.Show("В поле 'Оклад' должно быть число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}