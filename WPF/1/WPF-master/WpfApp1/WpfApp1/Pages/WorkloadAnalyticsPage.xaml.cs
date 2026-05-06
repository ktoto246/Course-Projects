using System.Linq;
using System.Windows.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class WorkloadAnalyticsPage : Page
    {
        public ISeries[] WorkloadSeries { get; set; }
        public Axis[] XAxes { get; set; }

        public WorkloadAnalyticsPage()
        {
            InitializeComponent();
            this.Loaded += (s, e) => LoadData();
        }

        private void LoadData()
        {
            using var db = new AppDbContext();
            var stats = db.Equipments.Where(e => e.EmployeeId != null && e.Status.Name != SystemStatuses.Scrapped)
                           .GroupBy(e => e.Employee.FullName)
                           .Select(g => new { Name = g.Key, Count = g.Count() })
                           .OrderByDescending(x => x.Count).Take(10).ToList();

            WorkloadSeries = new ISeries[] { new RowSeries<int> { Values = stats.Select(s => s.Count).ToArray(), Name = "Техника" } };
            XAxes = new Axis[] { new Axis { Labels = stats.Select(s => s.Name).ToArray() } };
            DataContext = null; DataContext = this;
        }
    }
}