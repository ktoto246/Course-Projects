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
    public partial class WeldingMachinesPage : Page
    {
        private readonly GasRegionDbContext _context = new();
        private ObservableCollection<WeldingMachine> _machines;
        private WeldingMachine? _selectedMachine;
        private bool _isEditing = false;

        public WeldingMachinesPage()
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
                _machines = new ObservableCollection<WeldingMachine>(_context.WeldingMachines.ToList());
                MachinesDataGrid.ItemsSource = _machines;
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
                if (string.IsNullOrWhiteSpace(InventoryNumberTextBox.Text) ||
                    string.IsNullOrWhiteSpace(ModelTextBox.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля");
                    return;
                }

                if (_isEditing && _selectedMachine != null)
                {
                    _selectedMachine.InventoryNumber = InventoryNumberTextBox.Text;
                    _selectedMachine.Model = ModelTextBox.Text;
                    _context.WeldingMachines.Update(_selectedMachine);
                }
                else
                {
                    var newMachine = new WeldingMachine
                    {
                        InventoryNumber = InventoryNumberTextBox.Text,
                        Model = ModelTextBox.Text
                    };
                    _context.WeldingMachines.Add(newMachine);
                    _machines.Add(newMachine);
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
            if (_selectedMachine == null)
            {
                MessageBox.Show("Выберите сварочный аппарат для удаления");
                return;
            }

            if (MessageBox.Show("Удалить выбранный сварочный аппарат?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.WeldingMachines.Remove(_selectedMachine);
                    _context.SaveChanges();
                    _machines.Remove(_selectedMachine);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void MachinesDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MachinesDataGrid.SelectedItem is WeldingMachine machine)
            {
                _selectedMachine = machine;
                InventoryNumberTextBox.Text = machine.InventoryNumber;
                ModelTextBox.Text = machine.Model;
                _isEditing = true;
            }
        }

        private void ClearForm()
        {
            InventoryNumberTextBox.Text = string.Empty;
            ModelTextBox.Text = string.Empty;
            _selectedMachine = null;
            _isEditing = false;
        }
    }
}
