using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class ProductsPage : Page
    {
        private MagnitDbContext _db = new MagnitDbContext();

        public ProductsPage()
        {
            InitializeComponent();
            RefreshData();
        }

        private void RefreshData()
        {
            CmbCategories.ItemsSource = _db.Categories.ToList();

            DgProducts.ItemsSource = _db.Products.Include(p => p.Category).ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string name = TxtProductName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || CmbCategories.SelectedValue == null)
            {
                MessageBox.Show("Ошибка: введите название товара и выберите категорию!");
                return;
            }

            int categoryId = (int)CmbCategories.SelectedValue;

            Product product = new Product
            {
                Name = name,
                CategoryId = categoryId
            };

            _db.Products.Add(product);
            _db.SaveChanges();

            ClearInputs();
            RefreshData();
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (DgProducts.SelectedItem is not Product selectedProduct)
            {
                MessageBox.Show("Ошибка: выберите товар из списка!");
                return;
            }

            string name = TxtProductName.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) || CmbCategories.SelectedValue == null)
            {
                MessageBox.Show("Ошибка: введите название товара и выберите категорию!");
                return;
            }

            int categoryId = (int)CmbCategories.SelectedValue;

            var product = _db.Products.Find(selectedProduct.Id);
            if (product != null)
            {
                product.Name = name;
                product.CategoryId = categoryId;
                _db.SaveChanges();
            }

            ClearInputs();
            RefreshData();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DgProducts.SelectedItem is not Product selectedProduct)
            {
                MessageBox.Show("Ошибка: выберите товар из списка!");
                return;
            }

            var result = MessageBox.Show($"Удалить товар {selectedProduct.Name}?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var product = _db.Products.Find(selectedProduct.Id);
                    if (product != null)
                    {
                        _db.Products.Remove(product);
                        _db.SaveChanges();
                    }

                    ClearInputs();
                    RefreshData();
                }
                catch (Exception)
                {
                    MessageBox.Show("Ошибка: невозможно удалить товар, так как он числится в журнале приемки.");
                }
            }
        }

        private void DgProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgProducts.SelectedItem is Product selectedProduct)
            {
                TxtProductName.Text = selectedProduct.Name;
                CmbCategories.SelectedValue = selectedProduct.CategoryId;
            }
        }

        private void ClearInputs()
        {
            TxtProductName.Clear();
            CmbCategories.SelectedItem = null;
        }
    }
}