using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class RentalsPage : Page
    {
        private BdContext db;

        public RentalsPage()
        {
            InitializeComponent();
            db = new BdContext();
            LoadDictionaries();
            LoadData();
        }

        private void LoadDictionaries()
        {
            cmbClient.ItemsSource = db.Клиентыs.ToList();
            cmbEmployee.ItemsSource = db.Сотрудникиs.ToList();

            cmbWatercraft.ItemsSource = db.Плавсредстваs
                .Include(p => p.IdКатегорииNavigation)
                .Where(p => p.Доступно == true)
                .ToList();
        }

        private void LoadData()
        {
            var query = db.Арендаs
                .Include(a => a.IdКлиентаNavigation)
                .Include(a => a.IdПлавсредстваNavigation)
                .Include(a => a.IdСотрудникаNavigation)
                .AsQueryable();

            if (chkActiveOnly.IsChecked == true)
            {
                query = query.Where(a => a.ВремяКонца == null);
            }

            if (!string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                query = query.Where(a =>
                    a.IdКлиентаNavigation.Фио.ToLower().Contains(search) ||
                    a.IdПлавсредстваNavigation.Модель.ToLower().Contains(search));
            }

            RentalsGrid.ItemsSource = query
                .OrderBy(a => a.ВремяКонца != null)
                .ThenByDescending(a => a.ВремяНачала)
                .ToList();
        }

        private void BtnStartRent_Click(object sender, RoutedEventArgs e)
        {
            if (cmbClient.SelectedItem == null || cmbWatercraft.SelectedItem == null || cmbEmployee.SelectedItem == null)
            {
                MessageBox.Show("Выбери клиента, лодку и сотрудника!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string depositText = txtDeposit.Text.Replace(',', '.');

            if (!decimal.TryParse(depositText, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal depositAmount))
            {
                MessageBox.Show("Сумма залога должна быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedWatercraft = (Плавсредства)cmbWatercraft.SelectedItem;

            var newRental = new Аренда
            {
                IdКлиента = (int)cmbClient.SelectedValue,
                IdПлавсредства = selectedWatercraft.IdПлавсредства,
                IdСотрудника = (int)cmbEmployee.SelectedValue,
                ВремяНачала = DateTime.Now,
                ВремяКонца = null,
                СуммаЗалога = depositAmount,
                ИтогоКОплате = null
            };

            db.Арендаs.Add(newRental);

            selectedWatercraft.Доступно = false;
            db.Плавсредстваs.Update(selectedWatercraft);

            db.SaveChanges();

            txtDeposit.Text = "0";
            LoadDictionaries();
            LoadData();
        }

        private void BtnCompleteRent_Click(object sender, RoutedEventArgs e)
        {
            if (RentalsGrid.SelectedItem is Аренда selectedRental)
            {
                if (selectedRental.ВремяКонца != null) return;

                if (MessageBox.Show("Завершить прокат?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    selectedRental.ВремяКонца = DateTime.Now;

                    var boat = db.Плавсредстваs
                        .Include(p => p.IdКатегорииNavigation)
                        .FirstOrDefault(p => p.IdПлавсредства == selectedRental.IdПлавсредства);

                    if (boat?.IdКатегорииNavigation == null)
                    {
                        MessageBox.Show("Ошибка: Не удалось подгрузить категорию лодки. Расчет невозможен!", "Критический баг");
                        return;
                    }

                    TimeSpan duration = selectedRental.ВремяКонца.Value - selectedRental.ВремяНачала;
                    decimal pricePerHour = boat.IdКатегорииNavigation.ЦенаЗаЧас;

                    decimal finalPrice = (decimal)duration.TotalHours * pricePerHour;
                    selectedRental.ИтогоКОплате = Math.Round(finalPrice, 2);

                    boat.Доступно = true;
                    db.SaveChanges();
                    LoadData();
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (RentalsGrid.SelectedItem is Аренда selectedRental)
            {
                if (MessageBox.Show("Точно удалить эту запись? Если лодка была на воде, она вернется на базу.", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    if (selectedRental.ВремяКонца == null)
                    {
                        var boat = db.Плавсредстваs.Find(selectedRental.IdПлавсредства);
                        if (boat != null)
                        {
                            boat.Доступно = true;
                            db.Плавсредстваs.Update(boat);
                        }
                    }

                    db.Арендаs.Remove(selectedRental);
                    db.SaveChanges();

                    LoadDictionaries();
                    LoadData();
                }
            }
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Сохранить журнал проката",
                    FileName = "Журнал_Аренды.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Журнал");

                        worksheet.Cell(1, 1).Value = "№ Заказа";
                        worksheet.Cell(1, 2).Value = "Клиент";
                        worksheet.Cell(1, 3).Value = "Плавсредство";
                        worksheet.Cell(1, 4).Value = "Сотрудник";
                        worksheet.Cell(1, 5).Value = "Время выдачи";
                        worksheet.Cell(1, 6).Value = "Время возврата";
                        worksheet.Cell(1, 7).Value = "Залог";
                        worksheet.Cell(1, 8).Value = "Итого к оплате";

                        worksheet.Range("A1:H1").Style.Font.Bold = true;
                        worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.LightGray;

                        var currentData = RentalsGrid.ItemsSource.Cast<Аренда>().ToList();

                        int row = 2;
                        foreach (var item in currentData)
                        {
                            worksheet.Cell(row, 1).Value = item.IdАренды;
                            worksheet.Cell(row, 2).Value = item.IdКлиентаNavigation?.Фио;
                            worksheet.Cell(row, 3).Value = item.IdПлавсредстваNavigation?.Модель;
                            worksheet.Cell(row, 4).Value = item.IdСотрудникаNavigation?.Фио;
                            worksheet.Cell(row, 5).Value = item.ВремяНачала.ToString("dd.MM.yyyy HH:mm");
                            worksheet.Cell(row, 6).Value = item.ВремяКонца?.ToString("dd.MM.yyyy HH:mm") ?? "На воде";
                            worksheet.Cell(row, 7).Value = item.СуммаЗалога;
                            worksheet.Cell(row, 8).Value = item.ИтогоКОплате;
                            row++;
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Журнал успешно выгружен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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