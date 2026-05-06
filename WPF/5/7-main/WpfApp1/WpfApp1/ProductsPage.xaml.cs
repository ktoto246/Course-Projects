using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class ProductsPage : Page
    {
        private AppDbContext _context;

        public ProductsPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => p.ProductName.Contains(searchText) || p.Category.Contains(searchText));
            }

            DataGridProducts.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text.Trim());
        }

        private void DataGridProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridProducts.SelectedItem is Product selectedProduct)
            {
                TxtProductName.Text = selectedProduct.ProductName;
                TxtCategory.Text = selectedProduct.Category;
                TxtBasePrice.Text = selectedProduct.BasePrice.ToString();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(TxtProductName.Text) || string.IsNullOrWhiteSpace(TxtCategory.Text))
            {
                MessageBox.Show("Заполните название и категорию.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!decimal.TryParse(TxtBasePrice.Text, out _))
            {
                MessageBox.Show("Цена должна быть числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            var newProduct = new Product
            {
                ProductName = TxtProductName.Text.Trim(),
                Category = TxtCategory.Text.Trim(),
                BasePrice = decimal.Parse(TxtBasePrice.Text.Trim())
            };

            _context.Products.Add(newProduct);
            _context.SaveChanges();

            ClearFields();
            LoadData(TxtSearch.Text.Trim());
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridProducts.SelectedItem is Product selectedProduct)
            {
                if (!ValidateInputs()) return;

                selectedProduct.ProductName = TxtProductName.Text.Trim();
                selectedProduct.Category = TxtCategory.Text.Trim();
                selectedProduct.BasePrice = decimal.Parse(TxtBasePrice.Text.Trim());

                _context.SaveChanges();

                ClearFields();
                LoadData(TxtSearch.Text.Trim());
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridProducts.SelectedItem is Product selectedProduct)
            {
                if (MessageBox.Show($"Удалить продукт '{selectedProduct.ProductName}'?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Products.Remove(selectedProduct);
                        _context.SaveChanges();
                        ClearFields();
                        LoadData(TxtSearch.Text.Trim());
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                    {
                        MessageBox.Show("Невозможно удалить продукт, так как по нему существуют оформленные сделки. Сначала удалите связанные сделки.",
                                        "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);

                        _context.Entry(selectedProduct).State = Microsoft.EntityFrameworkCore.EntityState.Unchanged;
                    }
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            TxtProductName.Clear();
            TxtCategory.Clear();
            TxtBasePrice.Clear();
            DataGridProducts.SelectedItem = null;
        }
    }
}