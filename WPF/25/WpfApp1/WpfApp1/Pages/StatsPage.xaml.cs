using System.Linq;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;

namespace WpfApp1.Pages
{
    public partial class StatsPage : Page
    {
        public SeriesCollection PieSeries { get; set; }
        public SeriesCollection BarSeries { get; set; }
        public string[] BarLabels { get; set; }

        public StatsPage()
        {
            InitializeComponent();
            LoadStatistics();
            DataContext = this;
        }

        private void LoadStatistics()
        {
            PieSeries = new SeriesCollection();
            BarSeries = new SeriesCollection();
            BarLabels = new string[0];

            using (var db = new BmtcContext())
            {
                var inspections = db.Inspections.Include(i => i.Service).ToList();

                if (!inspections.Any())
                {
                    TxtTotalInspections.Text = "Всего осмотров: 0";
                    TxtTotalRevenue.Text = "Общая выручка: 0.00 ₽";
                    return;
                }

                int totalCount = inspections.Count;
                decimal totalMoney = inspections.Sum(i => i.Service.Price);

                TxtTotalInspections.Text = $"Всего осмотров: {totalCount}";
                TxtTotalRevenue.Text = $"Общая выручка: {totalMoney:N2} ₽";

                int passedCount = inspections.Count(i => i.IsPassed);
                int failedCount = totalCount - passedCount;

                PieSeries.Add(new PieSeries
                {
                    Title = "Пройдено",
                    Values = new ChartValues<int> { passedCount },
                    DataLabels = true,
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(46, 204, 113))
                });

                PieSeries.Add(new PieSeries
                {
                    Title = "Не пройдено",
                    Values = new ChartValues<int> { failedCount },
                    DataLabels = true,
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(231, 76, 60))
                });

                var revenueByService = inspections
                    .GroupBy(i => i.Service.Name)
                    .Select(g => new
                    {
                        ServiceName = g.Key,
                        TotalRevenue = g.Sum(i => i.Service.Price)
                    })
                    .ToList();

                var barValues = new ChartValues<decimal>();
                foreach (var item in revenueByService)
                {
                    barValues.Add(item.TotalRevenue);
                }

                BarSeries.Add(new ColumnSeries
                {
                    Title = "Выручка",
                    Values = barValues,
                    DataLabels = true,
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(52, 152, 219))
                });

                BarLabels = revenueByService.Select(r => r.ServiceName).ToArray();
            }
        }
    }
}