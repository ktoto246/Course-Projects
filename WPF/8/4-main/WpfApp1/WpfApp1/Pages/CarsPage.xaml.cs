using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class CarsPage : Page
    {
        private readonly CarRentalContext _context;
        private Автомобиль? _currentCar;
        private ICollectionView? _carsView;

        public CarsPage()
        {
            InitializeComponent();
            _context = new CarRentalContext();
            LoadData();
        }

        private void LoadData()
        {
            // Загружаем данные в память
            _context.Автомобили.Load();

            CmbClass.ItemsSource = new[] { "Эконом", "Комфорт", "Бизнес", "Минивэн", "Внедорожник" };
            CmbStatus.ItemsSource = new[] { "Свободен", "В аренде", "В ремонте" };

            FilterStatusCombo.ItemsSource = new[] { "Все", "Свободен", "В аренде", "В ремонте" };
            FilterStatusCombo.SelectedIndex = 0;

            CarsGrid.ItemsSource = _context.Автомобили.Local.ToObservableCollection();

            if (CarsGrid.ItemsSource != null)
            {
                _carsView = CollectionViewSource.GetDefaultView(CarsGrid.ItemsSource);
                _carsView.Filter = FilterCars;
            }
            ClearForm();
        }

        private bool FilterCars(object obj)
        {
            if (obj is not Автомобиль car)
                return false;

            string search = SearchTextBox.Text?.ToLower() ?? string.Empty;
            string filterStatus = FilterStatusCombo.SelectedItem as string ?? "Все";

            bool matchesSearch = string.IsNullOrWhiteSpace(search) ||
                                 (car.Марка_Модель != null && car.Марка_Модель.ToLower().Contains(search)) ||
                                 (car.Гос_Номер != null && car.Гос_Номер.ToLower().Contains(search));

            bool matchesStatus = filterStatus == "Все" || car.Статус == filterStatus;

            return matchesSearch && matchesStatus;
        }

        private void CarsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CarsGrid.SelectedItem is Автомобиль selected)
            {
                _currentCar = selected;
                TxtModel.Text = selected.Марка_Модель;
                TxtNumber.Text = selected.Гос_Номер;
                CmbClass.SelectedItem = selected.Класс;
                TxtPrice.Text = selected.Цена_Сутки.ToString();
                TxtYear.Text = selected.Год_Выпуска.ToString();
                TxtMileage.Text = selected.Текущий_Пробег.ToString();
                CmbStatus.SelectedItem = selected.Статус;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDataValid()) return;

            try
            {
                if (_currentCar == null)
                {
                    var newCar = new Автомобиль
                    {
                        Марка_Модель = TxtModel.Text.Trim(),
                        Гос_Номер = TxtNumber.Text.Trim(),
                        Класс = CmbClass.SelectedItem as string ?? "Эконом",
                        Цена_Сутки = decimal.Parse(TxtPrice.Text),
                        Год_Выпуска = int.Parse(TxtYear.Text),
                        Текущий_Пробег = int.Parse(TxtMileage.Text),
                        Статус = CmbStatus.SelectedItem as string ?? "Свободен"
                    };
                    _context.Автомобили.Add(newCar);
                    _currentCar = newCar;
                }
                else
                {
                    _currentCar.Марка_Модель = TxtModel.Text.Trim();
                    _currentCar.Гос_Номер = TxtNumber.Text.Trim();
                    _currentCar.Класс = CmbClass.SelectedItem as string ?? "Эконом";
                    _currentCar.Цена_Сутки = decimal.Parse(TxtPrice.Text);
                    _currentCar.Год_Выпуска = int.Parse(TxtYear.Text);
                    _currentCar.Текущий_Пробег = int.Parse(TxtMileage.Text);
                    _currentCar.Статус = CmbStatus.SelectedItem as string ?? "Свободен";
                }

                _context.SaveChanges();
                CarsGrid.Items.Refresh();
                ClearForm();
                MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCar != null && MessageBox.Show("Точно удалить машину?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.Автомобили.Remove(_currentCar);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            CarsGrid.SelectedItem = null;
            _currentCar = null;
            TxtModel.Text = string.Empty;
            TxtNumber.Text = string.Empty;
            TxtPrice.Text = string.Empty;
            TxtYear.Text = string.Empty;
            TxtMileage.Text = string.Empty;
            CmbClass.SelectedIndex = -1;
            CmbStatus.SelectedIndex = 0;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _carsView?.Refresh();
        private void FilterStatusCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => _carsView?.Refresh();

        private bool IsDataValid()
        {
            if (string.IsNullOrWhiteSpace(TxtModel.Text) || string.IsNullOrWhiteSpace(TxtNumber.Text))
            {
                MessageBox.Show("Заполни марку и гос. номер!", "Ошибка");
                return false;
            }

            if (CmbClass.SelectedItem == null || CmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Выбери класс и статус!", "Ошибка");
                return false;
            }

            if (!decimal.TryParse(TxtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть числом больше нуля!", "Ошибка");
                return false;
            }

            if (!int.TryParse(TxtYear.Text, out int year) || year < 1900 || year > DateTime.Now.Year + 1)
            {
                MessageBox.Show("Укажи адекватный год выпуска!", "Ошибка");
                return false;
            }

            if (!int.TryParse(TxtMileage.Text, out int mileage) || mileage < 0)
            {
                MessageBox.Show("Пробег не может быть отрицательным!", "Ошибка");
                return false;
            }

            return true;
        }
    }
}