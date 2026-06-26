using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Models;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Linq;

namespace WpfApp1.Pages
{
    public partial class EmployeesPage : Page
    {
        private AppDbContext _context;
        private ICollectionView _employeesView;

        public EmployeesPage()
        {
            InitializeComponent();
            if (AppSession.CurrentRole == "Viewer" || AppSession.CurrentRole == "Operator")
            {
                AddPanel.Visibility = Visibility.Collapsed;
                BtnDelete.Visibility = Visibility.Collapsed;
                BtnSave.Visibility = Visibility.Collapsed;
                EmployeesGrid.IsReadOnly = true;
            }
            try
            {
                _context = new AppDbContext();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Unloaded += (s, e) => _context?.Dispose();
        }

        private void LoadData()
        {
            var positions = _context.Positions.ToList();
            var departments = _context.Departments.ToList();

            cmbPosition.ItemsSource = positions;
            cmbDepartment.ItemsSource = departments;

            colPosition.ItemsSource = positions;
            colDepartment.ItemsSource = departments;

            _context.Employees
                .Include(e => e.Position)
                .Include(e => e.Department)
                .Load();

            _employeesView = CollectionViewSource.GetDefaultView(_context.Employees.Local.ToObservableCollection());
            _employeesView.Filter = FilterEmployees;
            EmployeesGrid.ItemsSource = _employeesView;
        }

        private bool FilterEmployees(object item)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return true;

            if (item is Employee e)
            {
                string search = txtSearch.Text.ToLower();
                return e.FullName.ToLower().Contains(search) ||
                       (e.Position != null && e.Position.Name.ToLower().Contains(search)) ||
                       (e.Department != null && e.Department.Name.ToLower().Contains(search));
            }
            return false;
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            _employeesView?.Refresh();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("ФИО сотрудника обязательно для заполнения.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newEmployee = new Employee
            {
                FullName = txtFullName.Text.Trim(),
                PositionId = cmbPosition.SelectedValue != null ? (int?)cmbPosition.SelectedValue : null,
                DepartmentId = cmbDepartment.SelectedValue != null ? (int?)cmbDepartment.SelectedValue : null
            };

            try
            {
                _context.Employees.Add(newEmployee);
                _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            txtFullName.Clear();
            cmbPosition.SelectedItem = null;
            cmbDepartment.SelectedItem = null;

            EmployeesGrid.Items.Refresh();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee selectedEmployee)
            {
                int equipmentCount = _context.Equipments.Count(eq => eq.EmployeeId == selectedEmployee.Id);

                bool hasHistory = _context.MovementHistories.Any(m =>
                    m.FromEmployeeId == selectedEmployee.Id || m.ToEmployeeId == selectedEmployee.Id);

                if (equipmentCount > 0)
                {
                    MessageBox.Show($"За сотрудником числится {equipmentCount} ед. техники. Сначала переведите её на другого сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (hasHistory)
                {
                    MessageBox.Show("Данный сотрудник участвовал в перемещениях оборудования и числится в истории. Его нельзя удалить для сохранения целостности архива.", "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (MessageBox.Show($"Удалить '{selectedEmployee.FullName}'?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Employees.Remove(selectedEmployee);
                        _context.SaveChanges();
                        MessageBox.Show("Сотрудник успешно удалён.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении сотрудника:\n{ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Сохранить отчет",
                    FileName = $"Сотрудники_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Сотрудники");

                        worksheet.Cell(1, 1).Value = "Код";
                        worksheet.Cell(1, 2).Value = "ФИО";
                        worksheet.Cell(1, 3).Value = "Должность";
                        worksheet.Cell(1, 4).Value = "Отдел";

                        var headerRow = worksheet.Row(1);
                        headerRow.Style.Font.Bold = true;
                        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                        int row = 2;
                        foreach (Employee emp in _employeesView)
                        {
                            worksheet.Cell(row, 1).Value = emp.Id;
                            worksheet.Cell(row, 2).Value = emp.FullName;
                            worksheet.Cell(row, 3).Value = emp.Position?.Name;
                            worksheet.Cell(row, 4).Value = emp.Department?.Name;
                            row++;
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveFileDialog.FileName);
                    }
                    MessageBox.Show("Отчет успешно сохранен в Excel!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}