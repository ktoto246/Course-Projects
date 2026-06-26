using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class TripsPage : Page
    {
        private readonly AppDbContext _context;
        private Автопробег? _currentTrip;

        public TripsPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Автомобилис.Load();
            _context.Автопробеги.Include(t => t.IdАвтоNavigation).Load();

            var vehicles = _context.Автомобилис.Local
                .Select(v => new { v.IdАвто, Display = $"{v.ГосНомер} ({v.МаркаМодель})" })
                .ToList();

            CmbVehicle.ItemsSource = vehicles;
            CmbVehicle.DisplayMemberPath = "Display";
            CmbVehicle.SelectedValuePath = "IdАвто";

            TripsGrid.ItemsSource = _context.Автопробеги.Local.ToObservableCollection();
            ClearForm();
        }

        private void TripsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripsGrid.SelectedItem is Автопробег selected)
            {
                _currentTrip = selected;
                CmbVehicle.SelectedValue = selected.IdАвто;
                DpTripDate.SelectedDate = selected.ДатаПоездки;
                TxtDistance.Text = selected.ПройденоКм.ToString();
                TxtDriver.Text = selected.Водитель;
                TxtWaybill.Text = selected.ПутевойЛист;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CmbVehicle.SelectedValue == null)
            {
                MessageBox.Show("Выбери тачку!", "Ошибка");
                return;
            }

            if (!int.TryParse(TxtDistance.Text, out int distance) || distance <= 0)
            {
                MessageBox.Show("Пробег должен быть числом больше нуля!", "Ошибка");
                return;
            }

            if (DpTripDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите дату!", "Ошибка");
                return;
            }

            try
            {
                if (_currentTrip == null)
                {
                    var newTrip = new Автопробег
                    {
                        IdАвто = (int)CmbVehicle.SelectedValue,
                        ДатаПоездки = DpTripDate.SelectedDate.Value,
                        ПройденоКм = distance,
                        Водитель = TxtDriver.Text.Trim(),
                        ПутевойЛист = TxtWaybill.Text.Trim()
                    };
                    _context.Автопробеги.Add(newTrip);
                    _currentTrip = newTrip;
                }
                else
                {
                    _currentTrip.IdАвто = (int)CmbVehicle.SelectedValue;
                    _currentTrip.ДатаПоездки = DpTripDate.SelectedDate.Value;
                    _currentTrip.ПройденоКм = distance;
                    _currentTrip.Водитель = TxtDriver.Text.Trim();
                    _currentTrip.ПутевойЛист = TxtWaybill.Text.Trim();
                }

                _context.SaveChanges();
                TripsGrid.Items.Refresh();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentTrip != null && MessageBox.Show("Удалить выбранную запись?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.Автопробеги.Remove(_currentTrip);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            TripsGrid.SelectedItem = null;
            _currentTrip = null;
            CmbVehicle.SelectedIndex = -1;
            DpTripDate.SelectedDate = DateTime.Now;
            TxtDistance.Text = string.Empty;
            TxtDriver.Text = string.Empty;
            TxtWaybill.Text = string.Empty;
        }
    }
}