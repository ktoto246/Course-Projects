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
    public partial class CropsPage : Page
    {
        private AppDbContext _db;

        public CropsPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            dgCrops.ItemsSource = _db.Crops.ToList();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            dgCrops.ItemsSource = _db.Crops
                .Where(c => c.Name.ToLower().Contains(searchText))
                .ToList();
        }

        private void btnAddCrop_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNewName.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                MessageBox.Show("Название должно содержать минимум 3 символа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_db.Crops.Any(c => c.Name == name))
            {
                MessageBox.Show("Культура с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtNewMoisture.Text?.Replace('.', ','), out decimal moisture) || moisture < 0 || moisture > 100)
            {
                MessageBox.Show("Влажность должна быть числом от 0 до 100%.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtNewWeediness.Text?.Replace('.', ','), out decimal weediness) || weediness < 0 || weediness > 100)
            {
                MessageBox.Show("Сорность должна быть числом от 0 до 100%.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newCrop = new Crop
            {
                Name = name,
                BaseMoisture = moisture,
                BaseWeediness = weediness
            };

            _db.Crops.Add(newCrop);
            _db.SaveChanges();

            txtNewName.Clear();
            txtNewMoisture.Clear();
            txtNewWeediness.Clear();
            txtSearch.Clear();

            LoadData();
            MessageBox.Show("Культура успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            dgCrops.CommitEdit(DataGridEditingUnit.Row, true);
            var changedCrops = _db.ChangeTracker.Entries<Crop>()
                .Where(x => x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .ToList();

            foreach (var crop in changedCrops)
            {
                if (string.IsNullOrWhiteSpace(crop.Name) || crop.Name.Length < 3)
                {
                    MessageBox.Show($"Название культуры (ID: {crop.Id}) слишком короткое. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (_db.Crops.Any(c => c.Name == crop.Name && c.Id != crop.Id))
                {
                    MessageBox.Show($"Название '{crop.Name}' уже используется. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (crop.BaseMoisture < 0 || crop.BaseMoisture > 100)
                {
                    MessageBox.Show($"Влажность культуры (ID: {crop.Id}) должна быть от 0 до 100%. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (crop.BaseWeediness < 0 || crop.BaseWeediness > 100)
                {
                    MessageBox.Show($"Сорность культуры (ID: {crop.Id}) должна быть от 0 до 100%. Изменения отменены.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }
            }

            _db.SaveChanges();
            MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
        }

        private void btnDeleteCrop_Click(object sender, RoutedEventArgs e)
        {
            if (dgCrops.SelectedItem is Crop selectedCrop)
            {
                if (_db.GrainBatches.Any(b => b.CropId == selectedCrop.Id))
                {
                    MessageBox.Show($"Нельзя удалить '{selectedCrop.Name}', так как культура используется в партиях зерна.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить культуру '{selectedCrop.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _db.Crops.Remove(selectedCrop);
                    _db.SaveChanges();
                    txtSearch.Clear();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите культуру для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    var worksheet = package.Workbook.Worksheets.Add("Культуры");

                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Название";
                    worksheet.Cells[1, 3].Value = "Базовая влажность (%)";
                    worksheet.Cells[1, 4].Value = "Базовая сорность (%)";

                    var crops = _db.Crops.ToList();
                    for (int i = 0; i < crops.Count; i++)
                    {
                        var crop = crops[i];
                        worksheet.Cells[i + 2, 1].Value = crop.Id;
                        worksheet.Cells[i + 2, 2].Value = crop.Name;
                        worksheet.Cells[i + 2, 3].Value = crop.BaseMoisture;
                        worksheet.Cells[i + 2, 4].Value = crop.BaseWeediness;
                    }

                    File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                    MessageBox.Show("Данные выгружены в Excel", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}