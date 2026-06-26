using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class SuppliersPage : Page
    {
        private BalashovDbContext _context;

        public SuppliersPage()
        {
            InitializeComponent();
            _context = new BalashovDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            _context.Suppliers.Load();
            SuppliersGrid.ItemsSource = _context.Suppliers.Local.ToObservableCollection();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            var filtered = _context.Suppliers.Local.Where(s =>
                s.Name.ToLower().Contains(searchText) ||
                (s.Phone != null && s.Phone.ToLower().Contains(searchText)) ||
                (s.Address != null && s.Address.ToLower().Contains(searchText))).ToList();

            SuppliersGrid.ItemsSource = filtered;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new SupplierEditWindow(new Supplier());
            if (window.ShowDialog() == true)
            {
                _context.Suppliers.Add(window.CurrentSupplier);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SuppliersGrid.SelectedItem is Supplier selected)
            {
                var window = new SupplierEditWindow(selected);
                if (window.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    SuppliersGrid.Items.Refresh();
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (SuppliersGrid.SelectedItem is Supplier selected)
            {
                _context.Suppliers.Remove(selected);
                _context.SaveChanges();
                LoadData();
            }
        }
    }
}