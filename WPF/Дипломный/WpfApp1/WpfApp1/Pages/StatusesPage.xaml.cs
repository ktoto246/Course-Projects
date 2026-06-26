using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Models;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;

namespace WpfApp1.Pages
{
    public partial class StatusesPage : Page
    {
        private AppDbContext _context;
        private ICollectionView _statusesView;

        public StatusesPage()
        {
            InitializeComponent();
            if (AppSession.CurrentRole == "Viewer" || AppSession.CurrentRole == "Operator")
            {
                AddPanel.Visibility = Visibility.Collapsed;
                BtnDelete.Visibility = Visibility.Collapsed;
                BtnSave.Visibility = Visibility.Collapsed;
                StatusesGrid.IsReadOnly = true;
            }

            try
            {
                _context = new AppDbContext();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Unloaded += (s, e) => _context?.Dispose();
        }

        private void LoadData()
        {
            _context.Statuses.Load();
            _statusesView = CollectionViewSource.GetDefaultView(_context.Statuses.Local.ToObservableCollection());
            _statusesView.Filter = FilterStatuses;
            StatusesGrid.ItemsSource = _statusesView;
        }

        private bool FilterStatuses(object item)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return true;

            if (item is Status s)
            {
                return s.Name.ToLower().Contains(txtSearch.Text.ToLower());
            }
            return false;
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            _statusesView?.Refresh();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string newStatusName = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(newStatusName))
            {
                MessageBox.Show("Пожалуйста, введите название нового статуса.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_context.Statuses.Any(s => s.Name.ToLower() == newStatusName.ToLower()))
            {
                MessageBox.Show($"Статус '{newStatusName}' уже существует в базе!", "Ошибка уникальности", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newStatus = new Status { Name = newStatusName };

            try
            {
                _context.Statuses.Add(newStatus);
                _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            txtName.Clear();
            StatusesGrid.Items.Refresh();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (StatusesGrid.SelectedItem is Status selectedStatus)
            {
                string[] systemStatuses = {
            SystemStatuses.InUse,
            SystemStatuses.OnStock,
            SystemStatuses.InRepair,
            SystemStatuses.Scrapped
        };

                if (systemStatuses.Contains(selectedStatus.Name))
                {
                    MessageBox.Show($"Системный статус '{selectedStatus.Name}' нельзя удалить. Он намертво зашит в логику приложения!",
                                    "Критическая ошибка",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                    return;
                }

                if (_context.Equipments.Any(eq => eq.StatusId == selectedStatus.Id))
                {
                    MessageBox.Show("Нельзя удалить этот статус, пока он назначен оборудованию в базе данных!",
                                    "Ошибка валидации",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Вы уверены, что хотите удалить статус '{selectedStatus.Name}'?",
                                             "Подтверждение удаления",
                                             MessageBoxButton.YesNo,
                                             MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Statuses.Remove(selectedStatus);
                        _context.SaveChanges();
                        LoadData();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении из базы данных: {ex.Message}",
                                        "Ошибка БД",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите статус для удаления из списка.",
                                "Внимание",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dbNames = _context.Statuses.Local.Select(s => s.Name).ToList();
                string[] required = { SystemStatuses.InUse, SystemStatuses.OnStock, SystemStatuses.InRepair, SystemStatuses.Scrapped };

                foreach (var req in required)
                {
                    if (!dbNames.Contains(req))
                    {
                        MessageBox.Show($"Ошибка: системный статус '{req}' не найден. Переименование базовых статусов запрещено для сохранения логики системы.",
                                        "Нарушение целостности", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                _context.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Сохранить отчет",
                    FileName = $"Статусы_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Статусы");

                        worksheet.Cell(1, 1).Value = "Код";
                        worksheet.Cell(1, 2).Value = "Наименование";

                        var headerRow = worksheet.Row(1);
                        headerRow.Style.Font.Bold = true;
                        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                        int row = 2;
                        foreach (Status s in _statusesView)
                        {
                            worksheet.Cell(row, 1).Value = s.Id;
                            worksheet.Cell(row, 2).Value = s.Name;
                            row++;
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveFileDialog.FileName);
                    }
                    MessageBox.Show("Отчет успешно сохранен в Excel!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
