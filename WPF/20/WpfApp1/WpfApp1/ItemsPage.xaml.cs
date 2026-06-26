using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class ItemsPage : Page
    {
        private BalashovDbContext _context;

        public ItemsPage()
        {
            InitializeComponent();
            _context = new BalashovDbContext();
            LoadCategories();
            LoadData();
        }

        private void LoadCategories()
        {
            if (_context == null) return;
            CmbCategory.ItemsSource = _context.Categories.ToList();
        }

        private void LoadData()
        {
            if (_context == null) return;

            var query = _context.Items.Include(i => i.Category).AsQueryable();

            string searchText = TxtSearch.Text?.Trim().ToLower() ?? "";
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(i => i.ItemName.ToLower().Contains(searchText) || i.Category.CategoryName.ToLower().Contains(searchText));
            }

            if (CmbSort.SelectedItem is ComboBoxItem selectedSort)
            {
                switch (selectedSort.Content.ToString())
                {
                    case "Название (А-Я)":
                        query = query.OrderBy(i => i.ItemName);
                        break;
                    case "Название (Я-А)":
                        query = query.OrderByDescending(i => i.ItemName);
                        break;
                    case "По категории":
                        query = query.OrderBy(i => i.Category.CategoryName);
                        break;
                    case "Сначала меньше остаток":
                        query = query.OrderBy(i => i.AvailableQuantity);
                        break;
                    default:
                        query = query.OrderBy(i => i.ID);
                        break;
                }
            }

            GridItems.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void GridItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridItems.SelectedItem is Item selected)
            {
                TxtItemName.Text = selected.ItemName;
                CmbCategory.SelectedValue = selected.CategoryID;
                TxtTotalQty.Text = selected.TotalQuantity.ToString();
                TxtAvailableQty.Text = selected.AvailableQuantity.ToString();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string itemName = TxtItemName.Text?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(itemName) || CmbCategory.SelectedValue == null)
            {
                MessageBox.Show("Заполните наименование и выберите категорию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtTotalQty.Text, out int totalQty) || !int.TryParse(TxtAvailableQty.Text, out int availableQty))
            {
                MessageBox.Show("Количество должно быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newItem = new Item
            {
                ItemName = itemName,
                CategoryID = (int)CmbCategory.SelectedValue,
                TotalQuantity = totalQty,
                AvailableQuantity = availableQty
            };

            _context.Items.Add(newItem);
            _context.SaveChanges();
            LoadData();
            ClearFields();

            MessageBox.Show($"Добавлен инвентарь: {itemName} (Всего: {totalQty}, Доступно: {availableQty})", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (GridItems.SelectedItem is Item selected)
            {
                string itemName = TxtItemName.Text?.Trim() ?? "";

                if (string.IsNullOrWhiteSpace(itemName) || CmbCategory.SelectedValue == null)
                {
                    MessageBox.Show("Наименование и категория не могут быть пустыми.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(TxtTotalQty.Text, out int totalQty) || !int.TryParse(TxtAvailableQty.Text, out int availableQty))
                {
                    MessageBox.Show("Количество должно быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                selected.ItemName = itemName;
                selected.CategoryID = (int)CmbCategory.SelectedValue;
                selected.TotalQuantity = totalQty;
                selected.AvailableQuantity = availableQty;

                _context.SaveChanges();
                LoadData();
                ClearFields();

                MessageBox.Show("Изменения сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (GridItems.SelectedItem is Item selected)
            {
                if (MessageBox.Show("Удалить выбранную запись?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.Items.Remove(selected);
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
            TxtItemName.Text = string.Empty;
            CmbCategory.SelectedIndex = -1;
            TxtTotalQty.Text = string.Empty;
            TxtAvailableQty.Text = string.Empty;
            GridItems.SelectedItem = null;
        }
    }
}