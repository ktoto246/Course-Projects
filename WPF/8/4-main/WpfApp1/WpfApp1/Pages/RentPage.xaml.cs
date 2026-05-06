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
    public partial class RentPage : Page
    {
        private readonly CarRentalContext _context;
        private Аренда? _currentRent;
        private ICollectionView? _rentView;

        public RentPage()
        {
            InitializeComponent();
            _context = new CarRentalContext();
            LoadData();
        }

        private void LoadData()
        {
            _context.Аренды.Include(a => a.Автомобиль).Include(a => a.Клиент).Load();
            _context.Автомобили.Load();
            _context.Клиенты.Load();

            CmbCar.ItemsSource = _context.Автомобили.Local.ToList();
            CmbClient.ItemsSource = _context.Клиенты.Local.ToList();
            CmbStatus.ItemsSource = new[] { "Активна", "Завершена", "Просрочена" };
            FilterStatusCombo.ItemsSource = new[] { "Все", "Активна", "Завершена", "Просрочена" };
            FilterStatusCombo.SelectedIndex = 0;

            RentGrid.ItemsSource = _context.Аренды.Local.ToObservableCollection();
            _rentView = CollectionViewSource.GetDefaultView(RentGrid.ItemsSource);
            _rentView.Filter = FilterRent;
            ClearForm();
        }

        private bool FilterRent(object obj)
        {
            if (obj is not Аренда item) return false;
            string search = SearchTextBox.Text?.ToLower() ?? string.Empty;
            string filterStatus = FilterStatusCombo.SelectedItem as string ?? "Все";

            bool matchesSearch = string.IsNullOrWhiteSpace(search) ||
                                 item.Автомобиль.Марка_Модель.ToLower().Contains(search) ||
                                 item.Клиент.ФИО.ToLower().Contains(search);
            bool matchesStatus = filterStatus == "Все" || item.Статус == filterStatus;

            return matchesSearch && matchesStatus;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!IsDataValid()) return;
            if (CmbCar.SelectedValue == null || CmbClient.SelectedValue == null) return;
            try
            {
                if (_currentRent == null)
                {
                    var newItem = new Аренда
                    {
                        ID_Авто = (int)CmbCar.SelectedValue,
                        ID_Клиента = (int)CmbClient.SelectedValue,
                        Дата_Выдачи = DpStart.SelectedDate ?? DateTime.Now,
                        Дата_Возврата_План = DpEndPlan.SelectedDate ?? DateTime.Now.AddDays(1),
                        Сумма_Итого = decimal.TryParse(TxtSum.Text, out var s) ? s : 0,
                        Залог = decimal.TryParse(TxtDeposit.Text, out var z) ? z : 0,
                        Статус = CmbStatus.Text
                    };
                    _context.Аренды.Add(newItem);
                }
                else
                {
                    _currentRent.ID_Авто = (int)CmbCar.SelectedValue;
                    _currentRent.ID_Клиента = (int)CmbClient.SelectedValue;
                    _currentRent.Дата_Выдачи = DpStart.SelectedDate ?? DateTime.Now;
                    _currentRent.Дата_Возврата_План = DpEndPlan.SelectedDate ?? DateTime.Now.AddDays(1);
                    _currentRent.Сумма_Итого = decimal.TryParse(TxtSum.Text, out var s) ? s : 0;
                    _currentRent.Залог = decimal.TryParse(TxtDeposit.Text, out var z) ? z : 0;
                    _currentRent.Статус = CmbStatus.Text;
                }
                _context.SaveChanges();
                RentGrid.Items.Refresh();
                ClearForm();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void RentGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RentGrid.SelectedItem is Аренда selected)
            {
                _currentRent = selected;
                CmbCar.SelectedValue = selected.ID_Авто;
                CmbClient.SelectedValue = selected.ID_Клиента;
                DpStart.SelectedDate = selected.Дата_Выдачи;
                DpEndPlan.SelectedDate = selected.Дата_Возврата_План;
                TxtSum.Text = selected.Сумма_Итого.ToString();
                TxtDeposit.Text = selected.Залог.ToString();
                CmbStatus.Text = selected.Статус;
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_currentRent != null && MessageBox.Show("Удалить запись?", "Удаление", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _context.Аренды.Remove(_currentRent);
                _context.SaveChanges();
                ClearForm();
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e) => ClearForm();
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => _rentView?.Refresh();
        private void FilterStatusCombo_SelectionChanged(object sender, SelectionChangedEventArgs e) => _rentView?.Refresh();

        private void ClearForm()
        {
            RentGrid.SelectedItem = null;
            _currentRent = null;
            CmbCar.SelectedIndex = CmbClient.SelectedIndex = -1;
            DpStart.SelectedDate = DpEndPlan.SelectedDate = null;
            TxtSum.Text = TxtDeposit.Text = string.Empty;
            CmbStatus.SelectedIndex = 0;
        }
        private bool IsDataValid()
        {
            if (CmbCar.SelectedValue == null || CmbClient.SelectedValue == null)
            {
                MessageBox.Show("Выбери машину и клиента!", "Внимание");
                return false;
            }
            var selectedClient = CmbClient.SelectedItem as Клиент;
            if (selectedClient != null && selectedClient.Черный_Список)
            {
                MessageBox.Show("Этот клиент в ЧЕРНОМ СПИСКЕ! Аренда запрещена.", "ОТКАЗ", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
            }

            if (DpStart.SelectedDate == null || DpEndPlan.SelectedDate == null)
            {
                MessageBox.Show("Укажиту все даты!", "Внимание");
                return false;
            }

            if (DpEndPlan.SelectedDate < DpStart.SelectedDate)
            {
                MessageBox.Show("Дата возврата не может быть раньше выдачи!", "Ошибка");
                return false;
            }

            if (!decimal.TryParse(TxtSum.Text, out decimal sum) || sum < 0)
            {
                MessageBox.Show("Сумма должна быть числом!", "Ошибка");
                return false;
            }

            return true;
        }
    }
}