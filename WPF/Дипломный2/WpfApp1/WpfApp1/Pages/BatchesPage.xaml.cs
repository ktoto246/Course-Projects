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
    public partial class BatchesPage : Page
    {
        private AppDbContext _db;

        public BatchesPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            cmbClient.ItemsSource = _db.Clients.ToList();
            cmbCrop.ItemsSource = _db.Crops.ToList();
            cmbStorage.ItemsSource = _db.Storages.ToList();

            dgBatches.ItemsSource = _db.GrainBatches
                .Include(b => b.Client)
                .Include(b => b.Crop)
                .Include(b => b.Storage)
                .ToList();
            if(CurrentSession.CurrentUser.Role == "Администратор")
            {
                dgBatches.IsReadOnly = true;

                btnDeleteBatch.Visibility = Visibility.Collapsed;
                btnAddBatch.Visibility = Visibility.Collapsed;
                btnSaveChanges.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            dgBatches.ItemsSource = _db.GrainBatches
                .Include(b => b.Client)
                .Include(b => b.Crop)
                .Include(b => b.Storage)
                .Where(b => b.CarNumber.ToLower().Contains(searchText))
                .ToList();
        }

        private void btnAddBatch_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClient.SelectedItem is not Client selectedClient ||
                cmbCrop.SelectedItem is not Crop selectedCrop ||
                cmbStorage.SelectedItem is not Storage selectedStorage)
            {
                MessageBox.Show("Выберите клиента, культуру и склад.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string carNumber = txtCarNumber.Text?.Trim();
            if (string.IsNullOrWhiteSpace(carNumber))
            {
                MessageBox.Show("Номер автомобиля обязателен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtGrossWeight.Text?.Replace('.', ','), out decimal grossWeight) || grossWeight <= 0)
            {
                MessageBox.Show("Вес брутто должен быть > 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtTareWeight.Text?.Replace('.', ','), out decimal tareWeight) || tareWeight < 0)
            {
                MessageBox.Show("Вес тара должен быть >= 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (tareWeight >= grossWeight)
            {
                MessageBox.Show("Брутто должно быть больше тары.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            decimal netWeight = grossWeight - tareWeight;

            if (selectedStorage.CurrentLoad + netWeight > selectedStorage.Capacity)
            {
                MessageBox.Show("Склад переполнен. Выберите другой или уменьшите партию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newBatch = new GrainBatch
            {
                ClientId = selectedClient.Id,
                WeigherId = CurrentSession.CurrentUser.Id,
                CropId = selectedCrop.Id,
                StorageId = selectedStorage.Id,
                CarNumber = carNumber,
                GrossWeight = grossWeight,
                TareWeight = tareWeight,
                ArrivalDate = DateTime.Now,
                Status = "Ожидает анализа"
            };

            selectedStorage.CurrentLoad += netWeight;

            _db.GrainBatches.Add(newBatch);
            _db.SaveChanges();

            txtCarNumber.Clear();
            txtGrossWeight.Clear();
            txtTareWeight.Clear();
            cmbClient.SelectedIndex = -1;
            cmbCrop.SelectedIndex = -1;
            cmbStorage.SelectedIndex = -1;
            txtSearch.Clear();

            LoadData();
            MessageBox.Show("Партия успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            dgBatches.CommitEdit(DataGridEditingUnit.Row, true);

            var changedBatches = _db.ChangeTracker.Entries<GrainBatch>()
                .Where(x => x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .ToList();

            foreach (var batch in changedBatches)
            {
                if (string.IsNullOrWhiteSpace(batch.CarNumber))
                {
                    MessageBox.Show($"Номер авто (ID {batch.Id}) пуст. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (batch.GrossWeight <= 0 || batch.TareWeight < 0 || batch.TareWeight >= batch.GrossWeight)
                {
                    MessageBox.Show($"Ошибка в весе (ID {batch.Id}). Брутто должно быть > Тары. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                var entry = _db.ChangeTracker.Entries<GrainBatch>().First(x => x.Entity == batch);
                decimal oldGross = (decimal)entry.OriginalValues["GrossWeight"];
                decimal oldTare = (decimal)entry.OriginalValues["TareWeight"];
                decimal oldNet = oldGross - oldTare;
                decimal newNet = batch.GrossWeight - batch.TareWeight;
                decimal delta = newNet - oldNet;

                if (delta != 0 && batch.StorageId.HasValue)
                {
                    var storage = _db.Storages.Find(batch.StorageId.Value);
                    if (storage != null)
                    {
                        if (storage.CurrentLoad + delta > storage.Capacity)
                        {
                            MessageBox.Show($"Ошибка сохранения партии {batch.CarNumber}. Склад '{storage.Name}' будет переполнен. Доступно: {storage.Capacity - storage.CurrentLoad} т.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            _db.ChangeTracker.Clear();
                            LoadData();
                            return;
                        }

                        storage.CurrentLoad += delta;
                        if (storage.CurrentLoad < 0) storage.CurrentLoad = 0;
                    }
                }
            }

            try
            {
                _db.SaveChanges();
                _db.ChangeTracker.Clear();
                LoadData();
                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _db.ChangeTracker.Clear();
                LoadData();
                MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteBatch_Click(object sender, RoutedEventArgs e)
        {
            if (dgBatches.SelectedItem is GrainBatch selectedBatch)
            {
                if (_db.LabTests.Any(t => t.BatchId == selectedBatch.Id) || _db.RenderedServices.Any(rs => rs.BatchId == selectedBatch.Id))
                {
                    MessageBox.Show("Нельзя удалить партию. По ней есть анализы или услуги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить партию '{selectedBatch.CarNumber}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var storage = _db.Storages.FirstOrDefault(s => s.Id == selectedBatch.StorageId);
                    if (storage != null)
                    {
                        decimal actualNet = selectedBatch.GrossWeight - selectedBatch.TareWeight;
                        storage.CurrentLoad -= actualNet;
                        if (storage.CurrentLoad < 0) storage.CurrentLoad = 0;
                    }

                    _db.GrainBatches.Remove(selectedBatch);
                    _db.SaveChanges();
                    txtSearch.Clear();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите партию для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    var worksheet = package.Workbook.Worksheets.Add("Партии");
                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Клиент";
                    worksheet.Cells[1, 3].Value = "Культура";
                    worksheet.Cells[1, 4].Value = "Склад";
                    worksheet.Cells[1, 5].Value = "Авто";
                    worksheet.Cells[1, 6].Value = "Брутто (т)";
                    worksheet.Cells[1, 7].Value = "Тара (т)";
                    worksheet.Cells[1, 8].Value = "Нетто (т)";
                    worksheet.Cells[1, 9].Value = "Статус";

                    var batches = _db.GrainBatches.Include(b => b.Client).Include(b => b.Crop).Include(b => b.Storage).ToList();
                    for (int i = 0; i < batches.Count; i++)
                    {
                        var b = batches[i];
                        worksheet.Cells[i + 2, 1].Value = b.Id;
                        worksheet.Cells[i + 2, 2].Value = b.Client.CompanyName;
                        worksheet.Cells[i + 2, 3].Value = b.Crop.Name;
                        worksheet.Cells[i + 2, 4].Value = b.Storage?.Name;
                        worksheet.Cells[i + 2, 5].Value = b.CarNumber;
                        worksheet.Cells[i + 2, 6].Value = b.GrossWeight;
                        worksheet.Cells[i + 2, 7].Value = b.TareWeight;
                        worksheet.Cells[i + 2, 8].Value = b.NetWeight;
                        worksheet.Cells[i + 2, 9].Value = b.Status;
                    }
                    File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                    MessageBox.Show("Выгружено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}