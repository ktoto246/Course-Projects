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
    public partial class SoftwarePage : Page, INotifyPropertyChanged
    {
        private AssetDbContext _context;
        private ПрограммноеОбеспечение _currentItem;

        public ПрограммноеОбеспечение CurrentItem
        {
            get => _currentItem;
            set { _currentItem = value; OnPropertyChanged(); }
        }

        public SoftwarePage()
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
            GridData.ItemsSource = _context.ПрограммноеОбеспечение.ToList();
        }

        private void GridData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridData.SelectedItem is ПрограммноеОбеспечение selected)
            {
                CurrentItem = new ПрограммноеОбеспечение
                {
                    Id = selected.Id,
                    Название = selected.Название,
                    Версия = selected.Версия,
                    Разработчик = selected.Разработчик,
                    ТипЛицензии = selected.ТипЛицензии
                };
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCurrentItem()) return;
            var newItem = new ПрограммноеОбеспечение
            {
                Название = CurrentItem.Название,
                Версия = CurrentItem.Версия,
                Разработчик = CurrentItem.Разработчик,
                ТипЛицензии = CurrentItem.ТипЛицензии
            };
            _context.ПрограммноеОбеспечение.Add(newItem);
            SaveChangesAndReload();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;
            if (!ValidateCurrentItem()) return;

            var existing = _context.ПрограммноеОбеспечение.Find(CurrentItem.Id);
            if (existing != null)
            {
                existing.Название = CurrentItem.Название;
                existing.Версия = CurrentItem.Версия;
                existing.Разработчик = CurrentItem.Разработчик;
                existing.ТипЛицензии = CurrentItem.ТипЛицензии;
                SaveChangesAndReload();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;
            if (MessageBox.Show("Удалить ПО?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var existing = _context.ПрограммноеОбеспечение.Find(CurrentItem.Id);
                if (existing != null)
                {
                    _context.ПрограммноеОбеспечение.Remove(existing);
                    SaveChangesAndReload();
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            GridData.SelectedItem = null;
        }

        private void ClearForm() => CurrentItem = new ПрограммноеОбеспечение();

        private void SaveChangesAndReload()
        {
            try { _context.SaveChanges(); LoadData(); ClearForm(); }
            catch (Exception ex) { MessageBox.Show($"Ошибка БД: {ex.Message}"); }
        }

        private bool ValidateCurrentItem()
        {
            if (string.IsNullOrWhiteSpace(CurrentItem.Название))
            {
                MessageBox.Show("Название ПО обязательно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}