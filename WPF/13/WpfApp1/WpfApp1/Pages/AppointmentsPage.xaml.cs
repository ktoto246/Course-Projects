using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Pages
{
    public partial class AppointmentsPage : Page
    {
        private readonly RegistryContext _context;
        private Appointment? _currentAppointment;
        private ICollectionView? _appointmentsView;

        public AppointmentsPage()
        {
            InitializeComponent();
            _context = new RegistryContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Appointments.Include(a => a.Patient).Include(a => a.Doctor).Load();
            _context.Patients.Load();
            _context.Doctors.Load();

            CmbPatient.ItemsSource = _context.Patients.Local.ToObservableCollection();
            CmbDoctor.ItemsSource = _context.Doctors.Local.ToObservableCollection();

            var statuses = new[] { "Запланировано", "Завершено", "Отменено" };
            CmbStatus.ItemsSource = statuses;
            FilterStatusCombo.ItemsSource = new[] { "Все" }.Concat(statuses).ToList();
            FilterStatusCombo.SelectedIndex = 0;

            AppointmentsGrid.ItemsSource = _context.Appointments.Local.ToObservableCollection();

            if (AppointmentsGrid.ItemsSource != null)
            {
                _appointmentsView = CollectionViewSource.GetDefaultView(AppointmentsGrid.ItemsSource);
                _appointmentsView.Filter = FilterLogic;
            }
            ClearForm();
        }

        private bool FilterLogic(object obj)
        {
            if (obj is not Appointment item) return false;

            string search = SearchTextBox.Text?.ToLower() ?? string.Empty;
            string filterStatus = FilterStatusCombo.SelectedItem as string ?? "Все";

            bool matchesSearch = string.IsNullOrWhiteSpace(search) ||
                                 item.Patient.FullName.ToLower().Contains(search);
            bool matchesStatus = filterStatus == "Все" || item.Status == filterStatus;

            return matchesSearch && matchesStatus;
        }

        private void AppointmentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppointmentsGrid.SelectedItem is Appointment selected)
            {
                _currentAppointment = selected;
                CmbPatient.SelectedValue = selected.PatientId;
                CmbDoctor.SelectedValue = selected.DoctorId;
                DpDate.SelectedDate = selected.AppointmentDate;
                CmbStatus.SelectedItem = selected.Status;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CmbPatient.SelectedValue == null || CmbDoctor.SelectedValue == null || DpDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните данные", "Ошибка");
                return;
            }

            try
            {
                if (_currentAppointment == null)
                {
                    var newApp = new Appointment
                    {
                        PatientId = (int)CmbPatient.SelectedValue,
                        DoctorId = (int)CmbDoctor.SelectedValue,
                        AppointmentDate = DpDate.SelectedDate.Value,
                        Status = CmbStatus.SelectedItem as string ?? "Запланировано"
                    };
                    _context.Appointments.Add(newApp);
                }
                else
                {
                    _currentAppointment.PatientId = (int)CmbPatient.SelectedValue;
                    _currentAppointment.DoctorId = (int)CmbDoctor.SelectedValue;
                    _currentAppointment.AppointmentDate = DpDate.SelectedDate.Value;
                    _currentAppointment.Status = CmbStatus.SelectedItem as string ?? "Запланировано";
                }

                _context.SaveChanges();
                AppointmentsGrid.Items.Refresh();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentAppointment != null && MessageBox.Show("Удалить?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.Appointments.Remove(_currentAppointment);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _appointmentsView?.Refresh();
        private void FilterStatusCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => _appointmentsView?.Refresh();
        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            AppointmentsGrid.SelectedItem = null;
            _currentAppointment = null;
            CmbPatient.SelectedIndex = -1;
            CmbDoctor.SelectedIndex = -1;
            DpDate.SelectedDate = DateTime.Now;
            CmbStatus.SelectedIndex = 0;
        }
    }
}