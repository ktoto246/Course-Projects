using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class WorkLogsPage : Page
    {
        private int? _currentWorkLogId;

        public WorkLogsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string searchQuery = null)
        {
            try
            {
                using var context = new PrikhoperskoeDbContext();

                cboIncident.ItemsSource = context.Incidents.OrderByDescending(i => i.CreatedAt).ToList();
                cboBrigade.ItemsSource = context.Brigades.OrderBy(b => b.CallSign).ToList();

                IQueryable<WorkLog> query = context.WorkLogs
                    .Include(w => w.Incident)
                    .Include(w => w.Brigade);

                if (!string.IsNullOrWhiteSpace(searchQuery))
                {
                    query = query.Where(w => w.Brigade.CallSign.Contains(searchQuery) ||
                                             w.Brigade.LeaderName.Contains(searchQuery) ||
                                             w.Incident.Description.Contains(searchQuery));
                }

                dgWorkLogs.ItemsSource = query.OrderByDescending(w => w.DepartureTime).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgWorkLogs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgWorkLogs.SelectedItem is WorkLog selected)
            {
                _currentWorkLogId = selected.Id;
                cboIncident.SelectedItem = selected.Incident;
                cboBrigade.SelectedItem = selected.Brigade;
                dpDeparture.SelectedDate = selected.DepartureTime;
                dpFix.SelectedDate = selected.FixTime;
            }
            else
            {
                _currentWorkLogId = null;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedIncident = cboIncident.SelectedItem as Incident;
                var selectedBrigade = cboBrigade.SelectedItem as Brigade;

                if (selectedIncident == null)
                {
                    MessageBox.Show("Выберите инцидент.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (selectedBrigade == null)
                {
                    MessageBox.Show("Выберите бригаду.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (dpDeparture.SelectedDate == null)
                {
                    MessageBox.Show("Укажите время выезда.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                using var context = new PrikhoperskoeDbContext();

                if (_currentWorkLogId.HasValue)
                {
                    var existing = context.WorkLogs.Find(_currentWorkLogId.Value);
                    if (existing != null)
                    {
                        existing.IncidentId = selectedIncident.Id;
                        existing.BrigadeId = selectedBrigade.Id;
                        existing.DepartureTime = dpDeparture.SelectedDate.Value;
                        existing.FixTime = dpFix.SelectedDate;
                    }
                }
                else
                {
                    var newWorkLog = new WorkLog
                    {
                        IncidentId = selectedIncident.Id,
                        BrigadeId = selectedBrigade.Id,
                        DepartureTime = dpDeparture.SelectedDate.Value,
                        FixTime = dpFix.SelectedDate
                    };
                    context.WorkLogs.Add(newWorkLog);
                }

                context.SaveChanges();
                MessageBox.Show("Запись в журнале успешно сохранена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
            _currentWorkLogId = null;
            dgWorkLogs.SelectedItem = null;
            cboIncident.SelectedIndex = -1;
            cboBrigade.SelectedIndex = -1;
            dpDeparture.SelectedDate = null;
            dpFix.SelectedDate = null;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgWorkLogs.SelectedItem is WorkLog selected)
                {
                    var result = MessageBox.Show($"Вы уверены, что хотите удалить запись журнала работ №{selected.Id}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        using var context = new PrikhoperskoeDbContext();
                        var toDelete = context.WorkLogs.Find(selected.Id);
                        if (toDelete != null)
                        {
                            context.WorkLogs.Remove(toDelete);
                            context.SaveChanges();
                            MessageBox.Show("Запись удалена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            BtnClear_Click(sender, e);
                            LoadData();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите запись для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
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