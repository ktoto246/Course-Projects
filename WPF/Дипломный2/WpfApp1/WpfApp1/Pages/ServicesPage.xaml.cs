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
    public partial class ServicesPage : Page
    {
        private AppDbContext _db;

        public ServicesPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            dgServices.ItemsSource = _db.Services.ToList();
            if (CurrentSession.CurrentUser.Role == "Администратор")
            {
                dgServices.IsReadOnly = true;
                btnSaveChanges.Visibility = Visibility.Collapsed;
                btnDeleteService.Visibility = Visibility.Collapsed;
                btnAddService.Visibility = Visibility.Collapsed;
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            dgServices.ItemsSource = _db.Services
                .Where(s => s.Name.ToLower().Contains(searchText))
                .ToList();
        }

        private void btnAddService_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNewName.Text?.Trim();
            string unit = txtNewUnit.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                MessageBox.Show("Название должно содержать минимум 3 символа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(unit))
            {
                MessageBox.Show("Единица измерения обязательна.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_db.Services.Any(s => s.Name == name))
            {
                MessageBox.Show("Услуга с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtNewUnitPrice.Text?.Replace('.', ','), out decimal unitPrice) || unitPrice < 0)
            {
                MessageBox.Show("Цена должна быть числом >= 0.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (unitPrice == 0)
            {
                var res = MessageBox.Show("Цена услуги равна 0. Это бесплатная услуга? Сохранить?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (res != MessageBoxResult.Yes) return;
            }

            var newService = new Service
            {
                Name = name,
                Unit = unit,
                UnitPrice = unitPrice
            };

            _db.Services.Add(newService);
            _db.SaveChanges();

            txtNewName.Clear();
            txtNewUnit.Clear();
            txtNewUnitPrice.Clear();
            txtSearch.Clear();

            LoadData();
            MessageBox.Show("Услуга успешно добавлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            dgServices.CommitEdit(DataGridEditingUnit.Row, true);

            var changedServices = _db.ChangeTracker.Entries<Service>()
                .Where(x => x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .ToList();

            bool priceChanged = false;

            foreach (var service in changedServices)
            {
                if (string.IsNullOrWhiteSpace(service.Name) || service.Name.Length < 3)
                {
                    MessageBox.Show($"Название (ID: {service.Id}) слишком короткое. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (string.IsNullOrWhiteSpace(service.Unit))
                {
                    MessageBox.Show($"Единица измерения (ID: {service.Id}) не может быть пустой. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (_db.Services.Any(s => s.Name == service.Name && s.Id != service.Id))
                {
                    MessageBox.Show($"Название '{service.Name}' уже занято. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                if (service.UnitPrice < 0)
                {
                    MessageBox.Show($"Цена (ID: {service.Id}) не может быть < 0. Отмена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }

                var entry = _db.ChangeTracker.Entries<Service>().First(x => x.Entity == service);
                decimal oldPrice = (decimal)entry.OriginalValues["UnitPrice"];
                if (oldPrice != service.UnitPrice)
                {
                    priceChanged = true;
                }
            }

            _db.SaveChanges();
            _db.ChangeTracker.Clear();
            LoadData();

            if (priceChanged)
            {
                MessageBox.Show("Изменения сохранены.\n\nОбратите внимание: изменение цены не влияет на уже оформленные услуги (старые счета не пересчитываются).", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnDeleteService_Click(object sender, RoutedEventArgs e)
        {
            if (dgServices.SelectedItem is Service selectedService)
            {
                if (_db.RenderedServices.Any(rs => rs.ServiceId == selectedService.Id))
                {
                    MessageBox.Show($"Нельзя удалить услугу '{selectedService.Name}', так как она уже оказывалась.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (MessageBox.Show($"Удалить услугу '{selectedService.Name}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _db.Services.Remove(selectedService);
                    _db.SaveChanges();
                    txtSearch.Clear();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите услугу для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                    var worksheet = package.Workbook.Worksheets.Add("Услуги");
                    worksheet.Cells[1, 1].Value = "ID";
                    worksheet.Cells[1, 2].Value = "Название";
                    worksheet.Cells[1, 3].Value = "Ед. измерения";
                    worksheet.Cells[1, 4].Value = "Цена";

                    var services = _db.Services.ToList();
                    for (int i = 0; i < services.Count; i++)
                    {
                        var s = services[i];
                        worksheet.Cells[i + 2, 1].Value = s.Id;
                        worksheet.Cells[i + 2, 2].Value = s.Name;
                        worksheet.Cells[i + 2, 3].Value = s.Unit;
                        worksheet.Cells[i + 2, 4].Value = s.UnitPrice;
                    }
                    File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                    MessageBox.Show("Выгружено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}