using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class EquipmentPage : Page
    {
        private int? _currentId;

        public EquipmentPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string searchQuery = null)
        {
            try
            {
                using var context = new PrikhoperskoeDbContext();
                IQueryable<EquipmentType> query = context.EquipmentTypes;

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(e => e.Name.Contains(searchQuery));
                }

                dgEquipment.ItemsSource = query.OrderBy(e => e.Name).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgEquipment.SelectedItem is EquipmentType selected)
            {
                _currentId = selected.Id;
                txtName.Text = selected.Name;
            }
            else
            {
                _currentId = null;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите наименование.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var context = new PrikhoperskoeDbContext();

                if (_currentId.HasValue)
                {
                    var existing = context.EquipmentTypes.Find(_currentId.Value);
                    if (existing != null) existing.Name = txtName.Text.Trim();
                }
                else
                {
                    context.EquipmentTypes.Add(new EquipmentType { Name = txtName.Text.Trim() });
                }

                context.SaveChanges();
                MessageBox.Show("Сохранено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClear_Click(sender, e);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _currentId = null;
            dgEquipment.SelectedItem = null;
            txtName.Clear();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgEquipment.SelectedItem is EquipmentType selected)
                {
                    if (MessageBox.Show($"Удалить '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using var context = new PrikhoperskoeDbContext();
                        var toDelete = context.EquipmentTypes.Find(selected.Id);
                        if (toDelete != null)
                        {
                            context.EquipmentTypes.Remove(toDelete);
                            context.SaveChanges();
                            MessageBox.Show("Удалено.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            BtnClear_Click(sender, e);
                            LoadData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите запись.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e) => LoadData(txtSearch.Text);
    }
}