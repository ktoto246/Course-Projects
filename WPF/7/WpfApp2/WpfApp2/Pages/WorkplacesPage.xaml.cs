using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp2.Pages
{
    public partial class WorkplacesPage : Page, INotifyPropertyChanged
    {
        private AssetDbContext _context;
        private РабочееМесто _currentItem;

        public РабочееМесто CurrentItem
        {
            get => _currentItem;
            set { _currentItem = value; OnPropertyChanged(); }
        }

        public WorkplacesPage()
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
            GridData.ItemsSource = _context.РабочиеМеста.Include(r => r.Отдел).ToList();
            CmbОтделы.ItemsSource = _context.Отделы.ToList();
        }

        private void GridData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridData.SelectedItem is РабочееМесто selected)
            {
                CurrentItem = new РабочееМесто
                {
                    Id = selected.Id,
                    ИнвентарныйНомер = selected.ИнвентарныйНомер,
                    Расположение = selected.Расположение,
                    ОтделId = selected.ОтделId
                };
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCurrentItem()) return;
            var newItem = new РабочееМесто
            {
                ИнвентарныйНомер = CurrentItem.ИнвентарныйНомер,
                Расположение = CurrentItem.Расположение,
                ОтделId = CurrentItem.ОтделId
            };
            _context.РабочиеМеста.Add(newItem);
            SaveChangesAndReload();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;
            if (!ValidateCurrentItem()) return;

            var existing = _context.РабочиеМеста.Find(CurrentItem.Id);
            if (existing != null)
            {
                existing.ИнвентарныйНомер = CurrentItem.ИнвентарныйНомер;
                existing.Расположение = CurrentItem.Расположение;
                existing.ОтделId = CurrentItem.ОтделId;
                SaveChangesAndReload();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;
            if (MessageBox.Show("Удалить выбранное рабочее место?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var existing = _context.РабочиеМеста.Find(CurrentItem.Id);
                if (existing != null)
                {
                    _context.РабочиеМеста.Remove(existing);
                    SaveChangesAndReload();
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            GridData.SelectedItem = null;
        }

        private void ClearForm() => CurrentItem = new РабочееМесто();

        private void SaveChangesAndReload()
        {
            try { _context.SaveChanges(); LoadData(); ClearForm(); }
            catch (Exception ex) { MessageBox.Show($"Ошибка БД: {ex.Message}"); }
        }

        private bool ValidateCurrentItem()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(CurrentItem.ИнвентарныйНомер)) errors.Add("Инвентарный номер обязателен.");
            if (CurrentItem.ОтделId <= 0) errors.Add("Необходимо выбрать отдел.");
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