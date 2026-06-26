using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class AnalysisPage : Page
    {
        private BalashovDbContext _context;

        public AnalysisPage()
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
            _context.AnalysisLogs
                .Include(a => a.GrainBatch)
                .Include(a => a.LaboratoryStaff)
                .Include(a => a.QualityIndicator)
                .Load();

            AnalysisGrid.ItemsSource = _context.AnalysisLogs.Local.ToObservableCollection();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            var filtered = _context.AnalysisLogs.Local.Where(a =>
                a.GrainBatch.VehicleNumber.ToLower().Contains(searchText) ||
                a.QualityIndicator.Name.ToLower().Contains(searchText)).ToList();

            AnalysisGrid.ItemsSource = filtered;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new AnalysisEditWindow(new AnalysisLog { AnalysisDate = System.DateTime.Now }, _context);
            if (window.ShowDialog() == true)
            {
                _context.AnalysisLogs.Add(window.CurrentAnalysis);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AnalysisGrid.SelectedItem is AnalysisLog selected)
            {
                var window = new AnalysisEditWindow(selected, _context);
                if (window.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    AnalysisGrid.Items.Refresh();
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (AnalysisGrid.SelectedItem is AnalysisLog selected)
            {
                _context.AnalysisLogs.Remove(selected);
                _context.SaveChanges();
                LoadData();
            }
        }
    }
}