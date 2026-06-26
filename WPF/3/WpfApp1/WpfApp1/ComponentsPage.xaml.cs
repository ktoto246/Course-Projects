using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Model;

namespace WpfApp1
{
    public partial class ComponentsPage : Page
    {
        private AppDbContext _db;
        private Комплектующее _current;
        private ICollectionView _view;

        public ComponentsPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _db.Комплектующие.Include(c => c.Техника).Load();
            _db.Техника.Load();

            CompGrid.ItemsSource = _db.Комплектующие.Local.ToObservableCollection();
            CmbTech.ItemsSource = _db.Техника.Local.ToObservableCollection();

            _view = CollectionViewSource.GetDefaultView(CompGrid.ItemsSource);
            _view.Filter = (obj) => {
                var item = obj as Комплектующее;
                return string.IsNullOrWhiteSpace(SearchTextBox.Text) ||
                       (item.Модель != null && item.Модель.ToLower().Contains(SearchTextBox.Text.ToLower())) ||
                       (item.Серийный_номер != null && item.Серийный_номер.ToLower().Contains(SearchTextBox.Text.ToLower()));
            };
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _view?.Refresh();

        private void CompGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CompGrid.SelectedItem is Комплектующее selected)
            {
                _current = selected;
                TxtName.Text = selected.Название;
                TxtModel.Text = selected.Модель;
                TxtSerial.Text = selected.Серийный_номер;
                CmbTech.SelectedValue = selected.ID_Техники;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text)) return;

            if (_current == null)
            {
                _current = new Комплектующее();
                _db.Комплектующие.Add(_current);
            }

            _current.Название = TxtName.Text.Trim();
            _current.Модель = TxtModel.Text.Trim();
            _current.Серийный_номер = TxtSerial.Text.Trim();
            _current.ID_Техники = (int?)CmbTech.SelectedValue;

            _db.SaveChanges();
            CompGrid.Items.Refresh();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            _current = null;
            TxtName.Clear();
            TxtModel.Clear();
            TxtSerial.Clear();
            CmbTech.SelectedIndex = -1;
            CompGrid.SelectedItem = null;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            if (MessageBox.Show("Точно удалить эту деталь?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Комплектующие.Remove(_current);
                    _db.SaveChanges();
                    ClearBtn_Click(null, null);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    MessageBox.Show("Произошла ошибка при удалении из базы данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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