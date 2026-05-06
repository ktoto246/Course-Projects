using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Model;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private RetailDbContext _context;
        private int _selectedProductId = 0;

        public ProductsPage()
        {
            InitializeComponent();
            _context = new RetailDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
            LoadData();
        }
        private void LoadCategories()
        {
            var categories = _context.Categories.ToList();

            CmbFormCategory.ItemsSource = categories;

            CmbFilterCategory.ItemsSource = categories;
        }

        private void LoadData()
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (CmbFilterCategory.SelectedValue != null)
            {
                int categoryId = (int)CmbFilterCategory.SelectedValue;
                query = query.Where(p => p.CategoryId == categoryId);
            }

            string searchText = TxtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(p => p.Name.ToLower().Contains(searchText) || p.SKU.ToLower().Contains(searchText));
            }

            GridProducts.ItemsSource = query.ToList();
        }

        private bool ValidateForm()
        {
            if (CmbFormCategory.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TxtSKU.Text) || string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Артикул и название не могут быть пустыми!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!decimal.TryParse(TxtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Некорректная цена!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(TxtStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Некорректный остаток!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var newProduct = new Product
            {
                CategoryId = (int)CmbFormCategory.SelectedValue,
                SKU = TxtSKU.Text.Trim(),
                Name = TxtName.Text.Trim(),
                Price = decimal.Parse(TxtPrice.Text),
                StockQuantity = int.Parse(TxtStock.Text)
            };

            _context.Products.Add(newProduct);

            try
            {
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProductId == 0)
            {
                MessageBox.Show("Выберите товар из таблицы для изменения!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ValidateForm()) return;

            var product = _context.Products.Find(_selectedProductId);
            if (product != null)
            {
                product.CategoryId = (int)CmbFormCategory.SelectedValue;
                product.SKU = TxtSKU.Text.Trim();
                product.Name = TxtName.Text.Trim();
                product.Price = decimal.Parse(TxtPrice.Text);
                product.StockQuantity = int.Parse(TxtStock.Text);

                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProductId == 0) return;

            if (MessageBox.Show("Точно удалить?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var product = _context.Products.Find(_selectedProductId);
                if (product != null)
                {
                    _context.Products.Remove(product);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
            }
        }

        private void GridProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridProducts.SelectedItem is Product selectedProduct)
            {
                _selectedProductId = selectedProduct.Id;

                CmbFormCategory.SelectedValue = selectedProduct.CategoryId;
                TxtSKU.Text = selectedProduct.SKU;
                TxtName.Text = selectedProduct.Name;
                TxtPrice.Text = selectedProduct.Price.ToString("F2");
                TxtStock.Text = selectedProduct.StockQuantity.ToString();
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedProductId = 0;
            CmbFormCategory.SelectedItem = null;
            TxtSKU.Clear();
            TxtName.Clear();
            TxtPrice.Clear();
            TxtStock.Clear();
            GridProducts.SelectedItem = null;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void CmbFilterCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            CmbFilterCategory.SelectedItem = null;
            TxtSearch.Clear();
            LoadData();
        }
    }
}