using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class CategoriesPage : Page
    {
        private BalashovDbContext _context;

        public CategoriesPage()
        {
            InitializeComponent();
            _context = new BalashovDbContext();
            LoadData();
        }

        private void LoadData()
        {
            if (_context == null) return;

            var query = _context.Categories.AsQueryable();

            string searchText = TxtSearch.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(c => c.CategoryName.ToLower().Contains(searchText));
            }

            if (CmbSort.SelectedItem is ComboBoxItem selectedSort)
            {
                switch (selectedSort.Content.ToString())
                {
                    case "Название (А-Я)":
                        query = query.OrderBy(c => c.CategoryName);
                        break;
                    case "Название (Я-А)":
                        query = query.OrderByDescending(c => c.CategoryName);
                        break;
                    default:
                        query = query.OrderBy(c => c.ID);
                        break;
                }
            }

            GridCategories.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void GridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridCategories.SelectedItem is Category selected)
            {
                TxtCategoryName.Text = selected.CategoryName;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string categoryName = TxtCategoryName.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                MessageBox.Show("Заполните поле с названием категории.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newCat = new Category
            {
                CategoryName = categoryName
            };

            _context.Categories.Add(newCat);
            _context.SaveChanges();
            LoadData();
            ClearFields();

            MessageBox.Show($"Добавлена категория: {categoryName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (GridCategories.SelectedItem is Category selected)
            {
                string categoryName = TxtCategoryName.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(categoryName))
                {
                    MessageBox.Show("Поле не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selected.CategoryName = categoryName;

                _context.SaveChanges();
                LoadData();
                ClearFields();

                MessageBox.Show("Изменения сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridCategories.SelectedItem is Category selected)
            {
                if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.Categories.Remove(selected);
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
            TxtCategoryName.Text = string.Empty;
            GridCategories.SelectedItem = null;
        }
    }
}