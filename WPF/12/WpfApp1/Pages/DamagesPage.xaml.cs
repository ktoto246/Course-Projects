using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace WpfApp1.Pages
{
    public partial class DamagesPage : Page
    {
        private BdContext db;

        public DamagesPage()
        {
            InitializeComponent();
            db = new BdContext();
            LoadDictionaries(); 
            LoadData();
        }

        private void LoadDictionaries()
        {
            cmbRental.ItemsSource = db.Арендаs
                .Include(a => a.IdКлиентаNavigation)
                .OrderByDescending(a => a.IdАренды)
                .ToList();
        }

        private void LoadData()
        {
            DamagesGrid.ItemsSource = db.Поломкиs
                .Include(d => d.IdАрендыNavigation)
                    .ThenInclude(a => a.IdКлиентаNavigation)
                .Include(d => d.IdАрендыNavigation)
                    .ThenInclude(a => a.IdПлавсредстваNavigation)
                .ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cmbRental.SelectedItem == null || string.IsNullOrWhiteSpace(txtDescription.Text))
            {
                MessageBox.Show("Выбери заказ и напиши, что случилось!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(txtFine.Text, out decimal fineAmount))
            {
                MessageBox.Show("Введи нормальную сумму штрафа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newDamage = new Поломки
            {
                IdАренды = (int)cmbRental.SelectedValue,
                Описание = txtDescription.Text.Trim(),
                СуммаШтрафа = fineAmount,
                Оплачено = chkIsPaid.IsChecked
            };

            db.Поломкиs.Add(newDamage);
            db.SaveChanges();

            var rental = db.Арендаs
                .Include(a => a.IdПлавсредстваNavigation)
                .FirstOrDefault(a => a.IdАренды == newDamage.IdАренды);

            if (rental != null && rental.IdПлавсредстваNavigation != null && rental.IdПлавсредстваNavigation.Состояние == "Исправно")
            {
                if (MessageBox.Show($"Поломка зафиксирована. Отправить лодку '{rental.IdПлавсредстваNavigation.Модель}' в ремонт и снять с проката?",
                    "Смена статуса", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    rental.IdПлавсредстваNavigation.Состояние = "В ремонте";
                    rental.IdПлавсредстваNavigation.Доступно = false;
                    db.SaveChanges();
                    MessageBox.Show("Лодка отправлена в ремонт.", "Инфо", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }

            txtDescription.Clear();
            txtFine.Clear();
            chkIsPaid.IsChecked = false;

            LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DamagesGrid.SelectedItem is Поломки selected)
            {
                if (MessageBox.Show("Удалить запись о поломке?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    db.Поломкиs.Remove(selected);
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
                MessageBox.Show("Журнал поломок обновлен.");
                LoadData();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Ошибка сохранения. Проверьте суммы штрафов.");
                foreach (var entry in db.ChangeTracker.Entries()) entry.Reload();
                LoadData();
            }
        }

        private void Filter_Changed(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();

            DamagesGrid.ItemsSource = db.Поломкиs
                .Include(d => d.IdАрендыNavigation)
                    .ThenInclude(a => a.IdКлиентаNavigation)
                .Include(d => d.IdАрендыNavigation)
                    .ThenInclude(a => a.IdПлавсредстваNavigation)
                .Where(d => d.Описание.ToLower().Contains(search))
                .ToList();
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Экспорт журнала поломок",
                    FileName = "Штрафы_Отчет.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Штрафы");

                        worksheet.Cell(1, 1).Value = "ID";
                        worksheet.Cell(1, 2).Value = "Заказ №";
                        worksheet.Cell(1, 3).Value = "Клиент";
                        worksheet.Cell(1, 4).Value = "Плавсредство";
                        worksheet.Cell(1, 5).Value = "Описание поломки";
                        worksheet.Cell(1, 6).Value = "Сумма штрафа";
                        worksheet.Cell(1, 7).Value = "Статус оплаты";

                        worksheet.Range("A1:G1").Style.Font.Bold = true;
                        worksheet.Range("A1:G1").Style.Fill.BackgroundColor = XLColor.LightGray;

                        var damages = db.Поломкиs
                            .Include(d => d.IdАрендыNavigation).ThenInclude(a => a.IdКлиентаNavigation)
                            .Include(d => d.IdАрендыNavigation).ThenInclude(a => a.IdПлавсредстваNavigation)
                            .ToList();

                        int row = 2;
                        foreach (var d in damages)
                        {
                            worksheet.Cell(row, 1).Value = d.IdПоломки;
                            worksheet.Cell(row, 2).Value = d.IdАренды;
                            worksheet.Cell(row, 3).Value = d.IdАрендыNavigation?.IdКлиентаNavigation?.Фио;
                            worksheet.Cell(row, 4).Value = d.IdАрендыNavigation?.IdПлавсредстваNavigation?.Модель;
                            worksheet.Cell(row, 5).Value = d.Описание;
                            worksheet.Cell(row, 6).Value = d.СуммаШтрафа;
                            worksheet.Cell(row, 7).Value = d.Оплачено == true ? "Оплачено" : "Долг";
                            row++;
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Отчет готов!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}