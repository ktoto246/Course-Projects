using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Model;

namespace WpfApp1.Pages
{
    public partial class CategoriesPage : Page
    {
        private RetailDbContext _context;
        private int _selectedId = 0;

        public CategoriesPage()
        {
            InitializeComponent();
            _context = new RetailDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e) => LoadData();

        private void LoadData()
        {
            var query = _context.Categories.AsQueryable();
            string search = TxtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Name.ToLower().Contains(search));
            }
            GridCategories.ItemsSource = query.ToList();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("Имя категории не может быть пустым!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;
            _context.Categories.Add(new Category { Name = TxtName.Text.Trim() });
            _context.SaveChanges();
            LoadData();
            ClearForm();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0 || !ValidateForm()) return;
            var cat = _context.Categories.Find(_selectedId);
            if (cat != null)
            {
                cat.Name = TxtName.Text.Trim();
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedId == 0) return;
            if (MessageBox.Show("Удалить?", "Уверен?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var cat = _context.Categories.Find(_selectedId);
                if (cat != null)
                {
                    _context.Categories.Remove(cat);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
            }
        }

        private void GridCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridCategories.SelectedItem is Category cat)
            {
                _selectedId = cat.Id;
                TxtName.Text = cat.Name;
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

        private void ClearForm()
        {
            _selectedId = 0;
            TxtName.Clear();
            GridCategories.SelectedItem = null;
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => LoadData();
    }
}