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
    public partial class HardwarePage : Page, INotifyPropertyChanged
    {
        private AssetDbContext _context;
        private АппаратноеОбеспечение _currentItem;

        public АппаратноеОбеспечение CurrentItem
        {
            get => _currentItem;
            set { _currentItem = value; OnPropertyChanged(); }
        }

        public HardwarePage()
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
            GridData.ItemsSource = _context.АппаратноеОбеспечение.Include(a => a.РабочееМесто).ToList();
            CmbРабочиеМеста.ItemsSource = _context.РабочиеМеста.ToList();
        }

        private void GridData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridData.SelectedItem is АппаратноеОбеспечение selected)
            {
                CurrentItem = new АппаратноеОбеспечение
                {
                    Id = selected.Id,
                    Название = selected.Название,
                    Производитель = selected.Производитель,
                    СерийныйНомер = selected.СерийныйНомер,
                    ДатаПриобретения = selected.ДатаПриобретения,
                    РабочееМестоId = selected.РабочееМестоId
                };
            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateCurrentItem()) return;
            var newItem = new АппаратноеОбеспечение
            {
                Название = CurrentItem.Название,
                Производитель = CurrentItem.Производитель,
                СерийныйНомер = CurrentItem.СерийныйНомер,
                ДатаПриобретения = CurrentItem.ДатаПриобретения,
                РабочееМестоId = CurrentItem.РабочееМестоId
            };
            _context.АппаратноеОбеспечение.Add(newItem);
            SaveChangesAndReload();
        }

        private void UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;
            if (!ValidateCurrentItem()) return;

            var existing = _context.АппаратноеОбеспечение.Find(CurrentItem.Id);
            if (existing != null)
            {
                existing.Название = CurrentItem.Название;
                existing.Производитель = CurrentItem.Производитель;
                existing.СерийныйНомер = CurrentItem.СерийныйНомер;
                existing.ДатаПриобретения = CurrentItem.ДатаПриобретения;
                existing.РабочееМестоId = CurrentItem.РабочееМестоId;
                SaveChangesAndReload();
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentItem.Id == 0) return;
            if (MessageBox.Show("Удалить выбранное оборудование?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var existing = _context.АппаратноеОбеспечение.Find(CurrentItem.Id);
                if (existing != null)
                {
                    _context.АппаратноеОбеспечение.Remove(existing);
                    SaveChangesAndReload();
                }
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            GridData.SelectedItem = null;
        }

        private void ClearForm() => CurrentItem = new АппаратноеОбеспечение { ДатаПриобретения = DateTime.Now };

        private void SaveChangesAndReload()
        {
            try { _context.SaveChanges(); LoadData(); ClearForm(); }
            catch (Exception ex) { MessageBox.Show($"Ошибка БД: {ex.Message}"); }
        }

        private bool ValidateCurrentItem()
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(CurrentItem.Название)) errors.Add("Название обязательно.");
            if (CurrentItem.РабочееМестоId == null || CurrentItem.РабочееМестоId <= 0) errors.Add("Привяжите к рабочему месту.");
            if (errors.Count > 0)
            {
                MessageBox.Show(string.Join("\n", errors), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}