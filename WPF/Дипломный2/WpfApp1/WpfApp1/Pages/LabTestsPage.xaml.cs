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
    public partial class LabTestsPage : Page
    {
        private AppDbContext _db;

        public LabTestsPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            cmbBatch.SelectedValuePath = "Id";
            LoadData();
        }

        private void LoadData()
        {
            var availableBatches = _db.GrainBatches
                .Include(b => b.Client)
                .Include(b => b.Crop)
                .Include(b => b.LabTest)
                .Where(b => b.LabTest == null && b.Status == "Ожидает анализа")
                .ToList()
                .Select(b => new
                {
                    Id = b.Id,
                    DisplayText = $"#{b.Id} | {b.CarNumber} | {b.Crop.Name} | {b.Client.CompanyName}"
                })
                .ToList();

            cmbBatch.ItemsSource = availableBatches;

            dgLabTests.ItemsSource = _db.LabTests
                .Include(t => t.Batch)
                .ToList();

            if (CurrentSession.CurrentUser.Role == "Администратор")
            {
                dgLabTests.IsReadOnly = true;

                btnDeleteTest.Visibility = Visibility.Collapsed;
                btnAddTest.Visibility = Visibility.Collapsed;
                btnSaveChanges.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            dgLabTests.ItemsSource = _db.LabTests
                .Include(t => t.Batch)
                .Where(t => t.Batch.CarNumber.ToLower().Contains(searchText))
                .ToList();
        }

        private void btnAddTest_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBatch.SelectedValue == null)
            {
                MessageBox.Show("Выберите партию для анализа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedBatchId = (int)cmbBatch.SelectedValue;

            if (!decimal.TryParse(txtMoisture.Text?.Replace('.', ','), out decimal moisture) || moisture < 0 || moisture > 100)
            {
                MessageBox.Show("Влажность должна быть от 0 до 100%.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtWeediness.Text?.Replace('.', ','), out decimal weediness) || weediness < 0 || weediness > 100)
            {
                MessageBox.Show("Сорность должна быть от 0 до 100%.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var batch = _db.GrainBatches.FirstOrDefault(b => b.Id == selectedBatchId);
            if (batch == null) return;

            var newTest = new LabTest
            {
                BatchId = selectedBatchId,
                LabTechId = CurrentSession.CurrentUser.Id,
                Moisture = moisture,
                Weediness = weediness,
                Organoleptics = txtOrganoleptics.Text?.Trim(),
                TestDate = DateTime.Now
            };

            _db.LabTests.Add(newTest);
            batch.Status = "Проведен анализ";
            _db.SaveChanges();

            cmbBatch.SelectedIndex = -1;
            txtMoisture.Clear();
            txtWeediness.Clear();
            txtOrganoleptics.Clear();
            txtSearch.Clear();

            LoadData();
            MessageBox.Show("Анализ добавлен, статус партии обновлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            dgLabTests.CommitEdit(DataGridEditingUnit.Row, true);

            var changedTests = _db.ChangeTracker.Entries<LabTest>()
                .Where(x => x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .ToList();

            foreach (var test in changedTests)
            {
                if (test.Moisture < 0 || test.Moisture > 100 || test.Weediness < 0 || test.Weediness > 100)
                {
                    MessageBox.Show($"Показатели (ID {test.Id}) должны быть от 0 до 100%. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }
            }

            _db.SaveChanges();
            LoadData();
            MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnDeleteTest_Click(object sender, RoutedEventArgs e)
        {
            if (dgLabTests.SelectedItem is LabTest selectedTest)
            {
                if (_db.RenderedServices.Any(rs => rs.BatchId == selectedTest.BatchId))
                {
                    MessageBox.Show("Нельзя удалить анализ: по этой партии уже оформлены услуги.\nСначала удалите связанные услуги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить анализ партии ID {selectedTest.BatchId}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    var batch = _db.GrainBatches.Find(selectedTest.BatchId);
                    if (batch != null)
                    {
                        batch.Status = "Ожидает анализа";
                    }

                    _db.LabTests.Remove(selectedTest);
                    _db.SaveChanges();
                    txtSearch.Clear();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите анализ.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    var worksheet = package.Workbook.Worksheets.Add("Анализы");
                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Партия";
                    worksheet.Cells[1, 3].Value = "Автомобиль";
                    worksheet.Cells[1, 4].Value = "Влажность (%)";
                    worksheet.Cells[1, 5].Value = "Сорность (%)";
                    worksheet.Cells[1, 6].Value = "Органолептика";

                    var tests = _db.LabTests.Include(t => t.Batch).ToList();
                    for (int i = 0; i < tests.Count; i++)
                    {
                        var t = tests[i];
                        worksheet.Cells[i + 2, 1].Value = t.Id;
                        worksheet.Cells[i + 2, 2].Value = t.BatchId;
                        worksheet.Cells[i + 2, 3].Value = t.Batch.CarNumber;
                        worksheet.Cells[i + 2, 4].Value = t.Moisture;
                        worksheet.Cells[i + 2, 5].Value = t.Weediness;
                        worksheet.Cells[i + 2, 6].Value = t.Organoleptics;
                    }
                    File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                    MessageBox.Show("Выгружено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}