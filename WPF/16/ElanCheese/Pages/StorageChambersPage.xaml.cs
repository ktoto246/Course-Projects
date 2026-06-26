using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ElanCheeseApp.Models;

namespace ElanCheeseApp.Pages
{
    public partial class StorageChambersPage : Page
    {
        private ElanCheeseDbContext _context;
        private ObservableCollection<StorageChamber> _chambers;
        private StorageChamber? _selectedChamber;

        public StorageChambersPage()
        {
            InitializeComponent();
            _context = new ElanCheeseDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _chambers = new ObservableCollection<StorageChamber>(_context.StorageChambers.ToList());
            ChambersDataGrid.ItemsSource = _chambers;
        }

        private void ClearForm()
        {
            ChamberNumberTextBox.Text = string.Empty;
            TemperatureTextBox.Text = string.Empty;
            HumidityTextBox.Text = string.Empty;
            _selectedChamber = null;
            ChambersDataGrid.SelectedItem = null;
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ChamberNumberTextBox.Text, out int num) || num <= 0 ||
                !decimal.TryParse(TemperatureTextBox.Text, out decimal temp) ||
                !decimal.TryParse(HumidityTextBox.Text, out decimal hum))
            {
                MessageBox.Show("Заполни все поля корректными числами!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newChamber = new StorageChamber
            {
                ChamberNumber = num,
                Temperature = temp,
                Humidity = hum
            };

            _context.StorageChambers.Add(newChamber);
            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (_selectedChamber == null)
            {
                MessageBox.Show("Сначала выбери камеру в таблице!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(ChamberNumberTextBox.Text, out int num) || num <= 0 ||
                !decimal.TryParse(TemperatureTextBox.Text, out decimal temp) ||
                !decimal.TryParse(HumidityTextBox.Text, out decimal hum)) return;

            _selectedChamber.ChamberNumber = num;
            _selectedChamber.Temperature = temp;
            _selectedChamber.Humidity = hum;

            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_selectedChamber == null) return;

            if (MessageBox.Show($"Точно удалить камеру №{_selectedChamber.ChamberNumber}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.StorageChambers.Remove(_selectedChamber);
                _context.SaveChanges();

                LoadData();
                ClearForm();
            }
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void OnChambersSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ChambersDataGrid.SelectedItem is StorageChamber chamber)
            {
                _selectedChamber = chamber;
                ChamberNumberTextBox.Text = chamber.ChamberNumber.ToString();
                TemperatureTextBox.Text = chamber.Temperature.ToString();
                HumidityTextBox.Text = chamber.Humidity.ToString();
            }
        }
    }
}