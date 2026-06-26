using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class ProductPage : Page
    {
        private AppDbContext _context;
        private List<Product> _products = new List<Product>();
        private int currentId = 0;

        public ProductPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _products = _context.Products.ToList();
            dataGrid.ItemsSource = _products;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox searchBox = sender as TextBox;
            if (searchBox == null) return;

            string filter = searchBox.Text.ToLower();
            var filtered = _products.Where(p =>
                p.ProductName.ToLower().Contains(filter) ||
                p.PricePerTon.ToString().Contains(filter)).ToList();

            dataGrid.ItemsSource = filtered;
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is Product selectedProduct)
            {
                txtName.Text = selectedProduct.ProductName;
                txtWeight.Text = selectedProduct.PackagingWeightKg.ToString("0.##");
                txtPrice.Text = selectedProduct.PricePerTon.ToString("0.##");
                currentId = selectedProduct.ProductId;
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название продукта!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string weightText = txtWeight.Text.Replace('.', ',');
            string priceText = txtPrice.Text.Replace('.', ',');

            if (!decimal.TryParse(weightText, out decimal weight) || !decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Введите корректные числовые значения для веса и цены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var product = new Product
            {
                ProductName = txtName.Text,
                PackagingWeightKg = weight,
                PricePerTon = price
            };

            _context.Products.Add(product);
            _context.SaveChanges();
            LoadData();
            ClearFields();
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (currentId > 0)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Введите название продукта!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string weightText = txtWeight.Text.Replace('.', ',');
                string priceText = txtPrice.Text.Replace('.', ',');

                if (!decimal.TryParse(weightText, out decimal weight) || !decimal.TryParse(priceText, out decimal price))
                {
                    MessageBox.Show("Введите корректные числовые значения для веса и цены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var product = _context.Products.Find(currentId);
                if (product != null)
                {
                    product.ProductName = txtName.Text;
                    product.PackagingWeightKg = weight;
                    product.PricePerTon = price;

                    _context.SaveChanges();
                    LoadData();
                    ClearFields();
                }
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (currentId > 0)
            {
                var result = MessageBox.Show("Точно удалить продукт? Это затронет историю отгрузок!", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var product = _context.Products.Find(currentId);
                    if (product != null)
                    {
                        _context.Products.Remove(product);
                        _context.SaveChanges();
                        LoadData();
                        ClearFields();
                    }
                }
            }
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtWeight.Text = "";
            txtPrice.Text = "";
            currentId = 0;
            dataGrid.SelectedItem = null;
        }
    }
}