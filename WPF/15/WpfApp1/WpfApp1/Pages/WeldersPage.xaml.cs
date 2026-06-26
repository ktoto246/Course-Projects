using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class WeldersPage : Page
    {
        private readonly GasRegionDbContext _context = new();
        private ObservableCollection<Welder> _welders;
        private Welder? _selectedWelder;
        private bool _isEditing = false;

        public WeldersPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _welders = new ObservableCollection<Welder>(_context.Welders.ToList());
                WeldersDataGrid.ItemsSource = _welders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FullNameTextBox.Text) ||
                    string.IsNullOrWhiteSpace(StampNumberTextBox.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля");
                    return;
                }

                if (_isEditing && _selectedWelder != null)
                {
                    _selectedWelder.FullName = FullNameTextBox.Text;
                    _selectedWelder.StampNumber = StampNumberTextBox.Text;
                    _context.Welders.Update(_selectedWelder);
                }
                else
                {
                    var newWelder = new Welder
                    {
                        FullName = FullNameTextBox.Text,
                        StampNumber = StampNumberTextBox.Text
                    };
                    _context.Welders.Add(newWelder);
                    _welders.Add(newWelder);
                }

                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWelder == null)
            {
                MessageBox.Show("Выберите сварщика для удаления");
                return;
            }

            if (MessageBox.Show("Удалить выбранного сварщика?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Welders.Remove(_selectedWelder);
                    _context.SaveChanges();
                    _welders.Remove(_selectedWelder);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void WeldersDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WeldersDataGrid.SelectedItem is Welder welder)
            {
                _selectedWelder = welder;
                FullNameTextBox.Text = welder.FullName;
                StampNumberTextBox.Text = welder.StampNumber;
                _isEditing = true;
            }
        }

        private void ClearForm()
        {
            FullNameTextBox.Text = string.Empty;
            StampNumberTextBox.Text = string.Empty;
            _selectedWelder = null;
            _isEditing = false;
        }
    }
}
