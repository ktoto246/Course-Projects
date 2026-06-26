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
    public partial class StoragesPage : Page
    {
        private AppDbContext _db;

        public StoragesPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            dgStorages.ItemsSource = _db.Storages.ToList();
            if (CurrentSession.CurrentUser.Role == "Администратор")
            {
                dgStorages.IsReadOnly = true;
                btnSaveChanges.Visibility = Visibility.Collapsed;
                btnDeleteStorage.Visibility = Visibility.Collapsed;
                btnAddStorage.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            dgStorages.ItemsSource = _db.Storages
                .Where(s => s.Name.ToLower().Contains(searchText))
                .ToList();
        }

        private void btnAddStorage_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNewName.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                MessageBox.Show("Название должно содержать минимум 3 символа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_db.Storages.Any(s => s.Name == name))
            {
                MessageBox.Show("Склад с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtNewCapacity.Text?.Replace('.', ','), out decimal capacity) || capacity <= 0)
            {
                MessageBox.Show("Вместимость должна быть числом > 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newStorage = new Storage
            {
                Name = name,
                Capacity = capacity,
                CurrentLoad = 0
            };

            _db.Storages.Add(newStorage);
            _db.SaveChanges();

            txtNewName.Clear();
            txtNewCapacity.Clear();
            txtSearch.Clear();

            LoadData();
            MessageBox.Show("Склад успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            dgStorages.CommitEdit(DataGridEditingUnit.Row, true);
            var changedStorages = _db.ChangeTracker.Entries<Storage>()
                .Where(x => x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .ToList();

            foreach (var storage in changedStorages)
            {
                if (string.IsNullOrWhiteSpace(storage.Name) || storage.Name.Length < 3)
                {
                    MessageBox.Show($"Название склада (ID: {storage.Id}) слишком короткое. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (_db.Storages.Any(s => s.Name == storage.Name && s.Id != storage.Id))
                {
                    MessageBox.Show($"Название '{storage.Name}' уже занято. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (storage.Capacity <= 0)
                {
                    MessageBox.Show($"Вместимость (ID: {storage.Id}) должна быть > 0. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (storage.Capacity < storage.CurrentLoad)
                {
                    MessageBox.Show($"Нельзя уменьшить вместимость склада '{storage.Name}' до {storage.Capacity} т: текущая загрузка ({storage.CurrentLoad} т) превышает этот лимит.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (storage.CurrentLoad < 0 || storage.CurrentLoad > storage.Capacity)
                {
                    MessageBox.Show($"Загрузка склада '{storage.Name}' вне допустимого диапазона [0; {storage.Capacity}].", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }
            }

            _db.SaveChanges();
            LoadData();
            MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnDeleteStorage_Click(object sender, RoutedEventArgs e)
        {
            if (dgStorages.SelectedItem is Storage selectedStorage)
            {
                if (_db.GrainBatches.Any(b => b.StorageId == selectedStorage.Id))
                {
                    MessageBox.Show($"Нельзя удалить склад '{selectedStorage.Name}', в нем есть партии зерна.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить склад '{selectedStorage.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _db.Storages.Remove(selectedStorage);
                    _db.SaveChanges();
                    txtSearch.Clear();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите склад для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    var worksheet = package.Workbook.Worksheets.Add("Склады");
                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Название";
                    worksheet.Cells[1, 3].Value = "Вместимость (т)";
                    worksheet.Cells[1, 4].Value = "Текущая загрузка (т)";

                    var storages = _db.Storages.ToList();
                    for (int i = 0; i < storages.Count; i++)
                    {
                        var s = storages[i];
                        worksheet.Cells[i + 2, 1].Value = s.Id;
                        worksheet.Cells[i + 2, 2].Value = s.Name;
                        worksheet.Cells[i + 2, 3].Value = s.Capacity;
                        worksheet.Cells[i + 2, 4].Value = s.CurrentLoad;
                    }
                    File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                    MessageBox.Show("Выгружено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}