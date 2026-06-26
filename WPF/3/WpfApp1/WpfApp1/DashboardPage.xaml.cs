using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Model;

namespace WpfApp1
{
    public partial class DashboardPage : Page
    {
        public SeriesCollection TechCountSeries { get; set; }
        public SeriesCollection StatusSeries { get; set; }
        public string[] TypeLabels { get; set; }

        public DashboardPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadCharts();
        }

        private void LoadCharts()
        {
            using (var db = new AppDbContext())
            {
                var techByType = db.Техника
                    .Include(t => t.ТипСвт)
                    .Where(t => t.ТипСвт != null && t.ТипСвт.Название != null)
                    .GroupBy(t => t.ТипСвт.Название)
                    .Select(g => new { Name = g.Key, Count = g.Count() }).ToList();

                TechCountSeries = new SeriesCollection {
                    new ColumnSeries { Title = "Единиц", Values = new ChartValues<int>(techByType.Select(x => x.Count)) }
                };
                TypeLabels = techByType.Select(x => x.Name).ToArray();

                StatusSeries = new SeriesCollection();
                var stats = db.Техника
                    .Where(t => t.Статус != null && t.Статус != "")
                    .GroupBy(t => t.Статус)
                    .Select(g => new { Status = g.Key, Count = g.Count() }).ToList();

                foreach (var s in stats)
                {
                    StatusSeries.Add(new PieSeries { Title = s.Status, Values = new ChartValues<int> { s.Count }, DataLabels = true });
                }
            }
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadCharts();
            DataContext = null;
            DataContext = this;
        }
    }
}