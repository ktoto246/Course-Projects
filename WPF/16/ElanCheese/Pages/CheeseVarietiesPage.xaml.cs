using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ElanCheeseApp.Models;

namespace ElanCheeseApp.Pages
{
    public partial class CheeseVarietiesPage : Page
    {
        private ElanCheeseDbContext _context;
        private ObservableCollection<CheeseVariety> _varieties;
        private CheeseVariety? _selectedVariety;

        public CheeseVarietiesPage()
        {
            InitializeComponent();
            _context = new ElanCheeseDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _varieties = new ObservableCollection<CheeseVariety>(_context.CheeseVarieties.ToList());
            VarietiesDataGrid.ItemsSource = _varieties;
        }

        private void ClearForm()
        {
            NameTextBox.Text = string.Empty;
            MaturationDaysTextBox.Text = string.Empty;
            _selectedVariety = null;
            VarietiesDataGrid.SelectedItem = null;
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || !int.TryParse(MaturationDaysTextBox.Text, out int days) || days <= 0)
            {
                MessageBox.Show("Нормально заполни название и дни (числом)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newVariety = new CheeseVariety
            {
                Name = NameTextBox.Text,
                MaturationDays = days
            };

            _context.CheeseVarieties.Add(newVariety);
            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (_selectedVariety == null)
            {
                MessageBox.Show("Сначала выбери сорт сыра в таблице!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(NameTextBox.Text) || !int.TryParse(MaturationDaysTextBox.Text, out int days) || days <= 0) return;

            _selectedVariety.Name = NameTextBox.Text;
            _selectedVariety.MaturationDays = days;

            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_selectedVariety == null) return;

            if (MessageBox.Show($"Точно удалить сорт сыра \"{_selectedVariety.Name}\"?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.CheeseVarieties.Remove(_selectedVariety);
                _context.SaveChanges();

                LoadData();
                ClearForm();
            }
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void OnVarietiesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VarietiesDataGrid.SelectedItem is CheeseVariety variety)
            {
                _selectedVariety = variety;
                NameTextBox.Text = variety.Name;
                MaturationDaysTextBox.Text = variety.MaturationDays.ToString();
            }
        }
    }
}