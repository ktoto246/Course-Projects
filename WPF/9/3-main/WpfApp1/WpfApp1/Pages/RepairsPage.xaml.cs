using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class RepairsPage : Page
    {
        private readonly AppDbContext _context;
        private ПлановыеРемонты? _currentRepair;

        public RepairsPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Автомобилис.Load();
            _context.ПлановыеРемонтыs.Include(r => r.IdАвтоNavigation).Load();

            var vehicles = _context.Автомобилис.Local
                .Select(v => new { v.IdАвто, Display = $"{v.ГосНомер} ({v.МаркаМодель})" })
                .ToList();

            CmbVehicle.ItemsSource = vehicles;
            CmbVehicle.DisplayMemberPath = "Display";
            CmbVehicle.SelectedValuePath = "IdАвто";

            CmbStatus.ItemsSource = new[] { "Запланирован", "В процессе", "Выполнен", "Отменен" };
            CmbStatus.SelectedIndex = 0;

            RepairsGrid.ItemsSource = _context.ПлановыеРемонтыs.Local.ToObservableCollection();
            ClearForm();
        }

        private void RepairsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RepairsGrid.SelectedItem is ПлановыеРемонты selected)
            {
                _currentRepair = selected;
                CmbVehicle.SelectedValue = selected.IdАвто;
                TxtRepairType.Text = selected.ТипРемонта;
                CmbStatus.SelectedItem = selected.Статус;

                TxtPlanMileage.Text = selected.ПлановыйПробег?.ToString() ?? "";
                DpPlanDate.SelectedDate = selected.ПлановаяДата;
                DpActualDate.SelectedDate = selected.ФактическаяДата;
                TxtDescription.Text = selected.Описание;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CmbVehicle.SelectedValue == null)
            {
                MessageBox.Show("Выберите автомобиль из списка.", "Ошибка");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtRepairType.Text))
            {
                MessageBox.Show("Введите тип ремонта!", "Ошибка");
                return;
            }

            int? planMileage = null;
            if (!string.IsNullOrWhiteSpace(TxtPlanMileage.Text))
            {
                if (int.TryParse(TxtPlanMileage.Text, out int m))
                    planMileage = m;
                else
                {
                    MessageBox.Show("Плановый пробег должен быть числом!", "Ошибка");
                    return;
                }
            }

            try
            {
                if (_currentRepair == null)
                {
                    var newRepair = new ПлановыеРемонты
                    {
                        IdАвто = (int)CmbVehicle.SelectedValue,
                        ТипРемонта = TxtRepairType.Text.Trim(),
                        Статус = CmbStatus.SelectedItem as string,
                        ПлановыйПробег = planMileage,
                        ПлановаяДата = DpPlanDate.SelectedDate,
                        ФактическаяДата = DpActualDate.SelectedDate,
                        Описание = TxtDescription.Text.Trim()
                    };
                    _context.ПлановыеРемонтыs.Add(newRepair);
                    _currentRepair = newRepair;
                }
                else
                {
                    _currentRepair.IdАвто = (int)CmbVehicle.SelectedValue;
                    _currentRepair.ТипРемонта = TxtRepairType.Text.Trim();
                    _currentRepair.Статус = CmbStatus.SelectedItem as string;
                    _currentRepair.ПлановыйПробег = planMileage;
                    _currentRepair.ПлановаяДата = DpPlanDate.SelectedDate;
                    _currentRepair.ФактическаяДата = DpActualDate.SelectedDate;
                    _currentRepair.Описание = TxtDescription.Text.Trim();
                }

                _context.SaveChanges();
                RepairsGrid.Items.Refresh();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRepair != null && MessageBox.Show("Удалить этот ремонт?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.ПлановыеРемонтыs.Remove(_currentRepair);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            RepairsGrid.SelectedItem = null;
            _currentRepair = null;
            CmbVehicle.SelectedIndex = -1;
            TxtRepairType.Text = string.Empty;
            CmbStatus.SelectedIndex = 0;
            TxtPlanMileage.Text = string.Empty;
            DpPlanDate.SelectedDate = null;
            DpActualDate.SelectedDate = null;
            TxtDescription.Text = string.Empty;
        }
    }
}