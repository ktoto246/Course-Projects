using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class IndicatorsPage : Page
    {
        private BalashovDbContext _context;

        public IndicatorsPage()
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
            _context.QualityIndicators.Load();
            IndicatorsGrid.ItemsSource = _context.QualityIndicators.Local.ToObservableCollection();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            var filtered = _context.QualityIndicators.Local.Where(i =>
                i.Name.ToLower().Contains(searchText)).ToList();

            IndicatorsGrid.ItemsSource = filtered;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new IndicatorEditWindow(new QualityIndicator());
            if (window.ShowDialog() == true)
            {
                _context.QualityIndicators.Add(window.CurrentIndicator);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IndicatorsGrid.SelectedItem is QualityIndicator selected)
            {
                var window = new IndicatorEditWindow(selected);
                if (window.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    IndicatorsGrid.Items.Refresh();
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (IndicatorsGrid.SelectedItem is QualityIndicator selected)
            {
                _context.QualityIndicators.Remove(selected);
                _context.SaveChanges();
                LoadData();
            }
        }
    }
}