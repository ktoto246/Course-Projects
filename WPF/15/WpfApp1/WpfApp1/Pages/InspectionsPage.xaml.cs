using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class InspectionsPage : Page
    {
        private readonly GasRegionDbContext _context = new();
        private ObservableCollection<Inspection> _inspections;
        private Inspection? _selectedInspection;
        private bool _isEditing = false;
        private List<Joint> _joints;

        public InspectionsPage()
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
                _joints = _context.Joints.ToList();

                JointComboBox.ItemsSource = _joints;
                JointComboBox.DisplayMemberPath = "JointNumber";
                JointComboBox.SelectedValuePath = "Id";

                _inspections = new ObservableCollection<Inspection>(_context.Inspections.Include(i => i.Joint).ToList());
                InspectionsDataGrid.ItemsSource = _inspections;
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
                if (string.IsNullOrWhiteSpace(InspectionMethodTextBox.Text) ||
                    JointComboBox.SelectedValue == null ||
                    InspectionDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Заполните все обязательные поля");
                    return;
                }

                if (_isEditing && _selectedInspection != null)
                {
                    _selectedInspection.InspectionMethod = InspectionMethodTextBox.Text;
                    _selectedInspection.JointId = (int)JointComboBox.SelectedValue;
                    _selectedInspection.IsPassed = IsPassedCheckBox.IsChecked ?? false;
                    _selectedInspection.InspectionDate = InspectionDatePicker.SelectedDate.Value.Date;
                    _context.Inspections.Update(_selectedInspection);
                }
                else
                {
                    var newInspection = new Inspection
                    {
                        InspectionMethod = InspectionMethodTextBox.Text,
                        JointId = (int)JointComboBox.SelectedValue,
                        IsPassed = IsPassedCheckBox.IsChecked ?? false,
                        InspectionDate = InspectionDatePicker.SelectedDate.Value.Date
                    };
                    _context.Inspections.Add(newInspection);
                    _inspections.Add(newInspection);
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
            if (_selectedInspection == null)
            {
                MessageBox.Show("Выберите контроль для удаления");
                return;
            }

            if (MessageBox.Show("Удалить выбранный контроль?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Inspections.Remove(_selectedInspection);
                    _context.SaveChanges();
                    _inspections.Remove(_selectedInspection);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void InspectionsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (InspectionsDataGrid.SelectedItem is Inspection inspection)
            {
                _selectedInspection = inspection;
                InspectionMethodTextBox.Text = inspection.InspectionMethod;
                JointComboBox.SelectedValue = inspection.JointId;
                IsPassedCheckBox.IsChecked = inspection.IsPassed;
                InspectionDatePicker.SelectedDate = inspection.InspectionDate;
                _isEditing = true;
            }
        }

        private void ClearForm()
        {
            InspectionMethodTextBox.Text = string.Empty;
            JointComboBox.SelectedIndex = -1;
            IsPassedCheckBox.IsChecked = false;
            InspectionDatePicker.SelectedDate = null;
            _selectedInspection = null;
            _isEditing = false;
        }
    }
}
