using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Helpers;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class RenderedServicesPage : Page
    {
        private AppDbContext _db;
        private Service _selectedService;

        public RenderedServicesPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            cmbBatch.SelectedValuePath = "Id";
            LoadData();
        }

        private void LoadData()
        {
            var batchItems = _db.GrainBatches
                .Where(b => b.Status != "Завершено")
                .Select(b => new { Id = b.Id, DisplayText = $"#{b.Id} | {b.CarNumber} | {b.Status}" })
                .ToList();

            cmbBatch.ItemsSource = batchItems;
            cmbService.ItemsSource = _db.Services.ToList();

            var clients = _db.Clients.ToList();
            clients.Insert(0, new Client { Id = 0, CompanyName = "Все клиенты" });
            cmbClientFilter.ItemsSource = clients;
            cmbClientFilter.SelectedIndex = 0;

            if (CurrentSession.CurrentUser.Role == "Администратор")
            {
                dgRenderedServices.IsReadOnly = true;
                btnDeleteService.Visibility = Visibility.Collapsed;
                btnAddService.Visibility = Visibility.Collapsed;
                btnSaveChanges.Visibility = Visibility.Collapsed;
            }

            ApplyFilters(null, null);
        }

        private void ApplyFilters(object sender, RoutedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            int selectedClientId = cmbClientFilter.SelectedItem is Client c ? c.Id : 0;

            var query = _db.RenderedServices
                .Include(rs => rs.Batch).ThenInclude(b => b.Client)
                .Include(rs => rs.Service)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(rs => rs.Batch.CarNumber.ToLower().Contains(searchText));

            if (selectedClientId > 0)
                query = query.Where(rs => rs.Batch.ClientId == selectedClientId);

            dgRenderedServices.ItemsSource = query.OrderByDescending(rs => rs.RecordDate).ToList();
        }

        private void btnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cmbClientFilter.SelectedIndex = 0;
        }

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_selectedService != null && decimal.TryParse(txtQuantity.Text?.Replace('.', ','), out decimal quantity) && quantity >= 0)
            {
                txtTotalPrice.Text = (quantity * _selectedService.UnitPrice).ToString("F2");
            }
            else
            {
                txtTotalPrice.Text = "0,00";
            }
        }

        private void cmbService_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedService = cmbService.SelectedItem as Service;

            if (_selectedService != null)
            {
                if (_selectedService.Name.ToLower().Contains("хранен"))
                {
                    lblQuantity.Text = "Кол-во дней";
                    if (cmbBatch.SelectedValue != null)
                    {
                        int batchId = (int)cmbBatch.SelectedValue;
                        var batch = _db.GrainBatches.Find(batchId);
                        if (batch != null)
                        {
                            int days = (int)(DateTime.Today - batch.ArrivalDate.Date).TotalDays;
                            if (days <= 0) days = 1;
                            txtQuantity.Text = days.ToString();
                        }
                    }
                }
                else
                {
                    lblQuantity.Text = "Количество";
                }
            }
            txtQuantity_TextChanged(null, null);
        }

        private void btnAddService_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBatch.SelectedValue == null || _selectedService == null)
            {
                MessageBox.Show("Выберите партию и услугу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtQuantity.Text?.Replace('.', ','), out decimal quantity) || quantity <= 0)
            {
                MessageBox.Show("Количество должно быть > 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedBatchId = (int)cmbBatch.SelectedValue;

            bool duplicate = _db.RenderedServices.Any(rs => rs.BatchId == selectedBatchId && rs.ServiceId == _selectedService.Id);
            if (duplicate)
            {
                MessageBox.Show($"Услуга '{_selectedService.Name}' уже оказана этой партии.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newService = new RenderedService
            {
                BatchId = selectedBatchId,
                ServiceId = _selectedService.Id,
                ManagerId = CurrentSession.CurrentUser.Id,
                Quantity = quantity,
                TotalPrice = quantity * _selectedService.UnitPrice,
                RecordDate = DateTime.Now
            };

            try
            {
                _db.RenderedServices.Add(newService);

                var batchToUpdate = _db.GrainBatches.Find(selectedBatchId);
                if (batchToUpdate != null)
                {
                    batchToUpdate.Status = "Завершено";
                }

                _db.SaveChanges();

                cmbBatch.SelectedIndex = -1;
                cmbService.SelectedIndex = -1;
                txtQuantity.Clear();
                txtTotalPrice.Clear();
                _selectedService = null;
                txtSearch.Clear();

                LoadData();
                MessageBox.Show("Услуга оформлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception)
            {
                _db.ChangeTracker.Clear();
                LoadData();
                MessageBox.Show("Ошибка при добавлении.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            dgRenderedServices.CommitEdit(DataGridEditingUnit.Row, true);

            var changedServices = _db.ChangeTracker.Entries<RenderedService>()
                .Where(x => x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .ToList();

            foreach (var service in changedServices)
            {
                if (service.Quantity <= 0)
                {
                    MessageBox.Show($"Кол-во (ID {service.Id}) должно быть > 0. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                var svc = _db.Services.Find(service.ServiceId);
                if (svc != null)
                {
                    service.TotalPrice = service.Quantity * svc.UnitPrice;
                }
            }

            try
            {
                _db.SaveChanges();
                LoadData();
                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                _db.ChangeTracker.Clear();
                LoadData();
            }
        }

        private void btnDeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (dgRenderedServices.SelectedItem is RenderedService selectedService)
            {
                string serviceName = selectedService.Service?.Name ?? "Выбранную услугу";

                if (MessageBox.Show($"Удалить услугу '{serviceName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int currentBatchId = selectedService.BatchId;

                    _db.RenderedServices.Remove(selectedService);
                    _db.SaveChanges();

                    bool hasOtherServices = _db.RenderedServices.Any(rs => rs.BatchId == currentBatchId);
                    if (!hasOtherServices)
                    {
                        var batchToRollback = _db.GrainBatches.Find(currentBatchId);
                        if (batchToRollback != null)
                        {
                            bool hasLabTest = _db.LabTests.Any(t => t.BatchId == currentBatchId);
                            batchToRollback.Status = hasLabTest ? "Проведен анализ" : "Ожидает анализа";
                            _db.SaveChanges();
                        }
                    }

                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel Workbook|*.xlsx" };
            if (sfd.ShowDialog() == true)
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Оказанные_услуги");
                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Дата";
                    worksheet.Cells[1, 3].Value = "Клиент";
                    worksheet.Cells[1, 4].Value = "Авто";
                    worksheet.Cells[1, 5].Value = "Услуга";
                    worksheet.Cells[1, 6].Value = "Кол-во";
                    worksheet.Cells[1, 7].Value = "Итого";
                    worksheet.Cells[1, 8].Value = "Менеджер";

                    var services = _db.RenderedServices
                        .Include(rs => rs.Batch).ThenInclude(b => b.Client)
                        .Include(rs => rs.Service)
                        .Include(rs => rs.Manager)
                        .OrderByDescending(rs => rs.RecordDate)
                        .ToList();

                    for (int i = 0; i < services.Count; i++)
                    {
                        var s = services[i];
                        worksheet.Cells[i + 2, 1].Value = s.Id;
                        worksheet.Cells[i + 2, 2].Value = s.RecordDate.ToString("dd.MM.yyyy");
                        worksheet.Cells[i + 2, 3].Value = s.Batch?.Client?.CompanyName;
                        worksheet.Cells[i + 2, 4].Value = s.Batch?.CarNumber;
                        worksheet.Cells[i + 2, 5].Value = s.Service?.Name;
                        worksheet.Cells[i + 2, 6].Value = s.Quantity;
                        worksheet.Cells[i + 2, 7].Value = s.TotalPrice;
                        worksheet.Cells[i + 2, 8].Value = s.Manager?.FullName;
                    }
                    File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                    MessageBox.Show("Успешно выгружено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}