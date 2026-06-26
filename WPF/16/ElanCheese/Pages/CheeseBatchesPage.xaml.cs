using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ElanCheeseApp.Models;

namespace ElanCheeseApp.Pages
{
    public partial class CheeseBatchesPage : Page
    {
        private ElanCheeseDbContext _context;
        private CheeseBatch? _selectedBatch;

        public CheeseBatchesPage()
        {
            InitializeComponent();
            _context = new ElanCheeseDbContext();
            LoadData();
            LoadComboBoxData();
        }

        private void LoadData()
        {
            var batches = _context.CheeseBatches
                .Include(b => b.Variety)
                .Include(b => b.Chamber)
                .Include(b => b.Employee)
                .ToList();
            BatchesDataGrid.ItemsSource = batches;
        }

        private void LoadComboBoxData()
        {
            VarietyComboBox.ItemsSource = _context.CheeseVarieties.ToList();
            VarietyComboBox.DisplayMemberPath = "Name";
            VarietyComboBox.SelectedValuePath = "Id";

            ChamberComboBox.ItemsSource = _context.StorageChambers.ToList();
            ChamberComboBox.DisplayMemberPath = "ChamberNumber";
            ChamberComboBox.SelectedValuePath = "Id";

            EmployeeComboBox.ItemsSource = _context.Employees.ToList();
            EmployeeComboBox.DisplayMemberPath = "FullName";
            EmployeeComboBox.SelectedValuePath = "Id";
        }

        private void ClearForm()
        {
            VarietyComboBox.SelectedIndex = -1;
            ChamberComboBox.SelectedIndex = -1;
            EmployeeComboBox.SelectedIndex = -1;
            ProductionDatePicker.SelectedDate = null;
            WeightTextBox.Text = string.Empty;
            _selectedBatch = null;
            BatchesDataGrid.SelectedItem = null; 
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (VarietyComboBox.SelectedIndex == -1 || ChamberComboBox.SelectedIndex == -1 || EmployeeComboBox.SelectedIndex == -1 || !ProductionDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Заполните все выпадающие списки и дату!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(WeightTextBox.Text, out decimal weight) || weight <= 0)
            {
                MessageBox.Show("Укажите корректный вес!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // КНОПКА ДОБАВИТЬ ТЕПЕРЬ РЕАЛЬНО ДОБАВЛЯЕТ В БАЗУ
            var newBatch = new CheeseBatch
            {
                VarietyId = (int)VarietyComboBox.SelectedValue,
                ChamberId = (int)ChamberComboBox.SelectedValue,
                EmployeeId = (int)EmployeeComboBox.SelectedValue,
                ProductionDate = ProductionDatePicker.SelectedDate.Value,
                WeightKg = weight
            };

            _context.CheeseBatches.Add(newBatch);
            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (_selectedBatch == null)
            {
                MessageBox.Show("Сначала выбери партию в таблице для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(WeightTextBox.Text, out decimal weight) || weight <= 0) return;

            _selectedBatch.VarietyId = (int)VarietyComboBox.SelectedValue;
            _selectedBatch.ChamberId = (int)ChamberComboBox.SelectedValue;
            _selectedBatch.EmployeeId = (int)EmployeeComboBox.SelectedValue;
            _selectedBatch.ProductionDate = ProductionDatePicker.SelectedDate.Value;
            _selectedBatch.WeightKg = weight;

            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_selectedBatch == null) return;

            if (MessageBox.Show($"Точно удалить партию №{_selectedBatch.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.CheeseBatches.Remove(_selectedBatch);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void OnBatchesSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BatchesDataGrid.SelectedItem is CheeseBatch batch)
            {
                _selectedBatch = batch;
                VarietyComboBox.SelectedValue = batch.VarietyId;
                ChamberComboBox.SelectedValue = batch.ChamberId;
                EmployeeComboBox.SelectedValue = batch.EmployeeId;
                ProductionDatePicker.SelectedDate = batch.ProductionDate;
                WeightTextBox.Text = batch.WeightKg.ToString();
            }
        }
    }
}