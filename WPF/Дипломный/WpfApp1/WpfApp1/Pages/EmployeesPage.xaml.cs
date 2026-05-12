using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;
using WpfApp1.Helpers;
using OfficeOpenXml;
using Microsoft.Win32;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Pages
{
    public partial class EmployeesPage : Page
    {
        private AppDbContext _db;

        public EmployeesPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            dgEmployees.ItemsSource = _db.Employees.ToList();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            dgEmployees.ItemsSource = _db.Employees
                .Where(emp => emp.FullName.ToLower().Contains(searchText) || emp.Login.ToLower().Contains(searchText))
                .ToList();
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            string fullName = txtFullName.Text?.Trim();
            string login = txtLogin.Text?.Trim();
            string password = txtPassword.Text?.Trim();
            string role = (cmbRole.SelectedItem as ComboBoxItem)?.Content?.ToString();

            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length < 3)
            {
                MessageBox.Show("ФИО должно содержать минимум 3 символа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(login) || login.Length < 3)
            {
                MessageBox.Show("Логин должен содержать минимум 3 символа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_db.Employees.Any(emp => emp.Login == login))
            {
                MessageBox.Show("Сотрудник с таким логином уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 5)
            {
                MessageBox.Show("Пароль должен содержать минимум 5 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Выберите роль сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newEmployee = new Employee
            {
                FullName = fullName,
                Login = login,
                Password = password,
                Role = role,
                IsActive = true
            };

            _db.Employees.Add(newEmployee);
            _db.SaveChanges();

            txtFullName.Clear();
            txtLogin.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = -1;
            txtSearch.Clear();

            LoadEmployees();
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            dgEmployees.CommitEdit(DataGridEditingUnit.Row, true);

            var modifiedEntries = _db.ChangeTracker.Entries<Employee>()
                .Where(x => x.State == EntityState.Modified)
                .ToList();

            var validRoles = new[] { "Администратор", "Весовщик", "Лаборант", "Менеджер" };

            foreach (var entry in modifiedEntries)
            {
                var emp = entry.Entity;

                if (string.IsNullOrWhiteSpace(emp.FullName) || emp.FullName.Length < 3)
                {
                    MessageBox.Show($"ФИО сотрудника (ID: {emp.Id}) слишком короткое. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadEmployees();
                    return;
                }

                if (string.IsNullOrWhiteSpace(emp.Login) || emp.Login.Length < 3)
                {
                    MessageBox.Show($"Логин сотрудника (ID: {emp.Id}) слишком короткий. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadEmployees();
                    return;
                }

                if (string.IsNullOrWhiteSpace(emp.Password) || emp.Password.Length < 5)
                {
                    MessageBox.Show($"Пароль сотрудника (ID: {emp.Id}) слишком короткий. Минимум 5 символов. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadEmployees();
                    return;
                }

                if (_db.Employees.Any(existing => existing.Login == emp.Login && existing.Id != emp.Id))
                {
                    MessageBox.Show($"Логин '{emp.Login}' уже занят. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadEmployees();
                    return;
                }

                if (!validRoles.Contains(emp.Role))
                {
                    MessageBox.Show($"Недопустимая роль '{emp.Role}' у сотрудника (ID: {emp.Id}). Разрешены только: Администратор, Весовщик, Лаборант, Менеджер. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadEmployees();
                    return;
                }

                if (emp.Id == CurrentSession.CurrentUser.Id && !emp.IsActive)
                {
                    MessageBox.Show("Нельзя заблокировать собственный аккаунт. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadEmployees();
                    return;
                }
            }

            _db.SaveChanges();
            _db.ChangeTracker.Clear();
            MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadEmployees();
        }

        private void btnDeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (dgEmployees.SelectedItem is Employee selectedEmployee)
            {
                if (selectedEmployee.Id == CurrentSession.CurrentUser.Id)
                {
                    MessageBox.Show("Нельзя удалить собственную учетную запись.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedEmployee.Role == "Администратор")
                {
                    int adminCount = _db.Employees.Count(emp => emp.Role == "Администратор" && emp.IsActive);
                    if (adminCount <= 1)
                    {
                        MessageBox.Show("Нельзя удалить единственного активного Администратора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                }

                int weighCount = _db.GrainBatches.Count(b => b.WeigherId == selectedEmployee.Id);
                int labCount = _db.LabTests.Count(t => t.LabTechId == selectedEmployee.Id);
                int serviceCount = _db.RenderedServices.Count(s => s.ManagerId == selectedEmployee.Id);

                if (weighCount > 0 || labCount > 0 || serviceCount > 0)
                {
                    StringBuilder sb = new StringBuilder("Нельзя удалить сотрудника. Найдены связанные записи:\n");
                    if (weighCount > 0) sb.AppendLine($"- Партии зерна (весовщик): {weighCount}");
                    if (labCount > 0) sb.AppendLine($"- Лабораторные анализы: {labCount}");
                    if (serviceCount > 0) sb.AppendLine($"- Оказанные услуги: {serviceCount}");
                    MessageBox.Show(sb.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить сотрудника '{selectedEmployee.FullName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _db.Employees.Remove(selectedEmployee);
                    _db.SaveChanges();
                    txtSearch.Clear();
                    LoadEmployees();
                }
            }
            else
            {
                MessageBox.Show("Выберите сотрудника в таблице для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Workbook|*.xlsx" };
            if (sfd.ShowDialog() == true)
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Сотрудники");

                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "ФИО";
                    worksheet.Cells[1, 3].Value = "Логин";
                    worksheet.Cells[1, 4].Value = "Роль";
                    worksheet.Cells[1, 5].Value = "Активен";

                    var employees = _db.Employees.ToList();
                    for (int i = 0; i < employees.Count; i++)
                    {
                        var emp = employees[i];
                        worksheet.Cells[i + 2, 1].Value = emp.Id;
                        worksheet.Cells[i + 2, 2].Value = emp.FullName;
                        worksheet.Cells[i + 2, 3].Value = emp.Login;
                        worksheet.Cells[i + 2, 4].Value = emp.Role;
                        worksheet.Cells[i + 2, 5].Value = emp.IsActive ? "Да" : "Нет";
                    }

                    File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                    MessageBox.Show("Данные выгружены в Excel", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}