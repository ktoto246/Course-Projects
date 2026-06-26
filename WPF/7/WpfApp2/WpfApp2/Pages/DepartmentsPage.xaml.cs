using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp2.Pages
{
    public partial class DepartmentsPage : Page, INotifyPropertyChanged
    {
        private AssetDbContext _context;
        private Отдел _currentItem;

        public Отдел CurrentItem
        {
            get => _currentItem;
            set { _currentItem = value; OnPropertyChanged(); }
        }

        public DepartmentsPage()
        {
            InitializeComponent();
            _context = new AssetDbContext();
            DataContext = this;
            ClearForm();
            LoadData();
        }

        private void LoadData()
        {
            _context.ChangeTracker.Clear();
            GridData.ItemsSource = _context.Отделы.ToList();
        }

        private void GridData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridData.SelectedItem is Отдел selected)
            {
                CurrentItem = new Отдел
                {
                    Id = selected.Id,
                    Название = selected.Название,
                    Телефон = selected.Телефон
                };
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCurrentItem()) return;

            var newDep = new Отдел
            {
                Название = CurrentItem.Название,
                Телефон = CurrentItem.Телефон
            };

            _context.Отделы.Add(newDep);
            SaveChangesAndReload();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;
            if (!ValidateCurrentItem()) return;

            var existingDep = _context.Отделы.Find(CurrentItem.Id);
            if (existingDep != null)
            {
                existingDep.Название = CurrentItem.Название;
                existingDep.Телефон = CurrentItem.Телефон;
                SaveChangesAndReload();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;

            if (MessageBox.Show("Точно удалить эту запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var existingDep = _context.Отделы.Find(CurrentItem.Id);
                if (existingDep != null)
                {
                    _context.Отделы.Remove(existingDep);
                    SaveChangesAndReload();
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            GridData.SelectedItem = null;
        }

        private void ClearForm() => CurrentItem = new Отдел();

        private void SaveChangesAndReload()
        {
            try
            {
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении в БД: {ex.Message}");
            }
        }

        private bool ValidateCurrentItem()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(CurrentItem.Название)) errors.Add("Название отдела не может быть пустым.");
            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}