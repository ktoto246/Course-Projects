using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Models;
using ClosedXML.Excel;
using Microsoft.Win32;
using System;

namespace WpfApp1.Pages
{
    public partial class CategoriesPage : Page
    {
        private AppDbContext _context;
        private ICollectionView _categoriesView;

        public CategoriesPage()
        {
            InitializeComponent();
            if (AppSession.CurrentRole == "Viewer" || AppSession.CurrentRole == "Operator")
            {
                AddPanel.Visibility = Visibility.Collapsed;
                BtnDelete.Visibility = Visibility.Collapsed;
                BtnSave.Visibility = Visibility.Collapsed;
                CategoriesGrid.IsReadOnly = true;
            }
            try
            {
                _context = new AppDbContext();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка БД: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            this.Unloaded += (s, e) => _context?.Dispose();
        }

        private void LoadData()
        {
            _context.Categories.Load();
            _categoriesView = CollectionViewSource.GetDefaultView(_context.Categories.Local.ToObservableCollection());
            _categoriesView.Filter = FilterCategories;
            CategoriesGrid.ItemsSource = _categoriesView;
        }

        private bool FilterCategories(object item)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
                return true;

            if (item is Category c)
            {
                return c.Name.ToLower().Contains(txtSearch.Text.ToLower());
            }
            return false;
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            _categoriesView?.Refresh();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string newCatName = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(newCatName))
            {
                MessageBox.Show("Пожалуйста, введите название новой категории.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_context.Categories.Any(c => c.Name.ToLower() == newCatName.ToLower()))
            {
                MessageBox.Show($"Категория '{newCatName}' уже существует в базе!", "Ошибка уникальности", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newCategory = new Category { Name = newCatName };

            try
            {
                _context.Categories.Add(newCategory);
                _context.SaveChanges();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            txtName.Clear();
            CategoriesGrid.Items.Refresh();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem is Category selected)
            {
                if (_context.Equipments.Any(eq => eq.CategoryId == selected.Id))
                {
                    MessageBox.Show("Нельзя удалить категорию, пока к ней привязано оборудование!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Удалить '{selected.Name}'?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Categories.Remove(selected);
                    _context.SaveChanges();
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _context.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены.");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    Title = "Сохранить отчет",
                    FileName = $"Категории_{DateTime.Now:yyyyMMdd}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Категории");

                        worksheet.Cell(1, 1).Value = "Код";
                        worksheet.Cell(1, 2).Value = "Наименование";

                        var headerRow = worksheet.Row(1);
                        headerRow.Style.Font.Bold = true;
                        headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

                        int row = 2;
                        foreach (Category c in _categoriesView)
                        {
                            worksheet.Cell(row, 1).Value = c.Id;
                            worksheet.Cell(row, 2).Value = c.Name;
                            row++;
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(saveFileDialog.FileName);
                    }
                    MessageBox.Show("Отчет успешно сохранен в Excel!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
