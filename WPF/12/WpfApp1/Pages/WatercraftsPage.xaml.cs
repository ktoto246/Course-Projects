using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace WpfApp1.Pages
{
    public partial class WatercraftsPage : Page
    {
        private BdContext db;

        public WatercraftsPage()
        {
            InitializeComponent();
            db = new BdContext();
            LoadDictionaries();
            LoadData();
        }

        private void LoadDictionaries()
        {
            cmbCategory.ItemsSource = db.Категорииs.ToList();

            if (cmbCategory.Items.Count > 0)
                cmbCategory.SelectedIndex = 0;
        }

        private void LoadData()
        {
            WatercraftsGrid.ItemsSource = db.Плавсредстваs
                .Include(p => p.IdКатегорииNavigation)
                .ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCategory.SelectedItem == null || string.IsNullOrWhiteSpace(txtModel.Text) || string.IsNullOrWhiteSpace(txtSerial.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }

            var selectedCategory = (Категории)cmbCategory.SelectedItem;
            var selectedStatus = ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString();

            var newBoat = new Плавсредства
            {
                IdКатегории = selectedCategory.IdКатегории,
                Модель = txtModel.Text.Trim(),
                СерийныйНомер = txtSerial.Text.Trim(),
                Состояние = selectedStatus,
                Доступно = selectedStatus == "Исправно"
            };

            db.Плавсредстваs.Add(newBoat);
            db.SaveChanges();

            txtModel.Clear();
            txtSerial.Clear();

            LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (WatercraftsGrid.SelectedItem is Плавсредства selected)
            {
                if (MessageBox.Show($"Удалить {selected.Модель}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    db.Плавсредстваs.Remove(selected);
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
                MessageBox.Show("Данные по плавсредствам обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Ошибка: такой серийный номер уже есть в базе или данные некорректны.", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
                foreach (var entry in db.ChangeTracker.Entries())
                {
                    entry.Reload();
                }
                LoadData(); 
            }
        }

        private void Filter_Changed(object sender, TextChangedEventArgs e)
        {
            string search = txtSearch.Text.ToLower();

            WatercraftsGrid.ItemsSource = db.Плавсредстваs
                .Include(p => p.IdКатегорииNavigation)
                .Where(p => p.Модель.ToLower().Contains(search) || p.СерийныйНомер.ToLower().Contains(search))
                .ToList();
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx", 
                    Title = "Сохранить список плавсредств",
                    FileName = "Плавсредства_Отчет.xlsx" 
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Инвентарь");

                        worksheet.Cell(1, 1).Value = "ID";
                        worksheet.Cell(1, 2).Value = "Категория";
                        worksheet.Cell(1, 3).Value = "Модель";
                        worksheet.Cell(1, 4).Value = "Серийный номер";
                        worksheet.Cell(1, 5).Value = "Состояние";
                        worksheet.Cell(1, 6).Value = "Доступно";

                        worksheet.Range("A1:F1").Style.Font.Bold = true;
                        worksheet.Range("A1:F1").Style.Fill.BackgroundColor = XLColor.LightGray;


                        var watercrafts = db.Плавсредстваs.Include(p => p.IdКатегорииNavigation).ToList();

                        int row = 2;
                        foreach (var item in watercrafts)
                        {
                            worksheet.Cell(row, 1).Value = item.IdПлавсредства;

                            worksheet.Cell(row, 2).Value = item.IdКатегорииNavigation != null
                                ? item.IdКатегорииNavigation.Название
                                : "Без категории";

                            worksheet.Cell(row, 3).Value = item.Модель;
                            worksheet.Cell(row, 4).Value = item.СерийныйНомер;
                            worksheet.Cell(row, 5).Value = item.Состояние;

                            worksheet.Cell(row, 6).Value = item.Доступно == true ? "Да" : "Нет";

                            row++; 
                        }

                        worksheet.Columns().AdjustToContents();

                        workbook.SaveAs(saveFileDialog.FileName);

                        MessageBox.Show("Отчет успешно выгружен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при выгрузке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void BtnToggleStatus_Click(object sender, RoutedEventArgs e)
        {
            if (WatercraftsGrid.SelectedItem is Плавсредства selected)
            {
                if (selected.Состояние == "Исправно")
                {
                    if (MessageBox.Show($"Отправить '{selected.Модель}' в ремонт? Она пропадет из списка выдачи.", "Смена статуса", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        selected.Состояние = "В ремонте";
                        selected.Доступно = false;
                    }
                }
                else
                {
                    if (MessageBox.Show($"Вернуть '{selected.Модель}' в строй (сделать 'Исправно')?", "Смена статуса", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        selected.Состояние = "Исправно";
                        selected.Доступно = true;
                    }
                }

                try
                {
                    db.SaveChanges();
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при смене статуса: {ex.Message}", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Сначала выбери лодку в таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}