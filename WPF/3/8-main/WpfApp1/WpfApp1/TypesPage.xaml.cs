using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Model;

namespace WpfApp1
{
    public partial class TypesPage : Page
    {
        private readonly AppDbContext _db;
        private ТипСвт _current;
        private ICollectionView _view;

        public TypesPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _db.ТипыСвт.Load();
            TypesGrid.ItemsSource = _db.ТипыСвт.Local.ToObservableCollection();

            _view = CollectionViewSource.GetDefaultView(TypesGrid.ItemsSource);
            _view.Filter = (obj) => {
                var item = obj as ТипСвт;
                return string.IsNullOrWhiteSpace(SearchTextBox.Text) ||
                       (item.Название != null && item.Название.ToLower().Contains(SearchTextBox.Text.ToLower()));
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _view?.Refresh();

        private void TypesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypesGrid.SelectedItem is ТипСвт selected)
            {
                _current = selected;
                TxtName.Text = selected.Название;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text)) return;

            if (_current == null)
            {
                _current = new ТипСвт();
                _db.ТипыСвт.Add(_current);
            }
            _current.Название = TxtName.Text.Trim();
            _db.SaveChanges();
            TypesGrid.Items.Refresh();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            _current = null;
            TxtName.Clear();
            TypesGrid.SelectedItem = null;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            if (MessageBox.Show("Точно удалить эту запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.ТипыСвт.Remove(_current);
                    _db.SaveChanges();
                    ClearBtn_Click(null, null);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    MessageBox.Show("Нельзя удалить этот тип! К нему уже привязана техника. Сначала удалите или измените соответствующую технику.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
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