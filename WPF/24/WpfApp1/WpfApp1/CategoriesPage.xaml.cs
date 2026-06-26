using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class CategoriesPage : Page
    {
        private MagnitDbContext _db = new MagnitDbContext();

        public CategoriesPage()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            DgCategories.ItemsSource = _db.Categories.ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtCategoryName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Ошибка: название категории не может быть пустым!");
                return;
            }

            if (_db.Categories.Any(c => c.Name == name))
            {
                MessageBox.Show("Ошибка: такая категория уже существует!");
                return;
            }

            Category category = new Category { Name = name };
            _db.Categories.Add(category);
            _db.SaveChanges();

            TxtCategoryName.Clear();
            RefreshData();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DgCategories.SelectedItem is not Category selectedCategory)
            {
                MessageBox.Show("Ошибка: выберите категорию из списка!");
                return;
            }

            string name = TxtCategoryName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Ошибка: название категории не может быть пустым!");
                return;
            }

            if (_db.Categories.Any(c => c.Name == name && c.Id != selectedCategory.Id))
            {
                MessageBox.Show("Ошибка: категория с таким названием уже есть!");
                return;
            }

            var category = _db.Categories.Find(selectedCategory.Id);
            if (category != null)
            {
                category.Name = name;
                _db.SaveChanges();
            }

            TxtCategoryName.Clear();
            RefreshData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgCategories.SelectedItem is not Category selectedCategory)
            {
                MessageBox.Show("Ошибка: выберите категорию из списка!");
                return;
            }

            var result = MessageBox.Show("Внимание! При удалении категории удалятся и все связанные товары. Продолжить?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                var category = _db.Categories.Find(selectedCategory.Id);
                if (category != null)
                {
                    _db.Categories.Remove(category);
                    _db.SaveChanges();
                }

                TxtCategoryName.Clear();
                RefreshData();
            }
        }

        private void DgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgCategories.SelectedItem is Category selectedCategory)
            {
                TxtCategoryName.Text = selectedCategory.Name;
            }
        }
    }
}