using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class CrashPage : Page
    {
        private readonly CarRentalContext _context;
        private ДТП? _currentCrash;
        private ICollectionView? _crashView;

        public CrashPage()
        {
            InitializeComponent();
            _context = new CarRentalContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.ДТП_Записи.Include(d => d.Аренда).ThenInclude(a => a.Автомобиль).Load();
            _context.Аренды.Load();
            CmbRent.ItemsSource = _context.Аренды.Local.ToList();

            CrashGrid.ItemsSource = _context.ДТП_Записи.Local.ToObservableCollection();
            _crashView = CollectionViewSource.GetDefaultView(CrashGrid.ItemsSource);
            _crashView.Filter = FilterCrash;
            ClearForm();
        }

        private bool FilterCrash(object obj)
        {
            if (obj is not ДТП item) return false;
            string search = SearchTextBox.Text?.ToLower() ?? string.Empty;
            return string.IsNullOrWhiteSpace(search) ||
                   item.ID_Аренды.ToString().Contains(search) ||
                   (item.Описание != null && item.Описание.ToLower().Contains(search));
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDataValid()) return;
            if (CmbRent.SelectedValue == null) return;
            try
            {
                if (_currentCrash == null)
                {
                    var newItem = new ДТП
                    {
                        ID_Аренды = (int)CmbRent.SelectedValue,
                        Дата_ДТП = DpCrashDate.SelectedDate ?? DateTime.Now,
                        Описание = TxtDesc.Text,
                        Сумма_Ущерба = decimal.TryParse(TxtDamage.Text, out var d) ? d : 0,
                        Вина_Клиента = CbFault.IsChecked ?? false
                    };
                    _context.ДТП_Записи.Add(newItem);
                }
                else
                {
                    _currentCrash.ID_Аренды = (int)CmbRent.SelectedValue;
                    _currentCrash.Дата_ДТП = DpCrashDate.SelectedDate ?? DateTime.Now;
                    _currentCrash.Описание = TxtDesc.Text;
                    _currentCrash.Сумма_Ущерба = decimal.TryParse(TxtDamage.Text, out var d) ? d : 0;
                    _currentCrash.Вина_Клиента = CbFault.IsChecked ?? false;
                }
                _context.SaveChanges();
                CrashGrid.Items.Refresh();
                ClearForm();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CrashGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CrashGrid.SelectedItem is ДТП selected)
            {
                _currentCrash = selected;
                CmbRent.SelectedValue = selected.ID_Аренды;
                DpCrashDate.SelectedDate = selected.Дата_ДТП;
                TxtDamage.Text = selected.Сумма_Ущерба.ToString();
                TxtDesc.Text = selected.Описание;
                CbFault.IsChecked = selected.Вина_Клиента;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCrash != null && MessageBox.Show("Удалить?", "Вопрос", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.ДТП_Записи.Remove(_currentCrash);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _crashView?.Refresh();

        private void ClearForm()
        {
            CrashGrid.SelectedItem = null;
            _currentCrash = null;
            CmbRent.SelectedIndex = -1;
            DpCrashDate.SelectedDate = null;
            TxtDamage.Text = TxtDesc.Text = string.Empty;
            CbFault.IsChecked = false;
        }
        private bool IsDataValid()
        {
            if (CmbRent.SelectedValue == null)
            {
                MessageBox.Show("Выбери номер аренды!", "Внимание");
                return false;
            }

            if (DpCrashDate.SelectedDate == null)
            {
                MessageBox.Show("Укажи дату ДТП!", "Внимание");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtDesc.Text))
            {
                MessageBox.Show("Напиши описание аварии!", "Ошибка");
                return false;
            }

            if (!decimal.TryParse(TxtDamage.Text, out decimal damage) || damage < 0)
            {
                MessageBox.Show("Сумма ущерба должна быть числом!", "Ошибка");
                return false;
            }

            return true;
        }
    }
}