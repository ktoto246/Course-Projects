using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;
using Microsoft.Win32;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Pages
{
    public partial class ClientsPage : Page
    {
        private BdContext db;

        public ClientsPage()
        {
            InitializeComponent();
            db = new BdContext();
            LoadData();
        }

        private void LoadData()
        {
            ClientsGrid.ItemsSource = db.Клиентыs.ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFio.Text) || string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Заполни хотя бы ФИО и телефон!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newClient = new Клиенты
            {
                Фио = txtFio.Text.Trim(),
                Телефон = txtPhone.Text.Trim(),
                Паспорт = string.IsNullOrWhiteSpace(txtPassport.Text) ? null : txtPassport.Text.Trim() // Паспорт может быть пустым
            };

            db.Клиентыs.Add(newClient);
            db.SaveChanges();

            txtFio.Clear();
            txtPhone.Clear();
            txtPassport.Clear();

            LoadData(); 
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsGrid.SelectedItem is Клиенты selected)
            {
                bool hasRentals = db.Арендаs.Any(a => a.IdКлиента == selected.IdКлиента);

                if (hasRentals)
                {
                    MessageBox.Show("Этого клиента нельзя удалить, так как он уже брал лодки в прокат. Сначала удалите его заказы из журнала.", "Запрет удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show($"Точно удалить клиента {selected.Фио}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    db.Клиентыs.Remove(selected);
                    db.SaveChanges();
                    LoadData();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try 
            {
                db.SaveChanges();
                MessageBox.Show("Данные клиентов сохранены.", "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Не удалось сохранить изменения. Проверьте правильность заполнения полей.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                foreach (var entry in db.ChangeTracker.Entries()) entry.Reload();
                LoadData();
            }
        }

        private void Filter_Changed(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();

            ClientsGrid.ItemsSource = db.Клиентыs
                .Where(c => c.Фио.ToLower().Contains(search) ||
                            c.Телефон.ToLower().Contains(search) ||
                            (c.Паспорт != null && c.Паспорт.ToLower().Contains(search)))
                .ToList();
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Экспорт базы клиентов",
                    FileName = "Клиенты_Отчет.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Клиенты");

                        worksheet.Cell(1, 1).Value = "ID";
                        worksheet.Cell(1, 2).Value = "ФИО";
                        worksheet.Cell(1, 3).Value = "Телефон";
                        worksheet.Cell(1, 4).Value = "Паспорт";

                        worksheet.Range("A1:D1").Style.Font.Bold = true;
                        worksheet.Range("A1:D1").Style.Fill.BackgroundColor = XLColor.LightGray;

                        var clients = db.Клиентыs.ToList();

                        int row = 2;
                        foreach (var client in clients)
                        {
                            worksheet.Cell(row, 1).Value = client.IdКлиента;
                            worksheet.Cell(row, 2).Value = client.Фио;
                            worksheet.Cell(row, 3).Value = client.Телефон;
                            worksheet.Cell(row, 4).Value = client.Паспорт ?? "Не указан";
                            row++;
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Отчет успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выгрузке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}