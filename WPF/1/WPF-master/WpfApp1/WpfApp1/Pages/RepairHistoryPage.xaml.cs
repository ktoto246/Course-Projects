using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Models;
using ClosedXML.Excel;
using Microsoft.Win32;

namespace WpfApp1.Pages
{
    public partial class RepairHistoryPage : Page
    {
        private AppDbContext _context;
        private ICollectionView _repairsView;

        public RepairHistoryPage()
        {
            InitializeComponent();
            if (AppSession.CurrentRole == "Viewer")
            {
                AddPanel.Visibility = Visibility.Collapsed;
                BtnDelete.Visibility = Visibility.Collapsed;
                RepairsGrid.IsReadOnly = true;
            }
            else if (AppSession.CurrentRole == "Operator")
            {
                BtnDelete.Visibility = Visibility.Collapsed;
            }

            this.Loaded += (s, e) =>
            {
                try
                {
                    _context?.Dispose();
                    _context = new AppDbContext();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            this.Unloaded += (s, e) => _context?.Dispose();
        }

        private void LoadData()
        {
            cmbEquipment.ItemsSource = _context.Equipments.ToList();

            _context.RepairHistories
                .Include(r => r.Equipment)
                .Load();

            _repairsView = CollectionViewSource.GetDefaultView(_context.RepairHistories.Local.ToObservableCollection());
            _repairsView.Filter = FilterRepairs;
            RepairsGrid.ItemsSource = _repairsView;
        }

        private bool FilterRepairs(object item)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return true;

            if (item is RepairHistory r)
            {
                string search = txtSearch.Text.ToLower();
                return (r.Equipment?.Name?.ToLower().Contains(search) == true) ||
                       (r.IssueDescription?.ToLower().Contains(search) == true);
            }
            return false;
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            _repairsView?.Refresh();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cmbEquipment.SelectedItem == null || !dpDateIn.SelectedDate.HasValue || string.IsNullOrWhiteSpace(txtProblem.Text))
            {
                MessageBox.Show("Оборудование, дата сдачи и описание проблемы обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpDateOut.SelectedDate.HasValue && dpDateOut.SelectedDate.Value.Date < dpDateIn.SelectedDate.Value.Date)
            {
                MessageBox.Show("Дата возврата из ремонта не может быть раньше даты сдачи!", "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpDateIn.SelectedDate.HasValue && dpDateIn.SelectedDate.Value > DateTime.Today)
            {
                MessageBox.Show("Дата начала ремонта не может превышать текущую дату.",
                                "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int eqId = (int)cmbEquipment.SelectedValue;
            var equipmentCheck = _context.Equipments.Include(eq => eq.Status).FirstOrDefault(eq => eq.Id == eqId);

            if (equipmentCheck != null && equipmentCheck.Status?.Name == SystemStatuses.Scrapped)
            {
                MessageBox.Show("Нельзя отправлять в ремонт списанное оборудование! Оно уже снято с баланса.", "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal? cost = null;
            if (!string.IsNullOrWhiteSpace(txtCost.Text))
            {
                string priceInput = txtCost.Text.Replace(',', '.');
                if (decimal.TryParse(priceInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedPrice) && parsedPrice >= 0)
                {
                    cost = parsedPrice;
                }
                else
                {
                    MessageBox.Show("Некорректная стоимость.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var repair = new RepairHistory
            {
                EquipmentId = eqId,
                DateIn = dpDateIn.SelectedDate.Value,
                DateOut = dpDateOut.SelectedDate,
                IssueDescription = txtProblem.Text.Trim(),
                Cost = cost,
                Contractor = string.IsNullOrWhiteSpace(txtContractor.Text) ? null : txtContractor.Text.Trim()
            };

            try
            {
                _context.RepairHistories.Add(repair);

                var eqToUpdate = _context.Equipments.Find(eqId);
                if (eqToUpdate != null)
                {
                    if (repair.DateOut == null)
                    {
                        var repairStatus = _context.Statuses.FirstOrDefault(s => s.Name == SystemStatuses.InRepair);
                        if (repairStatus != null)
                        {
                            eqToUpdate.StatusId = repairStatus.Id;
                            eqToUpdate.EmployeeId = null;
                        }
                    }
                    else
                    {
                        string targetStatusName = eqToUpdate.EmployeeId != null ? SystemStatuses.InUse : SystemStatuses.OnStock;
                        var targetStatus = _context.Statuses.FirstOrDefault(s => s.Name == targetStatusName);
                        if (targetStatus != null)
                        {
                            eqToUpdate.StatusId = targetStatus.Id;
                        }
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return;
            }

            txtProblem.Clear();
            txtCost.Clear();
            dpDateIn.SelectedDate = null;
            dpDateOut.SelectedDate = null;
            cmbEquipment.SelectedItem = null;
            RepairsGrid.Items.Refresh();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (RepairsGrid.SelectedItem is RepairHistory selected)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить запись о ремонте?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        var eq = _context.Equipments.Find(selected.EquipmentId);

                        _context.RepairHistories.Remove(selected);
                        _context.SaveChanges();

                        bool hasOtherActiveRepairs = _context.RepairHistories
                            .Any(r => r.EquipmentId == eq.Id && r.DateOut == null);

                        if (eq != null && !hasOtherActiveRepairs)
                        {
                            string targetStatusName = eq.EmployeeId != null ? SystemStatuses.InUse : SystemStatuses.OnStock;
                            var targetStatus = _context.Statuses.FirstOrDefault(s => s.Name == targetStatusName);
                            if (targetStatus != null) eq.StatusId = targetStatus.Id;

                            _context.SaveChanges();
                        }

                        _repairsView?.Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
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
                    FileName = $"Ремонты_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Ремонты");

                        worksheet.Cell(1, 1).Value = "Оборудование";
                        worksheet.Cell(1, 2).Value = "Описание проблемы";
                        worksheet.Cell(1, 3).Value = "Дата сдачи";
                        worksheet.Cell(1, 4).Value = "Дата возврата";
                        worksheet.Cell(1, 5).Value = "Стоимость";
                        worksheet.Cell(1, 6).Value = "Исполнитель";

                        var headerRow = worksheet.Row(1);
                        headerRow.Style.Font.Bold = true;
                        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                        int row = 2;
                        foreach (RepairHistory rep in _repairsView)
                        {
                            worksheet.Cell(row, 1).Value = rep.Equipment?.Name;
                            worksheet.Cell(row, 2).Value = rep.IssueDescription;
                            worksheet.Cell(row, 3).Value = rep.DateIn.ToString("dd.MM.yyyy");
                            worksheet.Cell(row, 4).Value = rep.DateOut?.ToString("dd.MM.yyyy");
                            worksheet.Cell(row, 5).Value = rep.Cost;
                            worksheet.Cell(row, 6).Value = rep.Contractor;
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
