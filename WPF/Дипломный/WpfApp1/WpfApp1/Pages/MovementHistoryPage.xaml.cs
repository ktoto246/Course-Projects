using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Models;
using ClosedXML.Excel;
using Microsoft.Win32;

namespace WpfApp1.Pages
{
    public partial class MovementHistoryPage : Page
    {
        private AppDbContext _context;
        private ICollectionView _movementsView;

        public MovementHistoryPage()
        {
            InitializeComponent();
            if (AppSession.CurrentRole == "Viewer")
            {
                AddPanel.Visibility = Visibility.Collapsed;
                BtnDelete.Visibility = Visibility.Collapsed;
                MovementsGrid.IsReadOnly = true;
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
            var equipments = _context.Equipments.ToList();
            var employees = _context.Employees.ToList();
            employees.Insert(0, new Employee { Id = -1, FullName = "--- Внешний/Нет ---" });

            var cabinets = _context.Cabinets.ToList();

            cmbEquipment.ItemsSource = equipments;
            cmbFromEmployee.ItemsSource = employees;
            cmbToEmployee.ItemsSource = employees;
            cmbNewCabinet.ItemsSource = cabinets;

            colEquipment.ItemsSource = equipments;
            colFromEmployee.ItemsSource = employees;
            colToEmployee.ItemsSource = employees;

            cmbFromEmployee.SelectedIndex = 0;
            cmbToEmployee.SelectedIndex = 0;

            _context.MovementHistories
                .Include(m => m.Equipment)
                .Include(m => m.FromEmployee)
                .Include(m => m.ToEmployee)
                .Load();

            _movementsView = CollectionViewSource.GetDefaultView(_context.MovementHistories.Local.ToObservableCollection());
            _movementsView.Filter = FilterMovements;
            MovementsGrid.ItemsSource = _movementsView;
        }

        private bool FilterMovements(object item)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return true;

            if (item is MovementHistory m)
            {
                string search = txtSearch.Text.ToLower();
                return (m.Equipment?.Name?.ToLower().Contains(search) == true) ||
                       (m.FromEmployee?.FullName?.ToLower().Contains(search) == true) ||
                       (m.ToEmployee?.FullName?.ToLower().Contains(search) == true) ||
                       (m.Reason?.ToLower().Contains(search) == true);
            }
            return false;
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            _movementsView?.Refresh();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReason.Text))
            {
                MessageBox.Show("Поле 'Причина' обязательно для заполнения. Укажите документальное основание для перемещения оборудования (приказ, служебная записка или акт).",
                                "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (dpTransferDate.SelectedDate.HasValue && dpTransferDate.SelectedDate.Value > DateTime.Today)
            {
                MessageBox.Show("Дата перемещения не может быть позднее текущей даты.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbEquipment.SelectedItem == null || !dpTransferDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Оборудование и дата обязательны.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (cmbEquipment.SelectedValue == null || cmbFromEmployee.SelectedValue == null || cmbToEmployee.SelectedValue == null)
            {
                MessageBox.Show("Оборудование, поле «Передал» и поле «Принял» обязательны для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int eqId = (int)cmbEquipment.SelectedValue;
            int fromId = (int)cmbFromEmployee.SelectedValue;
            int toId = (int)cmbToEmployee.SelectedValue;

            if (fromId == toId)
            {
                MessageBox.Show("Сотрудник, передающий оборудование, и принимающий не могут совпадать.", "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var equipmentCheck = _context.Equipments
                .Include(eq => eq.Status)
                .Include(eq => eq.Employee)
                .FirstOrDefault(eq => eq.Id == eqId);

            if (equipmentCheck == null) return;

            if (equipmentCheck.Status?.Name == SystemStatuses.Scrapped)
            {
                MessageBox.Show("Нельзя передавать списанное оборудование! Оно уже снято с баланса.", "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (equipmentCheck.Status?.Name == SystemStatuses.InRepair)
            {
                MessageBox.Show("Невозможно выполнить перемещение: оборудование в данный момент находится на техническом обслуживании (в ремонте).",
                                "Ошибка логики", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int currentOwnerId = equipmentCheck.EmployeeId ?? -1;
            if (fromId != currentOwnerId)
            {
                string currentOwnerName = equipmentCheck.EmployeeId == null ? "Нет ответственного (Склад)" : equipmentCheck.Employee?.FullName ?? "Другой сотрудник";
                MessageBox.Show($"Нарушение цепочки ответственности!\nСейчас оборудование числится за: {currentOwnerName}.\nВ поле 'Передал' должен быть указан именно этот сотрудник (или склад).", "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var movement = new MovementHistory
            {
                EquipmentId = eqId,
                TransferDate = dpTransferDate.SelectedDate.Value,
                Reason = txtReason.Text.Trim()
            };

            if (fromId != -1) movement.FromEmployeeId = fromId;
            if (toId != -1) movement.ToEmployeeId = toId;

            try
            {
                _context.MovementHistories.Add(movement);

                var eq = _context.Equipments.Find(eqId);
                if (eq != null)
                {
                    if (toId != -1)
                    {
                        eq.EmployeeId = toId;
                        var inUse = _context.Statuses.FirstOrDefault(s => s.Name == SystemStatuses.InUse);
                        if (inUse != null) eq.StatusId = inUse.Id;
                    }
                    else
                    {
                        eq.EmployeeId = null;
                        var onStock = _context.Statuses.FirstOrDefault(s => s.Name == SystemStatuses.OnStock);
                        if (onStock != null) eq.StatusId = onStock.Id;
                    }

                    if (cmbNewCabinet.SelectedValue != null)
                    {
                        eq.CabinetId = (int)cmbNewCabinet.SelectedValue;
                    }
                    else eq.CabinetId = null;
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            txtReason.Clear();
            cmbNewCabinet.SelectedItem = null;
            dpTransferDate.SelectedDate = null;

            cmbEquipment.SelectedItem = null;
            cmbFromEmployee.SelectedIndex = 0;
            cmbToEmployee.SelectedIndex = 0;

            MovementsGrid.Items.Refresh();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MovementsGrid.SelectedItem is MovementHistory selectedMovement)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту запись о перемещении?\nЕсли это последнее перемещение, ответственный за оборудование будет восстановлен на предыдущего сотрудника.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var eq = _context.Equipments.Find(selectedMovement.EquipmentId);
                        if (eq != null)
                        {
                            var latestMovement = _context.MovementHistories
                                .Where(m => m.EquipmentId == selectedMovement.EquipmentId)
                                .OrderByDescending(m => m.TransferDate)
                                .ThenByDescending(m => m.Id)
                                .FirstOrDefault();

                            if (latestMovement != null && latestMovement.Id == selectedMovement.Id)
                            {
                                eq.EmployeeId = selectedMovement.FromEmployeeId;
                                eq.CabinetId = null;
                                string statusName = eq.EmployeeId != null ? SystemStatuses.InUse : SystemStatuses.OnStock;
                                var status = _context.Statuses.FirstOrDefault(s => s.Name == statusName);
                                if (status != null) eq.StatusId = status.Id;
                            }
                        }
                        _context.MovementHistories.Remove(selectedMovement);
                        _context.SaveChanges();

                        MovementsGrid.Items.Refresh();
                        MessageBox.Show("Запись успешно удалена, данные синхронизированы.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите запись для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    FileName = $"Перемещения_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Перемещения");

                        worksheet.Cell(1, 1).Value = "Оборудование";
                        worksheet.Cell(1, 2).Value = "Передал";
                        worksheet.Cell(1, 3).Value = "Принял";
                        worksheet.Cell(1, 4).Value = "Дата";
                        worksheet.Cell(1, 5).Value = "Причина";

                        var headerRow = worksheet.Row(1);
                        headerRow.Style.Font.Bold = true;
                        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                        int row = 2;
                        foreach (MovementHistory m in _movementsView)
                        {
                            worksheet.Cell(row, 1).Value = $"[{m.Equipment?.InventoryNumber}] {m.Equipment?.Name}";
                            worksheet.Cell(row, 2).Value = m.FromEmployee?.FullName;
                            worksheet.Cell(row, 3).Value = m.ToEmployee?.FullName;
                            worksheet.Cell(row, 4).Value = m.TransferDate.ToString("dd.MM.yyyy");
                            worksheet.Cell(row, 5).Value = m.Reason;
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