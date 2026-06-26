using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class VehiclesPage : Page
    {
        private readonly AppDbContext _context;
        private Автомобили? _currentVehicle;
        private ICollectionView? _vehiclesView;

        public VehiclesPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Автомобилис.Load();

            CmbStatus.ItemsSource = new[] { "Активен", "В ремонте", "Списан" };
            CmbStatus.SelectedIndex = 0;

            FilterStatusCombo.ItemsSource = new[] { "Все", "Активен", "В ремонте", "Списан" };
            FilterStatusCombo.SelectedIndex = 0;

            VehiclesGrid.ItemsSource = _context.Автомобилис.Local.ToObservableCollection();

            if (VehiclesGrid.ItemsSource != null)
            {
                _vehiclesView = CollectionViewSource.GetDefaultView(VehiclesGrid.ItemsSource);
                _vehiclesView.Filter = FilterVehicles;
            }
            ClearForm();
        }

        private bool FilterVehicles(object obj)
        {
            if (obj is not Автомобили item)
                return false;

            string search = SearchTextBox.Text?.ToLower() ?? string.Empty;
            string filterStatus = FilterStatusCombo.SelectedItem as string ?? "Все";

            bool matchesSearch = string.IsNullOrWhiteSpace(search) ||
                                 (item.ГосНомер != null && item.ГосНомер.ToLower().Contains(search)) ||
                                 (item.МаркаМодель != null && item.МаркаМодель.ToLower().Contains(search));

            bool matchesStatus = filterStatus == "Все" || item.Статус == filterStatus;

            return matchesSearch && matchesStatus;
        }

        private void VehiclesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VehiclesGrid.SelectedItem is Автомобили selected)
            {
                _currentVehicle = selected;
                TxtLicensePlate.Text = selected.ГосНомер;
                TxtBrandModel.Text = selected.МаркаМодель;
                TxtYear.Text = selected.ГодВыпуска.ToString();
                TxtMileage.Text = selected.ТекущийПробег?.ToString() ?? "0";
                CmbStatus.SelectedItem = selected.Статус;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string plate = TxtLicensePlate.Text.Trim();
            string brand = TxtBrandModel.Text.Trim();
            string yearStr = TxtYear.Text.Trim();
            string mileageStr = TxtMileage.Text.Trim();
            string status = CmbStatus.SelectedItem as string ?? "Активен";

            if (!IsDataValid(plate, brand, yearStr, mileageStr, out int year, out int mileage))
                return;

            try
            {
                if (_currentVehicle == null)
                {
                    var newVehicle = new Автомобили
                    {
                        ГосНомер = plate,
                        МаркаМодель = brand,
                        ГодВыпуска = year,
                        ТекущийПробег = mileage,
                        Статус = status
                    };
                    _context.Автомобилис.Add(newVehicle);
                    _currentVehicle = newVehicle;
                }
                else
                {
                    _currentVehicle.ГосНомер = plate;
                    _currentVehicle.МаркаМодель = brand;
                    _currentVehicle.ГодВыпуска = year;
                    _currentVehicle.ТекущийПробег = mileage;
                    _currentVehicle.Статус = status;
                }

                _context.SaveChanges();
                VehiclesGrid.Items.Refresh();
                ClearForm();
                MessageBox.Show("Автомабиль успешно сохранена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении (возможно такой гос. номер уже есть):\n{ex.Message}", "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentVehicle != null && MessageBox.Show($"Удалить данный автомобиль из базы данных? {_currentVehicle.ГосНомер} из базы данных?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.Автомобилис.Remove(_currentVehicle);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            VehiclesGrid.SelectedItem = null;
            _currentVehicle = null;
            TxtLicensePlate.Text = string.Empty;
            TxtBrandModel.Text = string.Empty;
            TxtYear.Text = string.Empty;
            TxtMileage.Text = string.Empty;
            CmbStatus.SelectedIndex = 0;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _vehiclesView?.Refresh();
        private void FilterStatusCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => _vehiclesView?.Refresh();

        private bool IsDataValid(string plate, string brand, string yearStr, string mileageStr, out int year, out int mileage)
        {
            year = 0;
            mileage = 0;

            if (string.IsNullOrWhiteSpace(plate))
            {
                MessageBox.Show("Введи гос. номер!", "Ошибка");
                return false;
            }

            if (string.IsNullOrWhiteSpace(brand))
            {
                MessageBox.Show("Укажи марку и модель!", "Ошибка");
                return false;
            }

            if (!int.TryParse(yearStr, out year) || year < 1950 || year > DateTime.Now.Year + 1)
            {
                MessageBox.Show("Некорректный год выпуска!", "Ошибка");
                return false;
            }

            if (!int.TryParse(mileageStr, out mileage) || mileage < 0)
            {
                MessageBox.Show("Пробег должен быть числом и не может быть отрицательным!", "Ошибка");
                return false;
            }

            return true;
        }
    }
}