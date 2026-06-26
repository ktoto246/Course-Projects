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
    public partial class ManufacturersPage : UserControl
    {
        private int selectedId = -1;
        private List<Manufacturer> _allManufacturers = new();

        public ManufacturersPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                _allManufacturers = context.Manufacturers.OrderBy(m => m.ManufacturerID).ToList();
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
            if (ManufacturersDataGrid == null) return;

            var result = _allManufacturers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string search = SearchTextBox.Text.Trim().ToLower();
                result = result.Where(m =>
                    m.ManufacturerName.ToLower().Contains(search) ||
                    (m.Country != null && m.Country.ToLower().Contains(search)));
            }

            ManufacturersDataGrid.ItemsSource = result.ToList();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void SearchBtn_Click(object sender, RoutedEventArgs e) => ApplyFilters();
        private void ResetSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            ApplyFilters();
        }

        private void ManufacturersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ManufacturersDataGrid.SelectedItem is Manufacturer m)
            {
                selectedId = m.ManufacturerID;
                NameTextBox.Text = m.ManufacturerName;
                CountryTextBox.Text = m.Country ?? "";
            }
            else
            {
                selectedId = -1;
                NameTextBox.Text = "";
                CountryTextBox.Text = "";
            }
        }

        private string ValidateInput(string name, string country)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Название производителя не может быть пустым.";
            if (name.Trim().Length < 2) return "Название должно содержать минимум 2 символа.";
            if (name.Length > 100) return "Название не должно превышать 100 символов.";
            if (country != null && country.Length > 50) return "Название страны не должно превышать 50 символов.";
            return null;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text.Trim();
            string country = CountryTextBox.Text.Trim();
            string error = ValidateInput(name, country);
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                if (context.Manufacturers.Any(m => m.ManufacturerName == name))
                {
                    StatusTextBlock.Text = "Такой производитель уже существует.";
                    return;
                }
                context.Manufacturers.Add(new Manufacturer { ManufacturerName = name, Country = country });
                context.SaveChanges();
                StatusTextBlock.Text = "Производитель успешно добавлен.";
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
            string name = NameTextBox.Text.Trim();
            string country = CountryTextBox.Text.Trim();
            string error = ValidateInput(name, country);
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                var entity = context.Manufacturers.Find(selectedId);
                if (entity == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                if (context.Manufacturers.Any(m => m.ManufacturerName == name && m.ManufacturerID != selectedId))
                {
                    StatusTextBlock.Text = "Такой производитель уже существует.";
                    return;
                }

                entity.ManufacturerName = name;
                entity.Country = string.IsNullOrEmpty(country) ? null : country;
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
            if (MessageBox.Show("Удалить выбранного производителя?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

            try
            {
                using var context = new BaltexEquipmentContext();
                var entity = context.Manufacturers.Find(selectedId);
                if (entity == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                context.Manufacturers.Remove(entity);
                context.SaveChanges();
                StatusTextBlock.Text = "Производитель успешно удален.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                ClearFields();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("conflicted with the FOREIGN KEY") == true || ex.InnerException?.Message.Contains("REFERENCES constraint") == true)
            {
                StatusTextBlock.Text = "Невозможно удалить: производитель связан с оборудованием.";
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
            NameTextBox.Text = "";
            CountryTextBox.Text = "";
            selectedId = -1;
            ManufacturersDataGrid.SelectedItem = null;
            StatusTextBlock.Text = "";
        }
    }
}