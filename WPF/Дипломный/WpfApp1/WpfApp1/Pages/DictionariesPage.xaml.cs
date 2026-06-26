using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class DictionariesPage : Page
    {
        private AppDbContext _context;

        public DictionariesPage()
        {
            InitializeComponent();

            if (AppSession.CurrentRole == "Viewer" || AppSession.CurrentRole == "Operator")
            {
                BtnSaveAll.Visibility = Visibility.Collapsed;
                txtPositionName.IsEnabled = false;
                txtDepartmentName.IsEnabled = false;
                txtCabinetNumber.IsEnabled = false;
                cmbCabinetEmployee.IsEnabled = false;
                PositionsGrid.IsReadOnly = true;
                DepartmentsGrid.IsReadOnly = true;
                CabinetsGrid.IsReadOnly = true;
            }

            this.Loaded += (s, e) =>
            {
                try
                {
                    _context?.Dispose();
                    _context = new AppDbContext();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка инициализации БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            this.Unloaded += (s, e) => _context?.Dispose();
        }

        private void LoadData()
        {
            var employees = _context.Employees.ToList();
            cmbCabinetEmployee.ItemsSource = employees;
            colCabinetEmp.ItemsSource = employees;

            _context.Positions.Load();
            _context.Departments.Load();
            _context.Cabinets.Include(c => c.ResponsibleEmployee).Load();

            PositionsGrid.ItemsSource = _context.Positions.Local.ToObservableCollection();
            DepartmentsGrid.ItemsSource = _context.Departments.Local.ToObservableCollection();
            CabinetsGrid.ItemsSource = _context.Cabinets.Local.ToObservableCollection();
        }

        private void BtnAddPosition_Click(object sender, RoutedEventArgs e)
        {
            string name = txtPositionName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) return;

            if (_context.Positions.Any(p => p.Name == name))
            {
                MessageBox.Show("Такая должность уже есть в справочнике.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _context.Positions.Add(new Position { Name = name });
            txtPositionName.Clear();
        }

        private void BtnDeletePosition_Click(object sender, RoutedEventArgs e)
        {
            if (PositionsGrid.SelectedItem is Position pos)
            {
                if (_context.Employees.Any(emp => emp.PositionId == pos.Id))
                {
                    MessageBox.Show("Нельзя удалить должность, так как она присвоена действующим сотрудникам!", "Ошибка целостности", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (MessageBox.Show($"Удалить должность '{pos.Name}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Positions.Remove(pos);
                }
            }
        }

        private void BtnAddDepartment_Click(object sender, RoutedEventArgs e)
        {
            string name = txtDepartmentName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name)) return;

            if (_context.Departments.Any(d => d.Name == name))
            {
                MessageBox.Show("Такой отдел уже существует.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _context.Departments.Add(new Department { Name = name });
            txtDepartmentName.Clear();
        }

        private void BtnDeleteDepartment_Click(object sender, RoutedEventArgs e)
        {
            if (DepartmentsGrid.SelectedItem is Department dept)
            {
                if (_context.Employees.Any(emp => emp.DepartmentId == dept.Id))
                {
                    MessageBox.Show("Нельзя удалить отдел, пока в нем числятся сотрудники!", "Ошибка целостности", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (MessageBox.Show($"Удалить отдел '{dept.Name}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Departments.Remove(dept);
                }
            }
        }

        private void BtnAddCabinet_Click(object sender, RoutedEventArgs e)
        {
            string num = txtCabinetNumber.Text.Trim();
            if (string.IsNullOrWhiteSpace(num)) return;

            if (_context.Cabinets.Any(c => c.Number == num))
            {
                MessageBox.Show("Кабинет с таким номером уже заведен.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? empId = cmbCabinetEmployee.SelectedValue != null ? (int)cmbCabinetEmployee.SelectedValue : null;

            _context.Cabinets.Add(new Cabinet { Number = num, ResponsibleEmployeeId = empId });

            txtCabinetNumber.Clear();
            cmbCabinetEmployee.SelectedItem = null;
        }

        private void BtnDeleteCabinet_Click(object sender, RoutedEventArgs e)
        {
            if (CabinetsGrid.SelectedItem is Cabinet cab)
            {
                if (_context.Equipments.Any(eq => eq.CabinetId == cab.Id && eq.Status.Name != SystemStatuses.Scrapped))
                {
                    MessageBox.Show("Нельзя удалить кабинет, так как в нем сейчас находится активное оборудование!", "Ошибка целостности", MessageBoxButton.OK, MessageBoxImage.Stop);
                    return;
                }

                if (MessageBox.Show($"Удалить кабинет '{cab.Number}'?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _context.Cabinets.Remove(cab);
                }
            }
        }

        private void BtnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Все справочники успешно синхронизированы с БД!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения изменений:\n{ex.Message}", "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}