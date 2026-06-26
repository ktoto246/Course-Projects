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
    public partial class JointsPage : Page
    {
        private readonly GasRegionDbContext _context = new();
        private ObservableCollection<Joint> _joints;
        private Joint? _selectedJoint;
        private bool _isEditing = false;
        private List<Welder> _welders;
        private List<Pipeline> _pipelines;
        private List<WeldingMachine> _machines;

        public JointsPage()
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
                _welders = _context.Welders.ToList();
                _pipelines = _context.Pipelines.ToList();
                _machines = _context.WeldingMachines.ToList();

                WelderComboBox.ItemsSource = _welders;
                WelderComboBox.DisplayMemberPath = "FullName";
                WelderComboBox.SelectedValuePath = "Id";

                PipelineComboBox.ItemsSource = _pipelines;
                PipelineComboBox.DisplayMemberPath = "Name";
                PipelineComboBox.SelectedValuePath = "Id";

                MachineComboBox.ItemsSource = _machines;
                MachineComboBox.DisplayMemberPath = "Model";
                MachineComboBox.SelectedValuePath = "Id";

                _joints = new ObservableCollection<Joint>(_context.Joints.Include(j => j.Welder).Include(j => j.Pipeline).Include(j => j.Machine).ToList());
                JointsDataGrid.ItemsSource = _joints;
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
                if (string.IsNullOrWhiteSpace(JointNumberTextBox.Text) ||
                    WelderComboBox.SelectedValue == null ||
                    PipelineComboBox.SelectedValue == null ||
                    MachineComboBox.SelectedValue == null ||
                    WeldingDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Заполните все обязательные поля");
                    return;
                }

                if (_isEditing && _selectedJoint != null)
                {
                    _selectedJoint.JointNumber = JointNumberTextBox.Text;
                    _selectedJoint.WelderId = (int)WelderComboBox.SelectedValue;
                    _selectedJoint.PipelineId = (int)PipelineComboBox.SelectedValue;
                    _selectedJoint.MachineId = (int)MachineComboBox.SelectedValue;
                    _selectedJoint.WeldingDate = WeldingDatePicker.SelectedDate.Value.Date;
                    _context.Joints.Update(_selectedJoint);
                }
                else
                {
                    var newJoint = new Joint
                    {
                        JointNumber = JointNumberTextBox.Text,
                        WelderId = (int)WelderComboBox.SelectedValue,
                        PipelineId = (int)PipelineComboBox.SelectedValue,
                        MachineId = (int)MachineComboBox.SelectedValue,
                        WeldingDate = WeldingDatePicker.SelectedDate.Value.Date
                    };
                    _context.Joints.Add(newJoint);
                    _joints.Add(newJoint);
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
            if (_selectedJoint == null)
            {
                MessageBox.Show("Выберите соединение для удаления");
                return;
            }

            if (MessageBox.Show("Удалить выбранное соединение?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Joints.Remove(_selectedJoint);
                    _context.SaveChanges();
                    _joints.Remove(_selectedJoint);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void JointsDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (JointsDataGrid.SelectedItem is Joint joint)
            {
                _selectedJoint = joint;
                JointNumberTextBox.Text = joint.JointNumber;
                WelderComboBox.SelectedValue = joint.WelderId;
                PipelineComboBox.SelectedValue = joint.PipelineId;
                MachineComboBox.SelectedValue = joint.MachineId;
                WeldingDatePicker.SelectedDate = joint.WeldingDate;
                _isEditing = true;
            }
        }

        private void ClearForm()
        {
            JointNumberTextBox.Text = string.Empty;
            WelderComboBox.SelectedIndex = -1;
            PipelineComboBox.SelectedIndex = -1;
            MachineComboBox.SelectedIndex = -1;
            WeldingDatePicker.SelectedDate = null;
            _selectedJoint = null;
            _isEditing = false;
        }
    }
}
