using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class ProductsPage : Page
    {
        private Product _editingProduct;

        public ProductsPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData(string searchText = "")
        {
            using (var context = new ElkiTorgContext())
            {
                var query = context.Products.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    query = query.Where(p => p.ModelName.Contains(searchText));
                }

                ProductsGrid.ItemsSource = query.ToList();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            _editingProduct = null;
            PanelTitle.Text = "Новый товар";
            TxtModelName.Text = "";
            TxtHeight.Text = "";
            TxtPrice.Text = "";
            TxtStock.Text = "0";

            EditPanel.Visibility = Visibility.Visible;
            ProductsGrid.IsEnabled = false;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selectedProduct)
            {
                _editingProduct = selectedProduct;
                PanelTitle.Text = "Редактирование";
                TxtModelName.Text = selectedProduct.ModelName;
                TxtHeight.Text = selectedProduct.Height.ToString(CultureInfo.CurrentCulture);
                TxtPrice.Text = selectedProduct.Price.ToString(CultureInfo.CurrentCulture);
                TxtStock.Text = selectedProduct.StockQuantity.ToString();

                EditPanel.Visibility = Visibility.Visible;
                ProductsGrid.IsEnabled = false;
            }
            else
            {
                MessageBox.Show("Сначала выберите товар в таблице.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem is Product selectedProduct)
            {
                if (MessageBox.Show("Удалить выбранный товар?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    using (var context = new ElkiTorgContext())
                    {
                        var productToDelete = context.Products.Find(selectedProduct.ProductID);
                        if (productToDelete != null)
                        {
                            try
                            {
                                context.Products.Remove(productToDelete);
                                context.SaveChanges();
                                LoadData(TxtSearch.Text);
                            }
                            catch
                            {
                                MessageBox.Show("Невозможно удалить товар, так как он используется в заказах.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Сначала выберите товар.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtModelName.Text))
            {
                MessageBox.Show("Наименование товара обязательно для заполнения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(TxtHeight.Text, out decimal height) || !decimal.TryParse(TxtPrice.Text, out decimal price) || !int.TryParse(TxtStock.Text, out int stock))
            {
                MessageBox.Show("Проверьте корректность ввода числовых данных.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var context = new ElkiTorgContext())
            {
                if (_editingProduct == null)
                {
                    var newProduct = new Product
                    {
                        ModelName = TxtModelName.Text,
                        Height = height,
                        Price = price,
                        StockQuantity = stock
                    };
                    context.Products.Add(newProduct);
                }
                else
                {
                    var productToUpdate = context.Products.Find(_editingProduct.ProductID);
                    if (productToUpdate != null)
                    {
                        productToUpdate.ModelName = TxtModelName.Text;
                        productToUpdate.Height = height;
                        productToUpdate.Price = price;
                        productToUpdate.StockQuantity = stock;
                    }
                }
                context.SaveChanges();
            }

            ClosePanel();
            LoadData(TxtSearch.Text);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel();
        }

        private void ClosePanel()
        {
            EditPanel.Visibility = Visibility.Collapsed;
            ProductsGrid.IsEnabled = true;
        }
    }
}