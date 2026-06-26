using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class IssuanceLogPage : Page
    {
        private BalashovDbContext _context;

        public IssuanceLogPage()
        {
            InitializeComponent();
            _context = new BalashovDbContext();
            LoadComboBoxes();
            LoadData();
        }

        private void LoadComboBoxes()
        {
            if (_context == null) return;
            CmbResident.ItemsSource = _context.Residents.ToList();
            CmbItem.ItemsSource = _context.Items.ToList();
            CmbEmployee.ItemsSource = _context.Employees.ToList();
            DpIssueDate.SelectedDate = DateTime.Now;
        }

        private void LoadData()
        {
            if (_context == null) return;

            var query = _context.IssuanceLogs
                .Include(l => l.Resident)
                .Include(l => l.Item)
                .Include(l => l.Employee)
                .AsQueryable();

            string searchText = TxtSearch.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(l =>
                    l.Resident.FullName.ToLower().Contains(searchText) ||
                    l.Item.ItemName.ToLower().Contains(searchText));
            }

            if (CmbSort.SelectedItem is ComboBoxItem selectedSort)
            {
                switch (selectedSort.Content.ToString())
                {
                    case "Сначала новые выдачи":
                        query = query.OrderByDescending(l => l.IssueDate);
                        break;
                    case "Сначала старые выдачи":
                        query = query.OrderBy(l => l.IssueDate);
                        break;
                    case "По проживающему":
                        query = query.OrderBy(l => l.Resident.FullName);
                        break;
                    default:
                        query = query.OrderByDescending(l => l.ID);
                        break;
                }
            }

            GridLogs.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void GridLogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridLogs.SelectedItem is IssuanceLog selected)
            {
                CmbResident.SelectedValue = selected.ResidentID;
                CmbItem.SelectedValue = selected.ItemID;
                CmbEmployee.SelectedValue = selected.EmployeeID;
                DpIssueDate.SelectedDate = selected.IssueDate;
                DpReturnDate.SelectedDate = selected.ReturnDate;
                TxtQuantity.Text = selected.Quantity.ToString();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (CmbResident.SelectedValue == null || CmbItem.SelectedValue == null || CmbEmployee.SelectedValue == null || DpIssueDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните все основные списки и выберите дату выдачи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Количество должно быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newLog = new IssuanceLog
            {
                ResidentID = (int)CmbResident.SelectedValue,
                ItemID = (int)CmbItem.SelectedValue,
                EmployeeID = (int)CmbEmployee.SelectedValue,
                IssueDate = DpIssueDate.SelectedDate.Value,
                ReturnDate = DpReturnDate.SelectedDate,
                Quantity = qty
            };

            _context.IssuanceLogs.Add(newLog);
            _context.SaveChanges();
            LoadData();

            string resName = (CmbResident.SelectedItem as Resident)?.FullName;
            string itemName = (CmbItem.SelectedItem as Item)?.ItemName;
            ClearFields();

            MessageBox.Show($"Оформлена выдача:\nПроживающий: {resName}\nПредмет: {itemName}\nКоличество: {qty}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (GridLogs.SelectedItem is IssuanceLog selected)
            {
                if (CmbResident.SelectedValue == null || CmbItem.SelectedValue == null || CmbEmployee.SelectedValue == null || DpIssueDate.SelectedDate == null)
                {
                    MessageBox.Show("Списки и дата выдачи не могут быть пустыми.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(TxtQuantity.Text, out int qty) || qty <= 0)
                {
                    MessageBox.Show("Количество должно быть положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selected.ResidentID = (int)CmbResident.SelectedValue;
                selected.ItemID = (int)CmbItem.SelectedValue;
                selected.EmployeeID = (int)CmbEmployee.SelectedValue;
                selected.IssueDate = DpIssueDate.SelectedDate.Value;
                selected.ReturnDate = DpReturnDate.SelectedDate;
                selected.Quantity = qty;

                _context.SaveChanges();
                LoadData();
                ClearFields();

                MessageBox.Show("Изменения сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridLogs.SelectedItem is IssuanceLog selected)
            {
                if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.IssuanceLogs.Remove(selected);
                    _context.SaveChanges();
                    LoadData();
                    ClearFields();
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            CmbResident.SelectedIndex = -1;
            CmbItem.SelectedIndex = -1;
            CmbEmployee.SelectedIndex = -1;
            DpIssueDate.SelectedDate = DateTime.Now;
            DpReturnDate.SelectedDate = null;
            TxtQuantity.Text = "1";
            GridLogs.SelectedItem = null;
        }
    }
}