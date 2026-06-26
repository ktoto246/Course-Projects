using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class IncidentsPage : Page
    {
        private int? _currentIncidentId;

        public IncidentsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string searchQuery = null)
        {
            try
            {
                using var context = new PrikhoperskoeDbContext();

                cboDistrict.ItemsSource = context.Districts.OrderBy(d => d.Name).ToList();
                cboEquipment.ItemsSource = context.EquipmentTypes.OrderBy(e => e.Name).ToList();

                IQueryable<Incident> query = context.Incidents
                    .Include(i => i.District)
                    .Include(i => i.EquipmentType);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(i => i.Description.Contains(searchQuery) ||
                                             i.District.Name.Contains(searchQuery) ||
                                             i.Status.Contains(searchQuery));
                }

                dgIncidents.ItemsSource = query.OrderByDescending(i => i.CreatedAt).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgIncidents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgIncidents.SelectedItem is Incident selected)
            {
                _currentIncidentId = selected.Id;
                cboDistrict.SelectedItem = selected.District;
                cboEquipment.SelectedItem = selected.EquipmentType;
                cboStatus.Text = selected.Status;
                dpCreatedAt.SelectedDate = selected.CreatedAt;
                txtDescription.Text = selected.Description;
            }
            else
            {
                _currentIncidentId = null;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedDistrict = cboDistrict.SelectedItem as District;
                var selectedEquipment = cboEquipment.SelectedItem as EquipmentType;
                var selectedStatus = cboStatus.SelectedItem as ComboBoxItem;

                if (selectedDistrict == null)
                {
                    MessageBox.Show("Выберите район.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedEquipment == null)
                {
                    MessageBox.Show("Выберите тип оборудования.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDescription.Text))
                {
                    MessageBox.Show("Введите описание инцидента.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedStatus == null)
                {
                    MessageBox.Show("Выберите статус.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var context = new PrikhoperskoeDbContext();

                if (_currentIncidentId.HasValue)
                {
                    var existing = context.Incidents.Find(_currentIncidentId.Value);
                    if (existing != null)
                    {
                        existing.DistrictId = selectedDistrict.Id;
                        existing.EquipmentTypeId = selectedEquipment.Id;
                        existing.Description = txtDescription.Text.Trim();
                        existing.Status = selectedStatus.Content.ToString();
                        existing.CreatedAt = dpCreatedAt.SelectedDate ?? DateTime.Now;
                    }
                }
                else
                {
                    var newIncident = new Incident
                    {
                        DistrictId = selectedDistrict.Id,
                        EquipmentTypeId = selectedEquipment.Id,
                        Description = txtDescription.Text.Trim(),
                        Status = selectedStatus.Content.ToString(),
                        CreatedAt = dpCreatedAt.SelectedDate ?? DateTime.Now
                    };
                    context.Incidents.Add(newIncident);
                }

                context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
            _currentIncidentId = null;
            dgIncidents.SelectedItem = null;
            cboDistrict.SelectedIndex = -1;
            cboEquipment.SelectedIndex = -1;
            cboStatus.SelectedIndex = -1;
            dpCreatedAt.SelectedDate = DateTime.Now;
            txtDescription.Clear();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgIncidents.SelectedItem is Incident selected)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить инцидент №{selected.Id}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        using var context = new PrikhoperskoeDbContext();
                        var toDelete = context.Incidents.Find(selected.Id);
                        if (toDelete != null)
                        {
                            context.Incidents.Remove(toDelete);
                            context.SaveChanges();
                            MessageBox.Show("Инцидент удален.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            BtnClear_Click(sender, e);
                            LoadData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите инцидент для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadData(txtSearch.Text);
        }
    }
}