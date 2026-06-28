using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для EmployeesPage.xaml
    /// </summary>
    public partial class EmployeesPage : Page
    {
        private Employee? _selectedEmployee;
        public EmployeesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            using (var db = new BmtcContext())
            {
                EmployeesGrid.ItemsSource = db.Employees.ToList();  
            }
        }

        private void EmployeesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Employee employee)
            {
                _selectedEmployee = employee;
                TxtFullName.Text = employee.FullName;

                foreach (ComboBoxItem item in CmbRole.Items) 
                {
                    if (item.Content.ToString() == employee.Role)
                    {
                        CmbRole.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text) || CmbRole.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (var db = new BmtcContext())
            {
                var newEmployee = new Employee
                {
                    FullName = TxtFullName.Text,
                    Role = (CmbRole.SelectedItem as ComboBoxItem)?.Content.ToString()
                };
                db.Employees.Add(newEmployee);
                db.SaveChanges();
            }
            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для редактирования.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new BmtcContext())
            {
                var empToUpdate = db.Employees.FirstOrDefault(x => x.Id == _selectedEmployee.Id);
                if (empToUpdate != null)
                {
                    empToUpdate.FullName = TxtFullName.Text;
                    empToUpdate.Role = (CmbRole.SelectedItem as ComboBoxItem)?.Content.ToString();
                    db.SaveChanges();
                }
            }
            LoadData();
            ClearForm();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedEmployee == null)
            {
                MessageBox.Show("Пожалуйста, выберите сотрудника для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(MessageBox.Show($"Вы уверены, что хотите удалить сотрудника {_selectedEmployee.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new BmtcContext())
                {
                    var empToDelete = db.Employees.FirstOrDefault(x => x.Id == _selectedEmployee.Id);
                    if (empToDelete != null)
                    {
                        db.Employees.Remove(empToDelete);
                        db.SaveChanges();
                    }
                }
                LoadData();
                ClearForm();
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedEmployee = null;
            TxtFullName.Clear();
            CmbRole.SelectedItem = 0;
            EmployeesGrid.SelectedItem = null;
        }
    }
}
