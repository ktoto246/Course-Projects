using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class ResidentsPage : Page
    {
        private BalashovDbContext _context;

        public ResidentsPage()
        {
            InitializeComponent();
            _context = new BalashovDbContext();
            LoadData();
        }

        private void LoadData()
        {
            if (_context == null) return;

            var query = _context.Residents.AsQueryable();

            string searchText = TxtSearch.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(r => r.FullName.ToLower().Contains(searchText) || r.RoomNumber.ToLower().Contains(searchText));
            }

            if (CmbSort.SelectedItem is ComboBoxItem selectedSort)
            {
                switch (selectedSort.Content.ToString())
                {
                    case "ФИО (А-Я)":
                        query = query.OrderBy(r => r.FullName);
                        break;
                    case "ФИО (Я-А)":
                        query = query.OrderByDescending(r => r.FullName);
                        break;
                    case "Комната":
                        query = query.OrderBy(r => r.RoomNumber);
                        break;
                    default:
                        query = query.OrderBy(r => r.ID);
                        break;
                }
            }

            GridResidents.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void GridResidents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridResidents.SelectedItem is Resident selected)
            {
                TxtFullName.Text = selected.FullName;
                DpBirthDate.SelectedDate = selected.BirthDate;
                TxtRoom.Text = selected.RoomNumber;

                if (selected.DisabilityGroup == null)
                    CmbDisability.SelectedIndex = 0;
                else
                    CmbDisability.SelectedIndex = selected.DisabilityGroup.Value;
            }
        }

        private int? GetSelectedDisabilityGroup()
        {
            if (CmbDisability.SelectedIndex == 0)
                return null;
            return CmbDisability.SelectedIndex;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string fullName = TxtFullName.Text?.Trim() ?? "";
            string room = TxtRoom.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(room) || DpBirthDate.SelectedDate == null)
            {
                MessageBox.Show("Заполните все поля (ФИО, Дата рождения, Комната).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newResident = new Resident
            {
                FullName = fullName,
                BirthDate = DpBirthDate.SelectedDate.Value,
                RoomNumber = room,
                DisabilityGroup = GetSelectedDisabilityGroup()
            };

            _context.Residents.Add(newResident);
            _context.SaveChanges();
            LoadData();
            ClearFields();

            MessageBox.Show($"Добавлен проживающий: {fullName}, Комната: {room}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (GridResidents.SelectedItem is Resident selected)
            {
                string fullName = TxtFullName.Text?.Trim() ?? "";
                string room = TxtRoom.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(room) || DpBirthDate.SelectedDate == null)
                {
                    MessageBox.Show("Поля не могут быть пустыми.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selected.FullName = fullName;
                selected.BirthDate = DpBirthDate.SelectedDate.Value;
                selected.RoomNumber = room;
                selected.DisabilityGroup = GetSelectedDisabilityGroup();

                _context.SaveChanges();
                LoadData();
                ClearFields();

                MessageBox.Show("Изменения сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridResidents.SelectedItem is Resident selected)
            {
                if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.Residents.Remove(selected);
                    _context.SaveChanges();
                    LoadData();
                    ClearFields();
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            TxtFullName.Text = string.Empty;
            DpBirthDate.SelectedDate = null;
            TxtRoom.Text = string.Empty;
            CmbDisability.SelectedIndex = 0;
            GridResidents.SelectedItem = null;
        }
    }
}