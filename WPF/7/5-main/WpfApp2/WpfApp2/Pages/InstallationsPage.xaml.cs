using Microsoft.EntityFrameworkCore;
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
    public partial class InstallationsPage : Page, INotifyPropertyChanged
    {
        private AssetDbContext _context;
        private УстановкаПО _currentItem;

        public УстановкаПО CurrentItem
        {
            get => _currentItem;
            set { _currentItem = value; OnPropertyChanged(); }
        }

        public InstallationsPage()
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
            GridData.ItemsSource = _context.УстановкиПО
                .Include(u => u.РабочееМесто)
                .Include(u => u.ПрограммноеОбеспечение)
                .ToList();

            CmbРабочиеМеста.ItemsSource = _context.РабочиеМеста.ToList();
            CmbПО.ItemsSource = _context.ПрограммноеОбеспечение.ToList();
        }

        private void GridData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridData.SelectedItem is УстановкаПО selected)
            {
                CurrentItem = new УстановкаПО
                {
                    Id = selected.Id,
                    РабочееМестоId = selected.РабочееМестоId,
                    ПрограммноеОбеспечениеId = selected.ПрограммноеОбеспечениеId,
                    ДатаУстановки = selected.ДатаУстановки
                };
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCurrentItem()) return;
            var newItem = new УстановкаПО
            {
                РабочееМестоId = CurrentItem.РабочееМестоId,
                ПрограммноеОбеспечениеId = CurrentItem.ПрограммноеОбеспечениеId,
                ДатаУстановки = CurrentItem.ДатаУстановки
            };
            _context.УстановкиПО.Add(newItem);
            SaveChangesAndReload();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;
            if (!ValidateCurrentItem()) return;

            var existing = _context.УстановкиПО.Find(CurrentItem.Id);
            if (existing != null)
            {
                existing.РабочееМестоId = CurrentItem.РабочееМестоId;
                existing.ПрограммноеОбеспечениеId = CurrentItem.ПрограммноеОбеспечениеId;
                existing.ДатаУстановки = CurrentItem.ДатаУстановки;
                SaveChangesAndReload();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;
            if (MessageBox.Show("Удалить запись об установке?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var existing = _context.УстановкиПО.Find(CurrentItem.Id);
                if (existing != null)
                {
                    _context.УстановкиПО.Remove(existing);
                    SaveChangesAndReload();
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            GridData.SelectedItem = null;
        }

        private void ClearForm() => CurrentItem = new УстановкаПО { ДатаУстановки = DateTime.Now };

        private void SaveChangesAndReload()
        {
            try { _context.SaveChanges(); LoadData(); ClearForm(); }
            catch (Exception ex) { MessageBox.Show($"Ошибка БД: {ex.Message}"); }
        }

        private bool ValidateCurrentItem()
        {
            List<string> errors = new List<string>();
            if (CurrentItem.РабочееМестоId <= 0) errors.Add("Выберите рабочее место.");
            if (CurrentItem.ПрограммноеОбеспечениеId <= 0) errors.Add("Выберите программное обеспечение.");
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