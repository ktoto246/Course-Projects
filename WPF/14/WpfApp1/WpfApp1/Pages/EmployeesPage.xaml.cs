using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class EmployeesPage : UserControl
    {
        private int selectedId = -1;

        public EmployeesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                EmployeesDataGrid.ItemsSource = context.Employees.ToList();
                StatusTextBlock.Text = "";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка: " + ex.Message;
                StatusTextBlock.Foreground = Brushes.Red;
            }
        }

        private void EmployeesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeesDataGrid.SelectedItem is Employee emp)
            {
                selectedId = emp.EmployeeID;
                NameTextBox.Text = emp.FullName;
                PosTextBox.Text = emp.Position ?? "";
            }
            else
            {
                selectedId = -1;
                NameTextBox.Text = "";
                PosTextBox.Text = "";
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) return;
            using var context = new BaltexEquipmentContext();
            context.Employees.Add(new Employee { FullName = NameTextBox.Text.Trim(), Position = PosTextBox.Text.Trim() });
            context.SaveChanges();
            LoadData();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedId == -1 || string.IsNullOrWhiteSpace(NameTextBox.Text)) return;
            using var context = new BaltexEquipmentContext();
            var emp = context.Employees.Find(selectedId);
            if (emp != null)
            {
                emp.FullName = NameTextBox.Text.Trim();
                emp.Position = PosTextBox.Text.Trim();
                context.SaveChanges();
                LoadData();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedId == -1) return;
            using var context = new BaltexEquipmentContext();
            var emp = context.Employees.Find(selectedId);
            if (emp != null)
            {
                context.Employees.Remove(emp);
                context.SaveChanges();
                LoadData();
            }
        }
    }
}