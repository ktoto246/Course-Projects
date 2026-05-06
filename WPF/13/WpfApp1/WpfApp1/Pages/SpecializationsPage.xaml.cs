using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Pages
{
    public partial class SpecializationsPage : Page
    {
        private readonly RegistryContext _context;
        private Specialization? _currentSpec;
        private ICollectionView? _specsView;

        public SpecializationsPage()
        {
            InitializeComponent();
            _context = new RegistryContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Specializations.Load();
            SpecsGrid.ItemsSource = _context.Specializations.Local.ToObservableCollection();

            if (SpecsGrid.ItemsSource != null)
            {
                _specsView = CollectionViewSource.GetDefaultView(SpecsGrid.ItemsSource);
                _specsView.Filter = o =>
                {
                    if (o is not Specialization s) return false;
                    string search = SearchTextBox.Text?.ToLower() ?? string.Empty;
                    return string.IsNullOrWhiteSpace(search) || s.Name.ToLower().Contains(search);
                };
            }
            ClearForm();
        }

        private void SpecsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpecsGrid.SelectedItem is Specialization selected)
            {
                _currentSpec = selected;
                TxtName.Text = selected.Name;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Введите название", "Ошибка");
                return;
            }

            try
            {
                if (_currentSpec == null)
                {
                    var newSpec = new Specialization { Name = TxtName.Text.Trim() };
                    _context.Specializations.Add(newSpec);
                }
                else
                {
                    _currentSpec.Name = TxtName.Text.Trim();
                }

                _context.SaveChanges();
                SpecsGrid.Items.Refresh();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка");
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSpec != null && MessageBox.Show("Удалить специальность?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Specializations.Remove(_currentSpec);
                    _context.SaveChanges();
                    ClearForm();
                }
                catch (DbUpdateException)
                {
                    _context.Entry(_currentSpec).State = EntityState.Unchanged;
                    MessageBox.Show("Удаление невозможно: есть врачи с этой специальностью.", "Ошибка");
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _specsView?.Refresh();
        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            SpecsGrid.SelectedItem = null;
            _currentSpec = null;
            TxtName.Text = string.Empty;
        }
    }
}