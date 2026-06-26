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
    public partial class EquipmentStatusesPage : UserControl
    {
        private int selectedId = -1;
        private List<EquipmentStatus> _allStatuses = new();

        public EquipmentStatusesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                _allStatuses = context.EquipmentStatuses.OrderBy(s => s.StatusID).ToList();
                StatusesDataGrid.ItemsSource = _allStatuses;
                StatusTextBlock.Text = "";
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = "Ошибка загрузки: " + ex.Message;
            }
        }

        private void ApplySearch()
        {
            string search = SearchTextBox.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(search))
            {
                StatusesDataGrid.ItemsSource = _allStatuses;
            }
            else
            {
                var result = _allStatuses.Where(s => s.StatusName.ToLower().Contains(search)).ToList();
                StatusesDataGrid.ItemsSource = result;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplySearch();

        private void SearchBtn_Click(object sender, RoutedEventArgs e) => ApplySearch();

        private void ResetSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            StatusesDataGrid.ItemsSource = _allStatuses;
        }

        private void StatusesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusesDataGrid.SelectedItem is EquipmentStatus s)
            {
                selectedId = s.StatusID;
                StatusNameTextBox.Text = s.StatusName;
            }
            else
            {
                selectedId = -1;
                StatusNameTextBox.Text = "";
            }
        }

        private string ValidateInput(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Название статуса не может быть пустым.";
            if (name.Trim().Length < 3) return "Название должно содержать минимум 3 символа.";
            if (name.Length > 50) return "Название не должно превышать 50 символов.";
            return null;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = StatusNameTextBox.Text.Trim();
            string error = ValidateInput(name);
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                if (context.EquipmentStatuses.Any(s => s.StatusName == name))
                {
                    StatusTextBlock.Text = "Такой статус уже существует.";
                    return;
                }
                context.EquipmentStatuses.Add(new EquipmentStatus { StatusName = name });
                context.SaveChanges();
                StatusTextBlock.Text = "Статус успешно добавлен.";
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
            string name = StatusNameTextBox.Text.Trim();
            string error = ValidateInput(name);
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                var entity = context.EquipmentStatuses.Find(selectedId);
                if (entity == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                if (context.EquipmentStatuses.Any(s => s.StatusName == name && s.StatusID != selectedId))
                {
                    StatusTextBlock.Text = "Такой статус уже существует.";
                    return;
                }

                entity.StatusName = name;
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
            if (MessageBox.Show("Удалить выбранный статус?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

            try
            {
                using var context = new BaltexEquipmentContext();
                var entity = context.EquipmentStatuses.Find(selectedId);
                if (entity == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                context.EquipmentStatuses.Remove(entity);
                context.SaveChanges();
                StatusTextBlock.Text = "Статус успешно удален.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                ClearFields();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("conflicted with the FOREIGN KEY") == true || ex.InnerException?.Message.Contains("REFERENCES constraint") == true)
            {
                StatusTextBlock.Text = "Невозможно удалить: статус используется в оборудовании.";
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
            StatusNameTextBox.Text = "";
            selectedId = -1;
            StatusesDataGrid.SelectedItem = null;
            StatusTextBlock.Text = "";
        }
    }
}