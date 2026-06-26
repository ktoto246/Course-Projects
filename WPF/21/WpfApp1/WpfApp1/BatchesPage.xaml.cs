using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class BatchesPage : Page
    {
        private BalashovDbContext _context;

        public BatchesPage()
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
            _context.GrainBatches.Include(g => g.Supplier).Load();
            BatchesGrid.ItemsSource = _context.GrainBatches.Local.ToObservableCollection();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            var filtered = _context.GrainBatches.Local.Where(g =>
                g.VehicleNumber.ToLower().Contains(searchText) ||
                (g.Supplier != null && g.Supplier.Name.ToLower().Contains(searchText))).ToList();

            BatchesGrid.ItemsSource = filtered;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new BatchEditWindow(new GrainBatch { DeliveryDate = System.DateTime.Now }, _context);
            if (window.ShowDialog() == true)
            {
                _context.GrainBatches.Add(window.CurrentBatch);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BatchesGrid.SelectedItem is GrainBatch selected)
            {
                var window = new BatchEditWindow(selected, _context);
                if (window.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    BatchesGrid.Items.Refresh();
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (BatchesGrid.SelectedItem is GrainBatch selected)
            {
                _context.GrainBatches.Remove(selected);
                _context.SaveChanges();
                LoadData();
            }
        }
    }
}