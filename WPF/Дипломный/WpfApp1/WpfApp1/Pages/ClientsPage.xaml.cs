using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;
using OfficeOpenXml;
using System.IO;
using Microsoft.Win32;

namespace WpfApp1.Pages
{
    public partial class ClientsPage : Page
    {
        private AppDbContext _db;

        public ClientsPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            dgClients.ItemsSource = _db.Clients.ToList();
        }

        private bool ValidateClientData(string name, string inn, string address, string phone, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
            {
                errorMessage = "Название организации не может быть пустым и должно содержать минимум 3 символа.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(inn) || (!Regex.IsMatch(inn, "^[0-9]{10}$") && !Regex.IsMatch(inn, "^[0-9]{12}$")))
            {
                errorMessage = "ИНН должен состоять только из цифр и содержать ровно 10 или 12 символов.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(address) && address.Length < 5)
            {
                errorMessage = "Если адрес указан, он должен содержать минимум 5 символов.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(phone) && !Regex.IsMatch(phone, @"^\+?[0-9\-\(\)\s]{7,20}$"))
            {
                errorMessage = "Телефон введен некорректно. Разрешены цифры, плюсы, скобки и дефисы.";
                return false;
            }

            return true;
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();
            dgClients.ItemsSource = _db.Clients
                .Where(c => c.CompanyName.ToLower().Contains(search) || c.INN.Contains(search))
                .ToList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = txtNewName.Text?.Trim();
            string inn = txtNewINN.Text?.Trim();
            string address = string.IsNullOrWhiteSpace(txtNewAddress.Text) ? null : txtNewAddress.Text.Trim();
            string phone = string.IsNullOrWhiteSpace(txtNewPhone.Text) ? null : txtNewPhone.Text.Trim();

            if (!ValidateClientData(name, inn, address, phone, out string error))
            {
                MessageBox.Show(error, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_db.Clients.Any(c => c.INN == inn))
            {
                MessageBox.Show("Клиент с таким ИНН уже существует в базе.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newClient = new Client
            {
                CompanyName = name,
                INN = inn,
                Address = address,
                Phone = phone
            };

            _db.Clients.Add(newClient);
            _db.SaveChanges();

            txtNewName.Clear();
            txtNewINN.Clear();
            txtNewAddress.Clear();
            txtNewPhone.Clear();

            LoadData();
            MessageBox.Show("Клиент успешно добавлен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            dgClients.CommitEdit(DataGridEditingUnit.Row, true);
            var modifiedEntries = _db.ChangeTracker.Entries<Client>()
                .Where(x => x.State == EntityState.Modified || x.State == EntityState.Added)
                .ToList();

            foreach (var entry in modifiedEntries)
            {
                var client = entry.Entity;
                if (!ValidateClientData(client.CompanyName, client.INN, client.Address, client.Phone, out string error))
                {
                    MessageBox.Show($"Ошибка в данных клиента (ID: {client.Id}):\n{error}\n\nИзменения отменены.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);

                    _db.ChangeTracker.Clear();
                    LoadData();
                    return;
                }
            }

            _db.SaveChanges();
            MessageBox.Show("Изменения успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadData();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgClients.SelectedItem is Client selectedClient)
            {
                bool isUsed = _db.GrainBatches.Any(b => b.ClientId == selectedClient.Id);

                if (isUsed)
                {
                    MessageBox.Show("Невозможно удалить клиента. У него есть оформленные партии зерна.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show($"Удалить клиента {selectedClient.CompanyName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _db.Clients.Remove(selectedClient);
                    _db.SaveChanges();
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите клиента в таблице для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx" };
            if (sfd.ShowDialog() == true)
            {
                using (var package = new ExcelPackage())
                {
                    var sheet = package.Workbook.Worksheets.Add("Клиенты");

                    sheet.Cells[1, 1].Value = "ID";
                    sheet.Cells[1, 2].Value = "Название организации";
                    sheet.Cells[1, 3].Value = "ИНН";
                    sheet.Cells[1, 4].Value = "Адрес";
                    sheet.Cells[1, 5].Value = "Телефон";

                    var clients = _db.Clients.ToList();
                    for (int i = 0; i < clients.Count; i++)
                    {
                        sheet.Cells[i + 2, 1].Value = clients[i].Id;
                        sheet.Cells[i + 2, 2].Value = clients[i].CompanyName;
                        sheet.Cells[i + 2, 3].Value = clients[i].INN;
                        sheet.Cells[i + 2, 4].Value = clients[i].Address;
                        sheet.Cells[i + 2, 5].Value = clients[i].Phone;
                    }

                    File.WriteAllBytes(sfd.FileName, package.GetAsByteArray());
                    MessageBox.Show("Данные успешно экспортированы", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}