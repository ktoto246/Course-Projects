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
    public partial class CategoriesPage : Page
    {
        private BdContext db;

        public CategoriesPage()
        {
            InitializeComponent();
            db = new BdContext();
            LoadData();
        }

        private void LoadData()
        {
            CategoriesGrid.ItemsSource = db.Категорииs.ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || !decimal.TryParse(txtPrice.Text, out decimal price))
            {
                MessageBox.Show("Введите название и корректную цену!", "Ошибка");
                return;
            }

            var category = new Категории
            {
                Название = txtName.Text.Trim(),
                ЦенаЗаЧас = price
            };

            db.Категорииs.Add(category);
            db.SaveChanges();

            txtName.Clear();
            txtPrice.Clear();
            LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is Категории selected)
            {
                if (db.Плавсредстваs.Any(p => p.IdКатегории == selected.IdКатегории))
                {
                    MessageBox.Show("Нельзя удалить категорию, к которой привязаны лодки!", "Ошибка");
                    return;
                }

                if (MessageBox.Show($"Удалить {selected.Название}?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    db.Категорииs.Remove(selected);
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
                MessageBox.Show("Прайс-лист обновлен!", "Успех");
                LoadData();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Ошибка при обновлении цен. Возможно, введены недопустимые значения.", "Ошибка");
                foreach (var entry in db.ChangeTracker.Entries()) entry.Reload();
                LoadData();
            }
        }

        private void Filter_Changed(object sender, TextChangedEventArgs e)
        {
            string s = txtSearch.Text.ToLower();
            CategoriesGrid.ItemsSource = db.Категорииs
                .Where(x => x.Название.ToLower().Contains(s))
                .ToList();
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sfd = new SaveFileDialog { Filter = "Excel|*.xlsx", FileName = "Pricelist.xlsx" };
                if (sfd.ShowDialog() == true)
                {
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Price");
                        ws.Cell(1, 1).Value = "ID";
                        ws.Cell(1, 2).Value = "Название";
                        ws.Cell(1, 3).Value = "Цена/час";

                        var data = db.Категорииs.ToList();
                        for (int i = 0; i < data.Count; i++)
                        {
                            ws.Cell(i + 2, 1).Value = data[i].IdКатегории;
                            ws.Cell(i + 2, 2).Value = data[i].Название;
                            ws.Cell(i + 2, 3).Value = data[i].ЦенаЗаЧас;
                        }
                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                        MessageBox.Show("Выгружено");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}