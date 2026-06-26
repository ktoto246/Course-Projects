using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class DistrictsPage : Page
    {
        private int? _currentId;

        public DistrictsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string searchQuery = null)
        {
            try
            {
                using var context = new PrikhoperskoeDbContext();
                IQueryable<District> query = context.Districts;

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(d => d.Name.Contains(searchQuery) || d.DispatcherPhone.Contains(searchQuery));
                }

                dgDistricts.ItemsSource = query.OrderBy(d => d.Name).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgDistricts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDistricts.SelectedItem is District selected)
            {
                _currentId = selected.Id;
                txtName.Text = selected.Name;
                txtPhone.Text = selected.DispatcherPhone;
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
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    MessageBox.Show("Заполните все поля.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var context = new PrikhoperskoeDbContext();

                if (_currentId.HasValue)
                {
                    var existing = context.Districts.Find(_currentId.Value);
                    if (existing != null)
                    {
                        existing.Name = txtName.Text.Trim();
                        existing.DispatcherPhone = txtPhone.Text.Trim();
                    }
                }
                else
                {
                    context.Districts.Add(new District { Name = txtName.Text.Trim(), DispatcherPhone = txtPhone.Text.Trim() });
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
            dgDistricts.SelectedItem = null;
            txtName.Clear();
            txtPhone.Clear();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgDistricts.SelectedItem is District selected)
                {
                    if (MessageBox.Show($"Удалить '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using var context = new PrikhoperskoeDbContext();
                        var toDelete = context.Districts.Find(selected.Id);
                        if (toDelete != null)
                        {
                            context.Districts.Remove(toDelete);
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