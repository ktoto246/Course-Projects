using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class MaintenancePage : UserControl
    {
        private int selectedId = -1;
        private List<MaintenanceRecord> _allRecords = new();

        public MaintenancePage()
        {
            InitializeComponent();
            LoadFilterComboBox();
            LoadEquipmentComboBox();
            LoadEmployeesComboBox();
            LoadData();
        }

        private void LoadFilterComboBox()
        {
            FilterTypeCombo.ItemsSource = new List<string> { "Все типы", "Плановое ТО", "Внеплановый ремонт", "Диагностика" };
            FilterTypeCombo.SelectedIndex = 0;
        }

        private void LoadEquipmentComboBox()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                var list = context.Equipment.OrderBy(e => e.InventoryNumber).ToList();
                EquipmentComboBox.ItemsSource = list;
                EquipmentComboBox.DisplayMemberPath = "InventoryNumber";
                EquipmentComboBox.SelectedValuePath = "EquipmentID";
            }
            catch { }
        }

        private void LoadEmployeesComboBox()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                var list = context.Employees.OrderBy(e => e.FullName).ToList();
                TechComboBox.ItemsSource = list;
                TechComboBox.DisplayMemberPath = "FullName";
                TechComboBox.SelectedValuePath = "EmployeeID";
            }
            catch { }
        }

        private void LoadData()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                _allRecords = context.MaintenanceRecords
                    .Include(m => m.Equipment)
                    .Include(m => m.Employee)
                    .OrderByDescending(m => m.MaintenanceDate)
                    .ToList();
                ApplyFilters();
                StatusTextBlock.Text = "";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка загрузки: " + ex.Message;
            }
        }

        private void ApplyFilters()
        {
            if (MaintenanceDataGrid == null) return;

            var result = _allRecords.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string search = SearchTextBox.Text.Trim().ToLower();
                result = result.Where(r =>
                    (r.Equipment != null && r.Equipment.InventoryNumber.ToLower().Contains(search)) ||
                    (r.Employee != null && r.Employee.FullName.ToLower().Contains(search)) ||
                    (r.Description != null && r.Description.ToLower().Contains(search)));
            }

            if (FilterTypeCombo.SelectedItem is string typeFilter && typeFilter != "Все типы")
            {
                result = result.Where(r => r.MaintenanceType == typeFilter);
            }

            MaintenanceDataGrid.ItemsSource = result.ToList();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void FilterTypeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ResetSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            FilterTypeCombo.SelectedIndex = 0;
            ApplyFilters();
        }

        private void MaintenanceDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaintenanceDataGrid.SelectedItem is MaintenanceRecord rec)
            {
                selectedId = rec.RecordID;
                EquipmentComboBox.SelectedValue = rec.EquipmentID;
                TechComboBox.SelectedValue = rec.EmployeeID;
                DateDatePicker.SelectedDate = rec.MaintenanceDate.ToDateTime(TimeOnly.MinValue);
                TypeComboBox.SelectedValue = TypeComboBox.Items.Cast<ComboBoxItem>().FirstOrDefault(i => i.Content.ToString() == rec.MaintenanceType);
                CostTextBox.Text = rec.Cost.ToString("0.00");
                DescTextBox.Text = rec.Description ?? "";
            }
            else
            {
                ClearFields();
            }
        }

        private string ValidateInput()
        {
            if (EquipmentComboBox.SelectedValue == null) return "Выберите оборудование.";
            if (TechComboBox.SelectedValue == null) return "Выберите сотрудника.";
            if (DateDatePicker.SelectedDate == null) return "Укажите дату проведения работ.";
            if (TypeComboBox.SelectedItem == null) return "Выберите тип работ.";
            if (!decimal.TryParse(CostTextBox.Text, out decimal cost)) return "Некорректная стоимость.";
            if (cost < 0) return "Стоимость не может быть отрицательной.";
            return null;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string error = ValidateInput();
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();

                int eqId = (int)EquipmentComboBox.SelectedValue;
                string maintType = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString();

                context.MaintenanceRecords.Add(new MaintenanceRecord
                {
                    EquipmentID = eqId,
                    EmployeeID = (int)TechComboBox.SelectedValue,
                    MaintenanceDate = DateOnly.FromDateTime(DateDatePicker.SelectedDate.Value),
                    MaintenanceType = maintType,
                    Cost = decimal.Parse(CostTextBox.Text),
                    Description = string.IsNullOrEmpty(DescTextBox.Text) ? null : DescTextBox.Text.Trim()
                });

                var equipment = context.Equipment.Find(eqId);
                if (equipment != null)
                {
                    string targetStatusName = "В ремонте";

                    if (maintType == "Плановое ТО")
                        targetStatusName = "На плановом ТО";
                    else if (maintType == "Внеплановый ремонт" || maintType == "Диагностика")
                        targetStatusName = "В ремонте";

                    var newStatus = context.EquipmentStatuses.FirstOrDefault(s => s.StatusName == targetStatusName);
                    if (newStatus != null)
                    {
                        equipment.StatusID = newStatus.StatusID;
                    }
                }
                context.SaveChanges();

                StatusTextBlock.Text = "Запись добавлена, статус оборудования обновлен.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка добавления: " + ex.Message;
                StatusTextBlock.Foreground = Brushes.Red;
            }
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedId == -1) { StatusTextBlock.Text = "Выберите запись для изменения."; return; }
            string error = ValidateInput();
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                var rec = context.MaintenanceRecords.Find(selectedId);
                if (rec == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                rec.EquipmentID = (int)EquipmentComboBox.SelectedValue;
                rec.EmployeeID = (int)TechComboBox.SelectedValue;
                rec.MaintenanceDate = DateOnly.FromDateTime(DateDatePicker.SelectedDate.Value);
                rec.MaintenanceType = ((ComboBoxItem)TypeComboBox.SelectedItem).Content.ToString();
                rec.Cost = decimal.Parse(CostTextBox.Text);
                rec.Description = string.IsNullOrEmpty(DescTextBox.Text) ? null : DescTextBox.Text.Trim();

                context.SaveChanges();
                StatusTextBlock.Text = "Данные успешно обновлены.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка обновления: " + ex.Message;
                StatusTextBlock.Foreground = Brushes.Red;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (selectedId == -1) { StatusTextBlock.Text = "Выберите запись для удаления."; return; }
            if (MessageBox.Show("Удалить выбранную запись из журнала ТО?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

            try
            {
                using var context = new BaltexEquipmentContext();
                var rec = context.MaintenanceRecords.Find(selectedId);
                if (rec == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                context.MaintenanceRecords.Remove(rec);
                context.SaveChanges();
                StatusTextBlock.Text = "Запись успешно удалена.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                ClearFields();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка удаления: " + ex.Message;
                StatusTextBlock.Foreground = Brushes.Red;
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearFields();

        private void ClearFields()
        {
            selectedId = -1;
            EquipmentComboBox.SelectedIndex = -1;
            TechComboBox.SelectedIndex = -1;
            DateDatePicker.SelectedDate = null;
            TypeComboBox.SelectedIndex = -1;
            CostTextBox.Text = "";
            DescTextBox.Text = "";
            MaintenanceDataGrid.SelectedItem = null;
            StatusTextBlock.Text = "";
        }

        private void NumberPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, e.Text.Length - 1) || e.Text == "," || e.Text == "."))
                e.Handled = true;
        }
    }
}