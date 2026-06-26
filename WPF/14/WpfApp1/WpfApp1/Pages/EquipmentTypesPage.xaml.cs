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
    public partial class EquipmentTypesPage : UserControl
    {
        private int selectedId = -1;
        private List<EquipmentType> _allTypes = new();

        public EquipmentTypesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using var context = new BaltexEquipmentContext();
                _allTypes = context.EquipmentTypes.OrderBy(t => t.TypeID).ToList();
                TypesDataGrid.ItemsSource = _allTypes;
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
                TypesDataGrid.ItemsSource = _allTypes;
            }
            else
            {
                var result = _allTypes.Where(t => t.TypeName.ToLower().Contains(search)).ToList();
                TypesDataGrid.ItemsSource = result;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => ApplySearch();

        private void SearchBtn_Click(object sender, RoutedEventArgs e) => ApplySearch();

        private void ResetSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            TypesDataGrid.ItemsSource = _allTypes;
        }

        private void TypesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypesDataGrid.SelectedItem is EquipmentType t)
            {
                selectedId = t.TypeID;
                TypeNameTextBox.Text = t.TypeName;
            }
            else
            {
                selectedId = -1;
                TypeNameTextBox.Text = "";
            }
        }

        private string ValidateInput(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Название типа не может быть пустым.";
            if (name.Trim().Length < 3) return "Название должно содержать минимум 3 символа.";
            if (name.Length > 100) return "Название не должно превышать 100 символов.";
            return null;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            string name = TypeNameTextBox.Text.Trim();
            string error = ValidateInput(name);
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                if (context.EquipmentTypes.Any(t => t.TypeName == name))
                {
                    StatusTextBlock.Text = "Такой тип уже существует.";
                    return;
                }
                context.EquipmentTypes.Add(new EquipmentType { TypeName = name });
                context.SaveChanges();
                StatusTextBlock.Text = "Тип успешно добавлен.";
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
            string name = TypeNameTextBox.Text.Trim();
            string error = ValidateInput(name);
            if (error != null) { StatusTextBlock.Text = error; return; }

            try
            {
                using var context = new BaltexEquipmentContext();
                var entity = context.EquipmentTypes.Find(selectedId);
                if (entity == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                if (context.EquipmentTypes.Any(t => t.TypeName == name && t.TypeID != selectedId))
                {
                    StatusTextBlock.Text = "Такой тип уже существует.";
                    return;
                }

                entity.TypeName = name;
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
            if (MessageBox.Show("Удалить выбранный тип?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No) return;

            try
            {
                using var context = new BaltexEquipmentContext();
                var entity = context.EquipmentTypes.Find(selectedId);
                if (entity == null) { StatusTextBlock.Text = "Запись не найдена."; return; }

                context.EquipmentTypes.Remove(entity);
                context.SaveChanges();
                StatusTextBlock.Text = "Тип успешно удален.";
                StatusTextBlock.Foreground = Brushes.Green;
                LoadData();
                ClearFields();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("conflicted with the FOREIGN KEY") == true || ex.InnerException?.Message.Contains("REFERENCES constraint") == true)
            {
                StatusTextBlock.Text = "Невозможно удалить: тип связан с оборудованием.";
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
            TypeNameTextBox.Text = "";
            selectedId = -1;
            TypesDataGrid.SelectedItem = null;
            StatusTextBlock.Text = "";
        }
    }
}