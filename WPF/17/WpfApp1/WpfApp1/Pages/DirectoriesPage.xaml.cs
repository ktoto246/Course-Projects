using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class DirectoriesPage : Page
    {
        private int? _currentDistrictId;
        private int? _currentEquipmentId;

        public DirectoriesPage()
        {
            InitializeComponent();
            LoadDistricts();
            LoadEquipmentTypes();
        }

        private void LoadDistricts(string searchQuery = null)
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
                MessageBox.Show($"Ошибка загрузки районов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgDistricts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDistricts.SelectedItem is District selected)
            {
                _currentDistrictId = selected.Id;
                txtDistrictName.Text = selected.Name;
                txtDistrictPhone.Text = selected.DispatcherPhone;
            }
            else
            {
                _currentDistrictId = null;
            }
        }

        private void BtnSaveDistrict_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtDistrictName.Text))
                {
                    MessageBox.Show("Введите название района.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDistrictPhone.Text))
                {
                    MessageBox.Show("Введите телефон диспетчера.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var context = new PrikhoperskoeDbContext();

                if (_currentDistrictId.HasValue)
                {
                    var existing = context.Districts.Find(_currentDistrictId.Value);
                    if (existing != null)
                    {
                        existing.Name = txtDistrictName.Text.Trim();
                        existing.DispatcherPhone = txtDistrictPhone.Text.Trim();
                    }
                }
                else
                {
                    var newDistrict = new District
                    {
                        Name = txtDistrictName.Text.Trim(),
                        DispatcherPhone = txtDistrictPhone.Text.Trim()
                    };
                    context.Districts.Add(newDistrict);
                }

                context.SaveChanges();
                MessageBox.Show("Район успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClearDistrict_Click(sender, e);
                LoadDistricts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\n\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClearDistrict_Click(object sender, RoutedEventArgs e)
        {
            _currentDistrictId = null;
            dgDistricts.SelectedItem = null;
            txtDistrictName.Clear();
            txtDistrictPhone.Clear();
        }

        private void BtnDeleteDistrict_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgDistricts.SelectedItem is District selected)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить район '{selected.Name}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        using var context = new PrikhoperskoeDbContext();
                        var toDelete = context.Districts.Find(selected.Id);
                        if (toDelete != null)
                        {
                            context.Districts.Remove(toDelete);
                            context.SaveChanges();
                            MessageBox.Show("Район удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            BtnClearDistrict_Click(sender, e);
                            LoadDistricts();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите район для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}\n\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearchDistrict_Click(object sender, RoutedEventArgs e)
        {
            LoadDistricts(txtSearchDistrict.Text);
        }

        private void LoadEquipmentTypes(string searchQuery = null)
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
                MessageBox.Show($"Ошибка загрузки оборудования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgEquipment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgEquipment.SelectedItem is EquipmentType selected)
            {
                _currentEquipmentId = selected.Id;
                txtEquipmentName.Text = selected.Name;
            }
            else
            {
                _currentEquipmentId = null;
            }
        }

        private void BtnSaveEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtEquipmentName.Text))
                {
                    MessageBox.Show("Введите название типа оборудования.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var context = new PrikhoperskoeDbContext();

                if (_currentEquipmentId.HasValue)
                {
                    var existing = context.EquipmentTypes.Find(_currentEquipmentId.Value);
                    if (existing != null)
                    {
                        existing.Name = txtEquipmentName.Text.Trim();
                    }
                }
                else
                {
                    var newEquipment = new EquipmentType
                    {
                        Name = txtEquipmentName.Text.Trim()
                    };
                    context.EquipmentTypes.Add(newEquipment);
                }

                context.SaveChanges();
                MessageBox.Show("Тип оборудования успешно сохранен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClearEquipment_Click(sender, e);
                LoadEquipmentTypes();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\n\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClearEquipment_Click(object sender, RoutedEventArgs e)
        {
            _currentEquipmentId = null;
            dgEquipment.SelectedItem = null;
            txtEquipmentName.Clear();
        }

        private void BtnDeleteEquipment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgEquipment.SelectedItem is EquipmentType selected)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить тип оборудования '{selected.Name}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        using var context = new PrikhoperskoeDbContext();
                        var toDelete = context.EquipmentTypes.Find(selected.Id);
                        if (toDelete != null)
                        {
                            context.EquipmentTypes.Remove(toDelete);
                            context.SaveChanges();
                            MessageBox.Show("Тип оборудования удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            BtnClearEquipment_Click(sender, e);
                            LoadEquipmentTypes();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите тип оборудования для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}\n\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearchEquipment_Click(object sender, RoutedEventArgs e)
        {
            LoadEquipmentTypes(txtSearchEquipment.Text);
        }
    }
}