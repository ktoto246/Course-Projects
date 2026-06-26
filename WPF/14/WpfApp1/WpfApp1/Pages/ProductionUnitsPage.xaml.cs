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
    public partial class ProductionUnitsPage : UserControl
    {
        private int selectedUnitId = -1;
        private List<ProductionUnit> _allUnits = new();

        public ProductionUnitsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                _allUnits = context.ProductionUnits.OrderBy(u => u.UnitID).ToList();
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
            if (UnitsDataGrid == null) return;

            var result = _allUnits.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string search = SearchTextBox.Text.Trim().ToLower();
                result = result.Where(u => u.UnitName.ToLower().Contains(search));
            }

            UnitsDataGrid.ItemsSource = result.ToList();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void SearchBtn_Click(object sender, RoutedEventArgs e) => ApplyFilters();
        private void ResetSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            ApplyFilters();
        }

        private void UnitsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UnitsDataGrid.SelectedItem is ProductionUnit unit)
            {
                selectedUnitId = unit.UnitID;
                UnitNameTextBox.Text = unit.UnitName;
            }
            else
            {
                selectedUnitId = -1;
                UnitNameTextBox.Text = "";
            }
        }

        private string ValidateInput(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Название не может быть пустым.";
            if (name.Trim().Length < 3) return "Название должно содержать минимум 3 символа.";
            if (name.Length > 100) return "Название не должно превышать 100 символов.";
            return null;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = UnitNameTextBox.Text.Trim();
            string error = ValidateInput(name);
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                if (context.ProductionUnits.Any(u => u.UnitName == name))
                {
                    StatusTextBlock.Text = "Такое подразделение уже существует.";
                    return;
                }
                context.ProductionUnits.Add(new ProductionUnit { UnitName = name });
                context.SaveChanges();
                StatusTextBlock.Text = "Подразделение успешно добавлено.";
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
            if (selectedUnitId == -1) { StatusTextBlock.Text = "Выберите запись для изменения."; return; }
            string name = UnitNameTextBox.Text.Trim();
            string error = ValidateInput(name);
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                var unit = context.ProductionUnits.Find(selectedUnitId);
                if (unit == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                if (context.ProductionUnits.Any(u => u.UnitName == name && u.UnitID != selectedUnitId))
                {
                    StatusTextBlock.Text = "Такое подразделение уже существует.";
                    return;
                }

                unit.UnitName = name;
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
            if (selectedUnitId == -1) { StatusTextBlock.Text = "Выберите запись для удаления."; return; }

            if (MessageBox.Show("Вы уверены, что хотите удалить выбранное подразделение?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

            try
            {
                using var context = new BaltexEquipmentContext();
                var unit = context.ProductionUnits.Find(selectedUnitId);
                if (unit == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                context.ProductionUnits.Remove(unit);
                context.SaveChanges();
                StatusTextBlock.Text = "Подразделение успешно удалено.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                ClearFields();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("conflicted with the FOREIGN KEY") == true)
            {
                StatusTextBlock.Text = "Невозможно удалить: подразделение связано с оборудованием.";
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
            UnitNameTextBox.Text = "";
            selectedUnitId = -1;
            UnitsDataGrid.SelectedItem = null;
            StatusTextBlock.Text = "";
        }
    }
}