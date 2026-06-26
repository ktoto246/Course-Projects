using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1
{
    public partial class StaffPage : Page
    {
        private BalashovDbContext _context;

        public StaffPage()
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
            _context.LaboratoryStaff.Load();
            StaffGrid.ItemsSource = _context.LaboratoryStaff.Local.ToObservableCollection();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            var filtered = _context.LaboratoryStaff.Local.Where(s =>
                s.FullName.ToLower().Contains(searchText) ||
                s.Position.ToLower().Contains(searchText)).ToList();

            StaffGrid.ItemsSource = filtered;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var window = new StaffEditWindow(new LaboratoryStaff());
            if (window.ShowDialog() == true)
            {
                _context.LaboratoryStaff.Add(window.CurrentStaff);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StaffGrid.SelectedItem is LaboratoryStaff selected)
            {
                var window = new StaffEditWindow(selected);
                if (window.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    StaffGrid.Items.Refresh();
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (StaffGrid.SelectedItem is LaboratoryStaff selected)
            {
                _context.LaboratoryStaff.Remove(selected);
                _context.SaveChanges();
                LoadData();
            }
        }
    }
}