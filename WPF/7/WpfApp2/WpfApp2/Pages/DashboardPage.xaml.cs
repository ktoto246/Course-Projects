using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.Data;
namespace WpfApp2.Pages
{
    public partial class DashboardPage : Page
    {
        private AssetDbContext _context;

        public ISeries[] PieSeries { get; set; }
        public ISeries[] BarSeries { get; set; }
        public Axis[] XAxes { get; set; }

        public DashboardPage()
        {
            InitializeComponent();
            _context = new AssetDbContext();
            LoadData();

            DataContext = this;
        }

        private void LoadData()
        {
            var softwareStats = _context.УстановкиПО
                .Include(u => u.ПрограммноеОбеспечение)
                .GroupBy(u => u.ПрограммноеОбеспечение.Название)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToList();

            var pieSeriesList = new List<ISeries>();
            foreach (var stat in softwareStats)
            {
                pieSeriesList.Add(new PieSeries<int>
                {
                    Values = new[] { stat.Count },
                    Name = stat.Name,
                    DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue} шт."
                });
            }
            PieSeries = pieSeriesList.ToArray();


            var deptStats = _context.РабочиеМеста
                .Include(r => r.Отдел)
                .GroupBy(r => r.Отдел.Название)
                .Select(g => new { Name = g.Key, Count = g.Count() })
                .ToList();

            BarSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = deptStats.Select(s => s.Count).ToArray(),
                    Name = "Рабочих мест",
                    Fill = new SolidColorPaint(SKColors.CornflowerBlue),
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsFormatter = point => point.Coordinate.PrimaryValue.ToString()
                }
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = deptStats.Select(s => s.Name).ToList(),
                    LabelsRotation = 15
                }
            };
        }
    }
}