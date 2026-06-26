using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using ElanCheeseApp.Models;

namespace ElanCheeseApp.Pages
{
    public partial class QualityInspectionsPage : Page
    {
        private ElanCheeseDbContext _context;
        private QualityInspection? _selectedInspection;

        public QualityInspectionsPage()
        {
            InitializeComponent();
            _context = new ElanCheeseDbContext();
            LoadData();
            LoadComboBoxData();
        }

        private void LoadData()
        {
            var inspections = _context.QualityInspections
                .Include(q => q.Batch)
                .ThenInclude(b => b.Variety)
                .ToList();
            InspectionsDataGrid.ItemsSource = inspections;
        }

        private void LoadComboBoxData()
        {
            BatchComboBox.ItemsSource = _context.CheeseBatches
                .Include(b => b.Variety)
                .ToList();
            BatchComboBox.SelectedValuePath = "Id";
        }

        private void ClearForm()
        {
            BatchComboBox.SelectedIndex = -1;
            InspectionDateDatePicker.SelectedDate = null;
            StatusTextBox.Text = string.Empty;
            NotesTextBox.Text = string.Empty;
            _selectedInspection = null;
            InspectionsDataGrid.SelectedItem = null;
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (BatchComboBox.SelectedIndex == -1 || !InspectionDateDatePicker.SelectedDate.HasValue || string.IsNullOrWhiteSpace(StatusTextBox.Text))
            {
                MessageBox.Show("Заполни все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newInspection = new QualityInspection
            {
                BatchId = (int)BatchComboBox.SelectedValue,
                InspectionDate = InspectionDateDatePicker.SelectedDate.Value,
                Status = StatusTextBox.Text,
                Notes = NotesTextBox.Text
            };

            _context.QualityInspections.Add(newInspection);
            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (_selectedInspection == null)
            {
                MessageBox.Show("Сначала выбери запись в таблице!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _selectedInspection.BatchId = (int)BatchComboBox.SelectedValue;
            _selectedInspection.InspectionDate = InspectionDateDatePicker.SelectedDate.Value;
            _selectedInspection.Status = StatusTextBox.Text;
            _selectedInspection.Notes = NotesTextBox.Text;

            _context.SaveChanges();

            LoadData();
            ClearForm();
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (_selectedInspection == null) return;

            if (MessageBox.Show("Точно удалить проверку?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                _context.QualityInspections.Remove(_selectedInspection);
                _context.SaveChanges();

                LoadData();
                ClearForm();
            }
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void OnInspectionsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InspectionsDataGrid.SelectedItem is QualityInspection inspection)
            {
                _selectedInspection = inspection;
                BatchComboBox.SelectedValue = inspection.BatchId;
                InspectionDateDatePicker.SelectedDate = inspection.InspectionDate;
                StatusTextBox.Text = inspection.Status;
                NotesTextBox.Text = inspection.Notes ?? string.Empty;
            }
        }
    }
}