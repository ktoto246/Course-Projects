using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.ComponentModel;
using System.Globalization;
using WpfApp1.Models;
using ClosedXML.Excel;
using Microsoft.Win32;

namespace WpfApp1.Pages
{
    public partial class EquipmentsPage : Page
    {
        private AppDbContext _context;
        private ICollectionView _equipmentsView;

        public EquipmentsPage()
        {
            InitializeComponent();

            if (AppSession.CurrentRole == "Viewer")
            {
                AddPanel.Visibility = Visibility.Collapsed;
                BtnDelete.Visibility = Visibility.Collapsed;
                BtnSave.Visibility = Visibility.Collapsed;
                EquipmentsGrid.IsReadOnly = true;
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
                    MessageBox.Show($"Ошибка контекста: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
            this.Unloaded += (s, e) => _context?.Dispose();
        }

        private void LoadData()
        {
            var categories = _context.Categories.ToList();
            var statuses = _context.Statuses.ToList();
            var employees = _context.Employees.ToList();
            var cabinets = _context.Cabinets.ToList();

            var noEmployee = new Employee { Id = -1, FullName = "--- Нет ответственного ---" };
            employees.Insert(0, noEmployee);

            var filterCategories = categories.ToList();
            filterCategories.Insert(0, new Category { Id = -1, Name = "--- Все ---" });
            cmbFilterCategory.ItemsSource = filterCategories;
            cmbFilterCategory.SelectedIndex = 0;

            var filterStatuses = statuses.ToList();
            filterStatuses.Insert(0, new Status { Id = -1, Name = "--- Все ---" });
            cmbFilterStatus.ItemsSource = filterStatuses;
            cmbFilterStatus.SelectedIndex = 0;

            cmbCategory.ItemsSource = categories;
            cmbStatus.ItemsSource = statuses;
            cmbEmployee.ItemsSource = employees;
            cmbCabinet.ItemsSource = cabinets;

            colCategory.ItemsSource = categories;
            colStatus.ItemsSource = statuses;
            colEmployee.ItemsSource = employees;

            _context.Equipments
                 .Include(e => e.Category)
                 .Include(e => e.Status)
                 .Include(e => e.Employee)
                 .Include(e => e.Cabinet) 
                 .Include(e => e.RepairHistories)
                 .Load();

            _equipmentsView = CollectionViewSource.GetDefaultView(_context.Equipments.Local.ToObservableCollection());
            _equipmentsView.Filter = FilterEquipments;
            EquipmentsGrid.ItemsSource = _equipmentsView;
        }

        private bool FilterEquipments(object item)
        {
            if (item is Equipment eq)
            {
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string search = txtSearch.Text.ToLower();

                    bool matchName = eq.Name.ToLower().Contains(search);
                    bool matchInv = eq.InventoryNumber.ToLower().Contains(search);
                    bool matchSerial = eq.SerialNumber != null && eq.SerialNumber.ToLower().Contains(search);
                    bool matchLocation = eq.Cabinet != null && eq.Cabinet.Number.ToLower().Contains(search);
                    bool matchEmployee = eq.Employee != null && eq.Employee.FullName != null && eq.Employee.FullName.ToLower().Contains(search);
                    bool matchSupplier = eq.Supplier != null && eq.Supplier.ToLower().Contains(search);

                    if (!matchName && !matchInv && !matchSerial && !matchLocation && !matchEmployee && !matchSupplier)
                        return false;
                }

                if (cmbFilterCategory.SelectedValue != null && (int)cmbFilterCategory.SelectedValue != -1)
                {
                    if (eq.CategoryId != (int)cmbFilterCategory.SelectedValue)
                        return false;
                }

                if (cmbFilterStatus.SelectedValue != null && (int)cmbFilterStatus.SelectedValue != -1)
                {
                    if (eq.StatusId != (int)cmbFilterStatus.SelectedValue)
                        return false;
                }

                return true;
            }
            return false;
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            _equipmentsView?.Refresh();
        }

        private void BtnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = string.Empty;
            cmbFilterCategory.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndex = 0;
            _equipmentsView?.Refresh();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dpPurchaseDate.SelectedDate.HasValue && dpPurchaseDate.SelectedDate.Value > DateTime.Today)
            {
                MessageBox.Show("Дата покупки не может быть в будущем!", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtInventoryNumber.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Инвентарный номер и наименование обязательны для заполнения.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cmbCategory.SelectedItem == null || cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Обязательно выберите категорию и статус из выпадающего списка.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? serviceLifeYears = null;
            if (!string.IsNullOrWhiteSpace(txtServiceLife.Text))
            {
                if (!int.TryParse(txtServiceLife.Text, out int sl) || sl <= 0)
                {
                    MessageBox.Show("Срок службы должен быть положительным числом (в годах).", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                serviceLifeYears = sl;
            }

            var selectedStatus = cmbStatus.SelectedItem as Status;
            int empId = cmbEmployee.SelectedItem != null ? (int)cmbEmployee.SelectedValue : -1;
            int? cabId = cmbCabinet.SelectedValue != null ? (int?)cmbCabinet.SelectedValue : null;

            if (selectedStatus != null)
            {
                if (selectedStatus.Name == SystemStatuses.InUse && empId == -1)
                {
                    MessageBox.Show("Для статуса В эксплуатации необходимо указать ответственного сотрудника.", "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string[] noOwnerStatuses = { SystemStatuses.OnStock, SystemStatuses.InRepair, SystemStatuses.Scrapped };
                if (noOwnerStatuses.Contains(selectedStatus.Name) && empId != -1)
                {
                    MessageBox.Show($"Если статус '{selectedStatus.Name}', ответственного быть не может. Сначала снимите человека с оборудования (выберите '--- Нет ответственного ---').", "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            decimal? price = null;
            if (!string.IsNullOrWhiteSpace(txtPrice.Text))
            {
                string priceInput = txtPrice.Text.Replace(',', '.');
                if (decimal.TryParse(priceInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal parsedPrice) && parsedPrice >= 0)
                {
                    price = parsedPrice;
                }
                else
                {
                    MessageBox.Show("Некорректный формат цены. Цена не может быть отрицательной.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            string invNum = txtInventoryNumber.Text.Trim();
            if (_context.Equipments.Any(e => e.InventoryNumber == invNum))
            {
                MessageBox.Show($"Оборудование с инвентарным номером '{invNum}' уже существует в базе.", "Ошибка уникальности", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newEquipment = new Equipment
            {
                InventoryNumber = invNum,
                Name = txtName.Text.Trim(),
                SerialNumber = string.IsNullOrWhiteSpace(txtSerialNumber.Text) ? null : txtSerialNumber.Text.Trim(),
                Price = price,
                PurchaseDate = dpPurchaseDate.SelectedDate,
                CabinetId = cabId,
                Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? null : txtNotes.Text.Trim(),

                CategoryId = (int)cmbCategory.SelectedValue,
                StatusId = (int)cmbStatus.SelectedValue,
                EmployeeId = empId != -1 ? empId : (int?)null,
                WarrantyExpireDate = dpWarranty.SelectedDate,
                Supplier = string.IsNullOrWhiteSpace(txtSupplier.Text) ? null : txtSupplier.Text.Trim(),
                ServiceLifeYears = serviceLifeYears
            };


            try
            {
                if (AppSession.CurrentRole == "Operator" && selectedStatus?.Name == SystemStatuses.Scrapped)
                {
                    MessageBox.Show("Оператор не имеет права регистрировать оборудование со статусом 'Списано'.",
                                    "Нарушение прав доступа", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _context.Equipments.Add(newEquipment);
                _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            txtInventoryNumber.Clear();
            txtName.Clear();
            txtSerialNumber.Clear();
            txtPrice.Clear();
            dpPurchaseDate.SelectedDate = null;
            cmbCategory.SelectedItem = null;
            cmbStatus.SelectedItem = null;
            cmbEmployee.SelectedItem = null;
            cmbCabinet.SelectedItem = null;
            txtNotes.Clear();
            EquipmentsGrid.Items.Refresh();
            txtSupplier.Clear();
            txtServiceLife.Clear();
            dpWarranty.SelectedDate = null;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (EquipmentsGrid.SelectedItem is Equipment selectedEquipment)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите СПИСАТЬ '{selectedEquipment.Name}'?\nЗапись останется в архиве, но статус сменится на '{SystemStatuses.Scrapped}', а ответственный и кабинет будут сняты.",
                                     "Подтверждение списания", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var scrappedStatus = _context.Statuses.FirstOrDefault(s => s.Name == SystemStatuses.Scrapped);

                        if (scrappedStatus != null)
                        {
                            selectedEquipment.StatusId = scrappedStatus.Id;
                            selectedEquipment.EmployeeId = null;
                            selectedEquipment.CabinetId = null;

                            _context.SaveChanges();
                            MessageBox.Show("Оборудование успешно списано.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            _equipmentsView?.Refresh();
                        }
                        else
                        {
                            MessageBox.Show($"Ошибка: статус '{SystemStatuses.Scrapped}' не найден в справочнике!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при списании: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите оборудование для списания.");
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var localData = _context.Equipments.Local.ToList();
                var allStatuses = _context.Statuses.ToList();

                foreach (var eq in localData)
                {
                    if (eq.ServiceLifeYears.HasValue && eq.ServiceLifeYears.Value <= 0)
                    {
                        MessageBox.Show($"Ошибка в строке {eq.InventoryNumber}: Срок службы должен быть больше нуля.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (eq.Price.HasValue && eq.Price.Value < 0)
                    {
                        MessageBox.Show($"Ошибка в строке {eq.InventoryNumber}: Цена не может быть отрицательной.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    if (eq.PurchaseDate.HasValue && eq.PurchaseDate.Value > DateTime.Today)
                    {
                        MessageBox.Show($"Ошибка в строке {eq.InventoryNumber}: Дата покупки не может быть в будущем.",
                                        "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var statusName = eq.Status?.Name ?? allStatuses.FirstOrDefault(s => s.Id == eq.StatusId)?.Name;

                    if (statusName != null)
                    {
                        if (statusName == SystemStatuses.Scrapped && AppSession.CurrentRole == "Operator")
                        {
                            var entry = _context.Entry(eq);

                            bool wasAlreadyScrapped = entry.State != EntityState.Modified ||
                                                      (int)entry.OriginalValues[nameof(eq.StatusId)] == eq.StatusId;

                            if (!wasAlreadyScrapped)
                            {
                                MessageBox.Show($"Оператор не может списывать оборудование ({eq.InventoryNumber}). Операция заблокирована.",
                                                "Нарушение прав доступа", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }
                        }

                        bool willHaveNoEmployee = eq.EmployeeId == null || eq.EmployeeId == -1;

                        if (statusName == SystemStatuses.InUse && willHaveNoEmployee)
                        {
                            MessageBox.Show($"Ошибка в строке {eq.InventoryNumber}: Оборудование 'В эксплуатации' должно быть закреплено за сотрудником!",
                                            "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        string[] restrictedStatuses = { SystemStatuses.OnStock, SystemStatuses.InRepair, SystemStatuses.Scrapped };
                        if (restrictedStatuses.Contains(statusName) && !willHaveNoEmployee)
                        {
                            MessageBox.Show($"Ошибка в строке {eq.InventoryNumber}: Статус '{statusName}' не подразумевает ответственного сотрудника. Снимите закрепление перед сохранением.",
                                            "Логическая ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }
                    }
                }

                var duplicateGroups = localData.GroupBy(e => e.InventoryNumber).Where(g => g.Count() > 1);

                if (duplicateGroups.Any())
                {
                    var firstDup = duplicateGroups.First();
                    MessageBox.Show($"Обнаружены дубликаты инвентарного номера: '{firstDup.Key}'. Исправьте их вручную в таблице перед сохранением.",
                                    "Ошибка уникальности", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                foreach (var eq in localData)
                {
                    if (eq.EmployeeId == -1)
                    {
                        eq.EmployeeId = null;
                        eq.Employee = null;
                    }
                }

                _context.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                _equipmentsView?.Refresh();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    FileName = $"Оборудование_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Оборудование");

                        worksheet.Cell(1, 1).Value = "Инв. №";
                        worksheet.Cell(1, 2).Value = "Наименование";
                        worksheet.Cell(1, 3).Value = "Серийный №";
                        worksheet.Cell(1, 4).Value = "Категория";
                        worksheet.Cell(1, 5).Value = "Статус";
                        worksheet.Cell(1, 6).Value = "Ответственный";
                        worksheet.Cell(1, 7).Value = "Кабинет";
                        worksheet.Cell(1, 8).Value = "Цена";
                        worksheet.Cell(1, 9).Value = "Остаточная стоимость";
                        worksheet.Cell(1, 10).Value = "Дата покупки";
                        worksheet.Cell(1, 11).Value = "Поставщик";
                        worksheet.Cell(1, 12).Value = "Гарантия до";
                        worksheet.Cell(1, 13).Value = "Сумма ремонтов";
                        worksheet.Cell(1, 14).Value = "Примечание";

                        var headerRow = worksheet.Row(1);
                        headerRow.Style.Font.Bold = true;
                        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                        int row = 2;
                        foreach (Equipment eq in _equipmentsView)
                        {
                            worksheet.Cell(row, 1).Value = eq.InventoryNumber;
                            worksheet.Cell(row, 2).Value = eq.Name;
                            worksheet.Cell(row, 3).Value = eq.SerialNumber;
                            worksheet.Cell(row, 4).Value = eq.Category?.Name;
                            worksheet.Cell(row, 5).Value = eq.Status?.Name;
                            worksheet.Cell(row, 6).Value = eq.Employee?.FullName;
                            worksheet.Cell(row, 7).Value = eq.Cabinet?.Number;
                            worksheet.Cell(row, 8).Value = eq.Price;
                            worksheet.Cell(row, 9).Value = eq.CurrentAmortizedValue;
                            if (eq.PurchaseDate.HasValue)
                                worksheet.Cell(row, 10).Value = eq.PurchaseDate.Value.ToString("dd.MM.yyyy");
                            worksheet.Cell(row, 11).Value = eq.Supplier;
                            if (eq.WarrantyExpireDate.HasValue)
                                worksheet.Cell(row, 12).Value = eq.WarrantyExpireDate.Value.ToString("dd.MM.yyyy");
                            worksheet.Cell(row, 13).Value = eq.TotalRepairCost;
                            worksheet.Cell(row, 14).Value = eq.Notes;

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