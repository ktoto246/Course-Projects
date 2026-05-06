using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class EmployeesPage : Page
    {
        private AppDbContext _context;

        public EmployeesPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(e => e.FullName.Contains(searchText) ||
                                         e.Position.Contains(searchText) ||
                                         e.Department.Contains(searchText));
            }

            DataGridEmployees.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text.Trim());
        }

        private void DataGridEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridEmployees.SelectedItem is Employee selectedEmployee)
            {
                TxtFullName.Text = selectedEmployee.FullName;
                TxtPosition.Text = selectedEmployee.Position;
                TxtDepartment.Text = selectedEmployee.Department;
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text) ||
                string.IsNullOrWhiteSpace(TxtPosition.Text) ||
                string.IsNullOrWhiteSpace(TxtDepartment.Text))
            {
                MessageBox.Show("Заполните все текстовые поля.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            var newEmployee = new Employee
            {
                FullName = TxtFullName.Text.Trim(),
                Position = TxtPosition.Text.Trim(),
                Department = TxtDepartment.Text.Trim()
            };

            _context.Employees.Add(newEmployee);
            _context.SaveChanges();

            ClearFields();
            LoadData(TxtSearch.Text.Trim());
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridEmployees.SelectedItem is Employee selectedEmployee)
            {
                if (!ValidateInputs()) return;

                selectedEmployee.FullName = TxtFullName.Text.Trim();
                selectedEmployee.Position = TxtPosition.Text.Trim();
                selectedEmployee.Department = TxtDepartment.Text.Trim();

                _context.SaveChanges();

                ClearFields();
                LoadData(TxtSearch.Text.Trim());
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridEmployees.SelectedItem is Employee selectedEmployee)
            {
                if (MessageBox.Show($"Удалить сотрудника '{selectedEmployee.FullName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Employees.Remove(selectedEmployee);
                        _context.SaveChanges();
                        ClearFields();
                        LoadData(TxtSearch.Text.Trim());
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                    {
                        MessageBox.Show("Невозможно удалить сотрудника, так как за ним закреплены сделки. Сначала удалите связанные сделки.",
                                        "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);

                        _context.Entry(selectedEmployee).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            TxtFullName.Clear();
            TxtPosition.Clear();
            TxtDepartment.Clear();
            DataGridEmployees.SelectedItem = null;
        }
    }
}