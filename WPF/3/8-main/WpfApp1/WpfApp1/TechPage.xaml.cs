using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfApp1.Model;

namespace WpfApp1
{
    public partial class TechPage : Page
    {
        private AppDbContext _db;
        private Техника _current;
        private ICollectionView _view;

        public TechPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _db.Техника.Include(t => t.ТипСвт).Include(t => t.Сотрудник).Load();
            _db.ТипыСвт.Load();
            _db.Сотрудники.Load();

            TechGrid.ItemsSource = _db.Техника.Local.ToObservableCollection();
            CmbType.ItemsSource = _db.ТипыСвт.Local.ToObservableCollection();
            CmbStaff.ItemsSource = _db.Сотрудники.Local.ToObservableCollection();

            _view = CollectionViewSource.GetDefaultView(TechGrid.ItemsSource);
            _view.Filter = (obj) =>
            {
                var item = obj as Техника;
                if (item == null) return false;

                string search = SearchBox.Text?.ToLower() ?? "";
                bool matchesSearch = string.IsNullOrWhiteSpace(search) ||
                                     (item.Модель != null && item.Модель.ToLower().Contains(search)) ||
                                     (item.Инвентарный_номер != null && item.Инвентарный_номер.ToLower().Contains(search));

                var selectedType = TypeFilterCombo.SelectedItem as ТипСвт;
                bool matchesType = selectedType == null || selectedType.Название == "Все типы" || item.ID_Типа == selectedType.IDТипа;

                return matchesSearch && matchesType;
            };

            var types = _db.ТипыСвт.Local.ToList();
            var filterList = new List<ТипСвт> { new ТипСвт { Название = "Все типы" } };
            filterList.AddRange(types);
            TypeFilterCombo.ItemsSource = filterList;
            TypeFilterCombo.SelectedIndex = 0;
        }

        private void FilterChanged(object sender, System.EventArgs e) => _view?.Refresh();

        private void TechGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TechGrid.SelectedItem is Техника selected)
            {
                _current = selected;
                TxtInv.Text = selected.Инвентарный_номер;
                TxtModel.Text = selected.Модель;
                CmbType.SelectedValue = selected.ID_Типа;
                CmbStaff.SelectedValue = selected.ID_Сотрудника;

                CmbStatus.SelectedIndex = -1;
                foreach (ComboBoxItem item in CmbStatus.Items)
                {
                    if (item.Content.ToString() == selected.Статус)
                    {
                        CmbStatus.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtInv.Text) || string.IsNullOrWhiteSpace(TxtModel.Text) || CmbType.SelectedValue == null)
            {
                MessageBox.Show("Заполни инвентарник, модель и тип!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_current == null)
            {
                _current = new Техника();
                _db.Техника.Add(_current);
            }

            _current.Инвентарный_номер = TxtInv.Text.Trim();
            _current.Модель = TxtModel.Text.Trim();
            _current.ID_Типа = (int)CmbType.SelectedValue;
            _current.ID_Сотрудника = (int?)CmbStaff.SelectedValue;

            if (CmbStatus.SelectedItem is ComboBoxItem selectedStatus)
            {
                _current.Статус = selectedStatus.Content.ToString();
            }

            _db.SaveChanges();
            TechGrid.Items.Refresh();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            _current = null;
            TxtInv.Clear();
            TxtModel.Clear();
            CmbType.SelectedIndex = -1;
            CmbStaff.SelectedIndex = -1;
            CmbStatus.SelectedIndex = -1;
            TechGrid.SelectedItem = null;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_current == null) return;

            if (MessageBox.Show($"Удалить технику {_current.Инвентарный_номер}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _db.Техника.Remove(_current);
                    _db.SaveChanges();
                    ClearBtn_Click(null, null);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    MessageBox.Show("Нельзя удалить технику! К ней привязаны комплектующие. Сначала открепите или удалите детали из этой техники.", "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
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