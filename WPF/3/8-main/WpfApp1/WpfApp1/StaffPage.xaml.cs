using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Model;

namespace WpfApp1
{
    public partial class StaffPage : Page
    {
        private AppDbContext _db;
        private Сотрудник _current;
        private ICollectionView _view;

        public StaffPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _db.Сотрудники.Load();
            StaffGrid.ItemsSource = _db.Сотрудники.Local.ToObservableCollection();

            _view = CollectionViewSource.GetDefaultView(StaffGrid.ItemsSource);
            _view.Filter = (obj) => {
                var item = obj as Сотрудник;
                return string.IsNullOrWhiteSpace(SearchTextBox.Text) ||
                       (item.ФИО != null && item.ФИО.ToLower().Contains(SearchTextBox.Text.ToLower()));
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _view?.Refresh();

        private void StaffGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StaffGrid.SelectedItem is Сотрудник selected)
            {
                _current = selected;
                TxtFio.Text = selected.ФИО;
                TxtDept.Text = selected.Отдел;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFio.Text)) return;

            if (_current == null)
            {
                _current = new Сотрудник();
                _db.Сотрудники.Add(_current);
            }
            _current.ФИО = TxtFio.Text.Trim();
            _current.Отдел = TxtDept.Text.Trim();
            _db.SaveChanges();
            StaffGrid.Items.Refresh();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            _current = null;
            TxtFio.Clear();
            TxtDept.Clear();
            StaffGrid.SelectedItem = null;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            if (MessageBox.Show("Точно удалить этого сотрудника?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Сотрудники.Remove(_current);
                    _db.SaveChanges();
                    ClearBtn_Click(null, null);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    MessageBox.Show("Нельзя удалить сотрудника! За ним числится техника. Сначала открепите технику от этого человека.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
                    _db.Entry(_current).State = EntityState.Unchanged;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}