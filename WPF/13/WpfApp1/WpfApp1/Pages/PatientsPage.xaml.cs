using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Pages
{
    public partial class PatientsPage : Page
    {
        private readonly RegistryContext _context;
        private Patient? _currentPatient;
        private ICollectionView? _patientsView;

        public PatientsPage()
        {
            InitializeComponent();
            _context = new RegistryContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Patients.Load();
            PatientsGrid.ItemsSource = _context.Patients.Local.ToObservableCollection();

            if (PatientsGrid.ItemsSource != null)
            {
                _patientsView = CollectionViewSource.GetDefaultView(PatientsGrid.ItemsSource);
                _patientsView.Filter = o =>
                {
                    if (o is not Patient p) return false;
                    string search = SearchTextBox.Text?.ToLower() ?? string.Empty;
                    return string.IsNullOrWhiteSpace(search) || p.FullName.ToLower().Contains(search);
                };
            }
            ClearForm();
        }

        private void PatientsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientsGrid.SelectedItem is Patient selected)
            {
                _currentPatient = selected;
                TxtFullName.Text = selected.FullName;
                DpBirthDate.SelectedDate = selected.BirthDate;
                TxtPhone.Text = selected.Phone;
                TxtAddress.Text = selected.Address;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text) || DpBirthDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните ФИО и дату рождения", "Ошибка");
                return;
            }

            try
            {
                if (_currentPatient == null)
                {
                    var newPatient = new Patient
                    {
                        FullName = TxtFullName.Text.Trim(),
                        BirthDate = DpBirthDate.SelectedDate.Value,
                        Phone = TxtPhone.Text.Trim(),
                        Address = TxtAddress.Text.Trim()
                    };
                    _context.Patients.Add(newPatient);
                }
                else
                {
                    _currentPatient.FullName = TxtFullName.Text.Trim();
                    _currentPatient.BirthDate = DpBirthDate.SelectedDate.Value;
                    _currentPatient.Phone = TxtPhone.Text.Trim();
                    _currentPatient.Address = TxtAddress.Text.Trim();
                }

                _context.SaveChanges();
                PatientsGrid.Items.Refresh();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPatient != null && MessageBox.Show("Удалить пациента?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Patients.Remove(_currentPatient);
                    _context.SaveChanges();
                    ClearForm();
                }
                catch (DbUpdateException)
                {
                    _context.Entry(_currentPatient).State = EntityState.Unchanged;
                    MessageBox.Show("Нельзя удалить пациента, у которого есть история посещений.", "Ошибка");
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _patientsView?.Refresh();
        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            PatientsGrid.SelectedItem = null;
            _currentPatient = null;
            TxtFullName.Text = string.Empty;
            DpBirthDate.SelectedDate = null;
            TxtPhone.Text = string.Empty;
            TxtAddress.Text = string.Empty;
        }
    }
}