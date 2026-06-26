using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class BrigadesPage : Page
    {
        private int? _currentBrigadeId;

        public BrigadesPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string searchQuery = null)
        {
            try
            {
                using var context = new PrikhoperskoeDbContext();

                var districts = context.Districts.OrderBy(d => d.Name).ToList();
                cboDistrict.ItemsSource = districts;

                IQueryable<Brigade> query = context.Brigades.Include(b => b.District);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(b => b.CallSign.Contains(searchQuery) ||
                                             b.LeaderName.Contains(searchQuery) ||
                                             b.District.Name.Contains(searchQuery));
                }

                dgBrigades.ItemsSource = query.OrderBy(b => b.DistrictId).ThenBy(b => b.CallSign).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgBrigades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgBrigades.SelectedItem is Brigade selected)
            {
                _currentBrigadeId = selected.Id;
                cboDistrict.SelectedValue = selected.DistrictId;
                txtCallSign.Text = selected.CallSign;
                txtLeaderName.Text = selected.LeaderName;
                LoadWorkers();
            }
            else
            {
                dgWorkers.ItemsSource = null;
            }
        }

        private void LoadWorkers()
        {
            if (_currentBrigadeId.HasValue)
            {
                try
                {
                    using var context = new PrikhoperskoeDbContext();
                    dgWorkers.ItemsSource = context.Workers
                        .Where(w => w.BrigadeId == _currentBrigadeId.Value)
                        .ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке состава: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                dgWorkers.ItemsSource = null;
            }
        }

        private void BtnAddWorker_Click(object sender, RoutedEventArgs e)
        {
            if (!_currentBrigadeId.HasValue)
            {
                MessageBox.Show("Выберите бригаду из списка слева.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtWorkerName.Text) || string.IsNullOrWhiteSpace(txtWorkerPosition.Text))
            {
                MessageBox.Show("Заполните ФИО и должность рабочего.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var context = new PrikhoperskoeDbContext();
                var worker = new Worker
                {
                    BrigadeId = _currentBrigadeId.Value,
                    FullName = txtWorkerName.Text.Trim(),
                    Position = txtWorkerPosition.Text.Trim()
                };

                context.Workers.Add(worker);
                context.SaveChanges();

                txtWorkerName.Clear();
                txtWorkerPosition.Clear();

                LoadWorkers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteWorker_Click(object sender, RoutedEventArgs e)
        {
            if (dgWorkers.SelectedItem is Worker selectedWorker)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить рабочего {selectedWorker.FullName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var context = new PrikhoperskoeDbContext();
                        var workerToDelete = context.Workers.Find(selectedWorker.Id);
                        if (workerToDelete != null)
                        {
                            context.Workers.Remove(workerToDelete);
                            context.SaveChanges();
                            LoadWorkers();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите рабочего в таблице состава.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedDistrictId = cboDistrict.SelectedValue as int?;
                if (selectedDistrictId == null)
                {
                    MessageBox.Show("Выберите район.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtCallSign.Text))
                {
                    MessageBox.Show("Введите позывной.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtLeaderName.Text))
                {
                    MessageBox.Show("Введите ФИО руководителя.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var context = new PrikhoperskoeDbContext();

                if (_currentBrigadeId.HasValue)
                {
                    var existing = context.Brigades.Find(_currentBrigadeId.Value);
                    if (existing != null)
                    {
                        existing.DistrictId = selectedDistrictId.Value;
                        existing.CallSign = txtCallSign.Text.Trim();
                        existing.LeaderName = txtLeaderName.Text.Trim();
                    }
                }
                else
                {
                    var newBrigade = new Brigade
                    {
                        DistrictId = selectedDistrictId.Value,
                        CallSign = txtCallSign.Text.Trim(),
                        LeaderName = txtLeaderName.Text.Trim()
                    };
                    context.Brigades.Add(newBrigade);
                }

                int saved = context.SaveChanges();
                MessageBox.Show($"Данные успешно сохранены. Изменено записей: {saved}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                BtnClear_Click(sender, e);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}\n\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            _currentBrigadeId = null;
            dgBrigades.SelectedItem = null;
            dgWorkers.ItemsSource = null;
            cboDistrict.SelectedIndex = -1;
            txtCallSign.Clear();
            txtLeaderName.Clear();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgBrigades.SelectedItem is Brigade selected)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить бригаду '{selected.CallSign}'? Все ее рабочие также будут удалены.", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        using var context = new PrikhoperskoeDbContext();
                        var toDelete = context.Brigades.Find(selected.Id);
                        if (toDelete != null)
                        {
                            context.Brigades.Remove(toDelete);
                            context.SaveChanges();
                            MessageBox.Show("Бригада успешно удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            BtnClear_Click(sender, e);
                            LoadData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите бригаду для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка удаления: {ex.Message}\n\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoadData(txtSearch.Text);
        }
    }
}