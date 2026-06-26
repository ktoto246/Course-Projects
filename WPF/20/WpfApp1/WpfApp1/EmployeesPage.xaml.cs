using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class EmployeesPage : Page
    {
        private BalashovDbContext _context;

        public EmployeesPage()
        {
            InitializeComponent();
            _context = new BalashovDbContext();
            LoadData();
        }

        private void LoadData()
        {
            if (_context == null) return;

            var query = _context.Employees.AsQueryable();

            string searchText = TxtSearch.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(e => e.FullName.ToLower().Contains(searchText) || e.Position.ToLower().Contains(searchText));
            }

            if (CmbSort.SelectedItem is ComboBoxItem selectedSort)
            {
                switch (selectedSort.Content.ToString())
                {
                    case "ФИО (А-Я)":
                        query = query.OrderBy(e => e.FullName);
                        break;
                    case "ФИО (Я-А)":
                        query = query.OrderByDescending(e => e.FullName);
                        break;
                    case "По должности":
                        query = query.OrderBy(e => e.Position);
                        break;
                    default:
                        query = query.OrderBy(e => e.ID);
                        break;
                }
            }

            GridEmployees.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void GridEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridEmployees.SelectedItem is Employee selected)
            {
                TxtFullName.Text = selected.FullName;
                TxtPosition.Text = selected.Position;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string fullName = TxtFullName.Text?.Trim() ?? "";
            string position = TxtPosition.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(position))
            {
                MessageBox.Show("Заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newEmp = new Employee
            {
                FullName = fullName,
                Position = position
            };

            _context.Employees.Add(newEmp);
            _context.SaveChanges();
            LoadData();
            ClearFields();

            MessageBox.Show($"Добавлена запись: {fullName} - {position}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (GridEmployees.SelectedItem is Employee selected)
            {
                string fullName = TxtFullName.Text?.Trim() ?? "";
                string position = TxtPosition.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(position))
                {
                    MessageBox.Show("Поля не могут быть пустыми.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selected.FullName = fullName;
                selected.Position = position;

                _context.SaveChanges();
                LoadData();
                ClearFields();

                MessageBox.Show("Изменения сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridEmployees.SelectedItem is Employee selected)
            {
                if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.Employees.Remove(selected);
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
            TxtPosition.Text = string.Empty;
            GridEmployees.SelectedItem = null;
        }
    }
}