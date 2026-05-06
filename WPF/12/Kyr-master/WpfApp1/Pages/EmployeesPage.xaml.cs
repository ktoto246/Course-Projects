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
    public partial class EmployeesPage : Page
    {
        private BdContext db;

        public EmployeesPage()
        {
            InitializeComponent();
            db = new BdContext();
            LoadData();
        }

        private void LoadData()
        {
            EmployeesGrid.ItemsSource = db.Сотрудникиs.ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFio.Text) || string.IsNullOrWhiteSpace(txtPosition.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var employee = new Сотрудники
            {
                Фио = txtFio.Text.Trim(),
                Должность = txtPosition.Text.Trim()
            };

            db.Сотрудникиs.Add(employee);
            db.SaveChanges();

            txtFio.Clear();
            txtPosition.Clear();
            LoadData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesGrid.SelectedItem is Сотрудники selected)
            {
                if (db.Арендаs.Any(a => a.IdСотрудника == selected.IdСотрудника))
                {
                    MessageBox.Show("Нельзя удалить сотрудника, который оформлял аренду!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show($"Удалить {selected.Фио}?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    db.Сотрудникиs.Remove(selected);
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
                MessageBox.Show("Список сотрудников обновлен.");
                LoadData();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Не удалось обновить данные сотрудников.");
                foreach (var entry in db.ChangeTracker.Entries()) entry.Reload();
                LoadData();
            }
        }

        private void Filter_Changed(object sender, TextChangedEventArgs e)
        {
            string s = txtSearch.Text.ToLower();
            EmployeesGrid.ItemsSource = db.Сотрудникиs
                .Where(x => x.Фио.ToLower().Contains(s) || x.Должность.ToLower().Contains(s))
                .ToList();
        }

        private void BtnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sfd = new SaveFileDialog { Filter = "Excel|*.xlsx", FileName = "Employees.xlsx" };
                if (sfd.ShowDialog() == true)
                {
                    using (var wb = new XLWorkbook())
                    {
                        var ws = wb.Worksheets.Add("Staff");
                        ws.Cell(1, 1).Value = "ID";
                        ws.Cell(1, 2).Value = "ФИО";
                        ws.Cell(1, 3).Value = "Должность";

                        var data = db.Сотрудникиs.ToList();
                        for (int i = 0; i < data.Count; i++)
                        {
                            ws.Cell(i + 2, 1).Value = data[i].IdСотрудника;
                            ws.Cell(i + 2, 2).Value = data[i].Фио;
                            ws.Cell(i + 2, 3).Value = data[i].Должность;
                        }
                        ws.Columns().AdjustToContents();
                        wb.SaveAs(sfd.FileName);
                        MessageBox.Show("Готово");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}