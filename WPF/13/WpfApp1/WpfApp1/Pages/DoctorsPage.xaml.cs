using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Pages
{
    public partial class DoctorsPage : Page
    {
        private readonly RegistryContext _context;
        private Doctor? _currentDoctor;
        private ICollectionView? _doctorsView;

        public DoctorsPage()
        {
            InitializeComponent();
            _context = new RegistryContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Doctors.Include(d => d.Specialization).Load();
            _context.Specializations.Load();

            CmbSpecialization.ItemsSource = _context.Specializations.Local.ToObservableCollection();

            var filterSpecs = _context.Specializations.ToList();
            filterSpecs.Insert(0, new Specialization { Id = 0, Name = "Все" });
            FilterSpecCombo.ItemsSource = filterSpecs;
            FilterSpecCombo.SelectedIndex = 0;

            DoctorsGrid.ItemsSource = _context.Doctors.Local.ToObservableCollection();

            if (DoctorsGrid.ItemsSource != null)
            {
                _doctorsView = CollectionViewSource.GetDefaultView(DoctorsGrid.ItemsSource);
                _doctorsView.Filter = FilterLogic;
            }
            ClearForm();
        }

        private bool FilterLogic(object obj)
        {
            if (obj is not Doctor item) return false;

            string search = SearchTextBox.Text?.ToLower() ?? string.Empty;
            var filterSpec = FilterSpecCombo.SelectedItem as Specialization;

            bool matchesSearch = string.IsNullOrWhiteSpace(search) ||
                                 item.FullName.ToLower().Contains(search);
            bool matchesSpec = filterSpec == null || filterSpec.Id == 0 || item.SpecializationId == filterSpec.Id;

            return matchesSearch && matchesSpec;
        }

        private void DoctorsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsGrid.SelectedItem is Doctor selected)
            {
                _currentDoctor = selected;
                TxtFullName.Text = selected.FullName;
                CmbSpecialization.SelectedValue = selected.SpecializationId;
                TxtCabinet.Text = selected.CabinetNumber.ToString();
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFullName.Text) || CmbSpecialization.SelectedValue == null)
            {
                MessageBox.Show("Заполните поля", "Ошибка");
                return;
            }

            if (!int.TryParse(TxtCabinet.Text, out int cabinet))
            {
                MessageBox.Show("Кабинет должен быть числом", "Ошибка");
                return;
            }

            try
            {
                if (_currentDoctor == null)
                {
                    var newDoctor = new Doctor
                    {
                        FullName = TxtFullName.Text.Trim(),
                        SpecializationId = (int)CmbSpecialization.SelectedValue,
                        CabinetNumber = cabinet
                    };
                    _context.Doctors.Add(newDoctor);
                }
                else
                {
                    _currentDoctor.FullName = TxtFullName.Text.Trim();
                    _currentDoctor.SpecializationId = (int)CmbSpecialization.SelectedValue;
                    _currentDoctor.CabinetNumber = cabinet;
                }

                _context.SaveChanges();
                DoctorsGrid.Items.Refresh();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentDoctor != null && MessageBox.Show("Удалить выбранного врача?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Doctors.Remove(_currentDoctor);
                    _context.SaveChanges();
                    ClearForm();
                }
                catch (DbUpdateException)
                {
                    _context.Entry(_currentDoctor).State = EntityState.Unchanged;
                    MessageBox.Show("Невозможно удалить врача, так как за ним числятся записи на прием.", "Ошибка");
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _doctorsView?.Refresh();
        private void FilterSpecCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => _doctorsView?.Refresh();
        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            DoctorsGrid.SelectedItem = null;
            _currentDoctor = null;
            TxtFullName.Text = string.Empty;
            CmbSpecialization.SelectedIndex = -1;
            TxtCabinet.Text = string.Empty;
        }
    }
}