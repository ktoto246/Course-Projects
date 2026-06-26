using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class EquipmentPage : UserControl
    {
        private int selectedId = -1;
        private List<Equipment> _allEquipment = new();

        public EquipmentPage()
        {
            InitializeComponent();
            LoadData();
            LoadComboBoxes();
            LoadFilterComboBoxes();
        }

        private void LoadData()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                _allEquipment = context.Equipment
                    .Include(e => e.Type)
                    .Include(e => e.Manufacturer)
                    .Include(e => e.Unit)
                    .Include(e => e.Status)
                    .OrderBy(e => e.InventoryNumber)
                    .ToList();
                EquipmentDataGrid.ItemsSource = _allEquipment;
                StatusTextBlock.Text = "";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка загрузки: " + ex.Message;
            }
        }

        private void LoadComboBoxes()
        {
            using var context = new BaltexEquipmentContext();
            TypeComboBox.ItemsSource = context.EquipmentTypes.OrderBy(t => t.TypeName).ToList();
            TypeComboBox.DisplayMemberPath = "TypeName";
            TypeComboBox.SelectedValuePath = "TypeID";

            ManufacturerComboBox.ItemsSource = context.Manufacturers.OrderBy(m => m.ManufacturerName).ToList();
            ManufacturerComboBox.DisplayMemberPath = "ManufacturerName";
            ManufacturerComboBox.SelectedValuePath = "ManufacturerID";

            UnitComboBox.ItemsSource = context.ProductionUnits.OrderBy(u => u.UnitName).ToList();
            UnitComboBox.DisplayMemberPath = "UnitName";
            UnitComboBox.SelectedValuePath = "UnitID";

            StatusComboBox.ItemsSource = context.EquipmentStatuses.OrderBy(s => s.StatusName).ToList();
            StatusComboBox.DisplayMemberPath = "StatusName";
            StatusComboBox.SelectedValuePath = "StatusID";
        }

        private void LoadFilterComboBoxes()
        {
            using var context = new BaltexEquipmentContext();

            var statuses = context.EquipmentStatuses.OrderBy(s => s.StatusName).ToList();
            FilterStatusCombo.ItemsSource = new[] { "Все статусы" }.Concat(statuses.Select(s => s.StatusName)).ToList();
            FilterStatusCombo.SelectedIndex = 0;

            var units = context.ProductionUnits.OrderBy(u => u.UnitName).ToList();
            FilterUnitCombo.ItemsSource = new[] { "Все цеха" }.Concat(units.Select(u => u.UnitName)).ToList();
            FilterUnitCombo.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            if (EquipmentDataGrid == null) return;

            var result = _allEquipment.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string search = SearchTextBox.Text.Trim().ToLower();
                result = result.Where(e =>
                    e.InventoryNumber.ToLower().Contains(search) ||
                    e.EquipmentName.ToLower().Contains(search) ||
                    (e.Model != null && e.Model.ToLower().Contains(search)));
            }

            if (FilterStatusCombo.SelectedItem is string statusFilter && statusFilter != "Все статусы")
            {
                result = result.Where(e => e.Status.StatusName == statusFilter);
            }

            if (FilterUnitCombo.SelectedItem is string unitFilter && unitFilter != "Все цеха")
            {
                result = result.Where(e => e.Unit.UnitName == unitFilter);
            }

            EquipmentDataGrid.ItemsSource = result.ToList();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();


        private void ResetSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            FilterStatusCombo.SelectedIndex = 0;
            FilterUnitCombo.SelectedIndex = 0;
            EquipmentDataGrid.ItemsSource = _allEquipment;
        }

        private void FilterStatusCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void FilterUnitCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void EquipmentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EquipmentDataGrid.SelectedItem is Equipment eq)
            {
                selectedId = eq.EquipmentID;
                InvNumTextBox.Text = eq.InventoryNumber;
                NameTextBox.Text = eq.EquipmentName;
                TypeComboBox.SelectedValue = eq.TypeID;
                ManufacturerComboBox.SelectedValue = eq.ManufacturerID;
                ModelTextBox.Text = eq.Model ?? "";
                UnitComboBox.SelectedValue = eq.UnitID;
                StatusComboBox.SelectedValue = eq.StatusID;
                InstallDatePicker.SelectedDate = eq.InstallDate.HasValue ? eq.InstallDate.Value.ToDateTime(TimeOnly.MinValue) : null;
            }
            else
            {
                selectedId = -1;
                InvNumTextBox.Text = "";
                NameTextBox.Text = "";
                TypeComboBox.SelectedIndex = -1;
                ManufacturerComboBox.SelectedIndex = -1;
                ModelTextBox.Text = "";
                UnitComboBox.SelectedIndex = -1;
                StatusComboBox.SelectedIndex = -1;
                InstallDatePicker.SelectedDate = null;
            }
        }

        private string ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(InvNumTextBox.Text)) return "Инвентарный номер обязателен.";
            if (InvNumTextBox.Text.Trim().Length > 20) return "Инвентарный номер макс. 20 символов.";
            if (string.IsNullOrWhiteSpace(NameTextBox.Text)) return "Наименование обязательно.";
            if (NameTextBox.Text.Trim().Length < 3) return "Наименование мин. 3 символа.";
            if (TypeComboBox.SelectedValue == null) return "Выберите тип оборудования.";
            if (ManufacturerComboBox.SelectedValue == null) return "Выберите производителя.";
            if (UnitComboBox.SelectedValue == null) return "Выберите цех.";
            if (StatusComboBox.SelectedValue == null) return "Выберите статус.";
            return null;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string error = ValidateInput();
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                string inv = InvNumTextBox.Text.Trim();
                if (context.Equipment.Any(x => x.InventoryNumber == inv))
                {
                    StatusTextBlock.Text = "Инвентарный номер уже существует.";
                    return;
                }

                context.Equipment.Add(new Equipment
                {
                    InventoryNumber = inv,
                    EquipmentName = NameTextBox.Text.Trim(),
                    TypeID = (int)TypeComboBox.SelectedValue,
                    ManufacturerID = (int)ManufacturerComboBox.SelectedValue,
                    Model = string.IsNullOrEmpty(ModelTextBox.Text) ? null : ModelTextBox.Text.Trim(),
                    UnitID = (int)UnitComboBox.SelectedValue,
                    StatusID = (int)StatusComboBox.SelectedValue,
                    InstallDate = InstallDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(InstallDatePicker.SelectedDate.Value) : null
                });
                context.SaveChanges();
                StatusTextBlock.Text = "Оборудование успешно добавлено.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                LoadFilterComboBoxes();
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
                var eq = context.Equipment.Find(selectedId);
                if (eq == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                string inv = InvNumTextBox.Text.Trim();
                if (context.Equipment.Any(x => x.InventoryNumber == inv && x.EquipmentID != selectedId))
                {
                    StatusTextBlock.Text = "Инвентарный номер уже существует.";
                    return;
                }

                eq.InventoryNumber = inv;
                eq.EquipmentName = NameTextBox.Text.Trim();
                eq.TypeID = (int)TypeComboBox.SelectedValue;
                eq.ManufacturerID = (int)ManufacturerComboBox.SelectedValue;
                eq.Model = string.IsNullOrEmpty(ModelTextBox.Text) ? null : ModelTextBox.Text.Trim();
                eq.UnitID = (int)UnitComboBox.SelectedValue;
                eq.StatusID = (int)StatusComboBox.SelectedValue;
                eq.InstallDate = InstallDatePicker.SelectedDate.HasValue ? DateOnly.FromDateTime(InstallDatePicker.SelectedDate.Value) : null;

                context.SaveChanges();
                StatusTextBlock.Text = "Данные успешно обновлены.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                LoadFilterComboBoxes();
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
            if (MessageBox.Show("Удалить выбранное оборудование?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

            try
            {
                using var context = new BaltexEquipmentContext();
                var eq = context.Equipment.Find(selectedId);
                if (eq == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                context.Equipment.Remove(eq);
                context.SaveChanges();
                StatusTextBlock.Text = "Оборудование успешно удалено.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                LoadFilterComboBoxes();
                ClearFields();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("REFERENCES constraint") == true || ex.InnerException?.Message.Contains("FOREIGN KEY") == true)
            {
                StatusTextBlock.Text = "Невозможно удалить: оборудование связано с журналом ТО.";
                StatusTextBlock.Foreground = Brushes.Red;
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
            InvNumTextBox.Text = "";
            NameTextBox.Text = "";
            TypeComboBox.SelectedIndex = -1;
            ManufacturerComboBox.SelectedIndex = -1;
            ModelTextBox.Text = "";
            UnitComboBox.SelectedIndex = -1;
            StatusComboBox.SelectedIndex = -1;
            InstallDatePicker.SelectedDate = null;
            EquipmentDataGrid.SelectedItem = null;
            StatusTextBlock.Text = "";
        }
    }
}