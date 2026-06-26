using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class WriteOffPage : UserControl
    {
        private int selectedId = -1;

        public WriteOffPage()
        {
            InitializeComponent();
            LoadData();
            LoadEquipmentComboBox();
        }

        private void LoadData()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                var list = context.Equipment
                    .Include(e => e.Status)
                    .Where(e => e.Status.StatusName == "Списано")
                    .OrderBy(e => e.InventoryNumber)
                    .ToList();
                WriteOffDataGrid.ItemsSource = list;
                StatusTextBlock.Text = "";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка загрузки: " + ex.Message;
            }
        }

        private void LoadEquipmentComboBox()
        {
            using var context = new BaltexEquipmentContext();
            var available = context.Equipment
                 .Include(e => e.Status)
                 .Where(e => e.Status.StatusName != "Списано")
                 .OrderBy(e => e.InventoryNumber)
                 .ToList();

            EquipmentComboBox.ItemsSource = available;
            EquipmentComboBox.DisplayMemberPath = "InventoryNumber";
            EquipmentComboBox.SelectedValuePath = "EquipmentID";
        }

        private void WriteOffDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WriteOffDataGrid.SelectedItem is Equipment eq)
            {
                selectedId = eq.EquipmentID;
                WriteOffDatePicker.SelectedDate = eq.WriteOffDate.HasValue ? eq.WriteOffDate.Value.ToDateTime(TimeOnly.MinValue) : null;
                RepairCostTextBox.Text = eq.WriteOffRepairCost?.ToString("0.00") ?? "";
                PartsCostTextBox.Text = eq.WriteOffPartsCost?.ToString("0.00") ?? "";
            }
            else
            {
                selectedId = -1;
                WriteOffDatePicker.SelectedDate = null;
                RepairCostTextBox.Text = "";
                PartsCostTextBox.Text = "";
            }
        }

        private string ValidateInput()
        {
            if (EquipmentComboBox.SelectedValue == null) return "Выберите оборудование для списания.";
            if (WriteOffDatePicker.SelectedDate == null) return "Укажите дату списания.";
            if (!decimal.TryParse(RepairCostTextBox.Text, out decimal repair)) return "Некорректная стоимость ремонта.";
            if (!decimal.TryParse(PartsCostTextBox.Text, out decimal parts)) return "Некорректная стоимость деталей.";
            if (repair < 0 || parts < 0) return "Стоимость не может быть отрицательной.";
            return null;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string error = ValidateInput();
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                int eqId = (int)EquipmentComboBox.SelectedValue;
                var eq = context.Equipment.Include(e => e.Status).FirstOrDefault(e => e.EquipmentID == eqId);
                if (eq == null) { StatusTextBlock.Text = "Оборудование не найдено."; return; }
                if (eq.Status.StatusName == "Списано") { StatusTextBlock.Text = "Это оборудование уже списано."; return; }

                var status = context.EquipmentStatuses.FirstOrDefault(s => s.StatusName == "Списано");
                if (status == null) { StatusTextBlock.Text = "Статус «Списано» не найден в БД."; return; }

                eq.StatusID = status.StatusID;
                eq.WriteOffDate = DateOnly.FromDateTime(WriteOffDatePicker.SelectedDate.Value);
                eq.WriteOffRepairCost = decimal.Parse(RepairCostTextBox.Text);
                eq.WriteOffPartsCost = decimal.Parse(PartsCostTextBox.Text);

                context.SaveChanges();
                StatusTextBlock.Text = "Оборудование успешно списано.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                LoadEquipmentComboBox();
                ClearFields();
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка сохранения: " + ex.Message;
                StatusTextBlock.Foreground = Brushes.Red;
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearFields();

        private void ClearFields()
        {
            selectedId = -1;
            EquipmentComboBox.SelectedIndex = -1;
            WriteOffDatePicker.SelectedDate = null;
            RepairCostTextBox.Text = "";
            PartsCostTextBox.Text = "";
            WriteOffDataGrid.SelectedItem = null;
            StatusTextBlock.Text = "";
        }

        private void NumberPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, e.Text.Length - 1) || e.Text == "," || e.Text == "."))
                e.Handled = true;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                string search = SearchTextBox?.Text?.Trim()?.ToLower() ?? "";

                var query = context.Equipment
                    .Include(e => e.Status)
                    .Where(e => e.Status.StatusName == "Списано");

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(e =>
                        e.InventoryNumber.ToLower().Contains(search) ||
                        e.EquipmentName.ToLower().Contains(search) ||
                        (e.Model != null && e.Model.ToLower().Contains(search)));
                }

                WriteOffDataGrid.ItemsSource = query.OrderBy(e => e.InventoryNumber).ToList();
            }
            catch { }
        }

        private void ResetSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox != null) SearchTextBox.Text = "";
            LoadData();
        }
    }
}