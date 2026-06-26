using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Data;
using Microsoft.EntityFrameworkCore;
using LiveCharts;
using LiveCharts.Wpf;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Логика взаимодействия для AnalyticsPage.xaml
    /// </summary>
    public partial class AnalyticsPage : Page
    {
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }

        public AnalyticsPage()
        {
            InitializeComponent();
            LoadData();
            DataContext = this;
        }

        private void LoadData()
        {
            using var context = new GasRegionDbContext();

            var stats = context.Inspections
                .Include(i => i.Joint)
                .ThenInclude(j => j.Welder)
                .ToList()
                .GroupBy(i => i.Joint.Welder.FullName)
                .Select(g => new
                {
                    WelderName = g.Key,
                    DefectPercentage = g.Count() > 0
                        ? (double)g.Count(i => !i.IsPassed) / g.Count() * 100
                        : 0
                })
                .ToList();

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Брак (%)",
                    Values = new ChartValues<double>(stats.Select(s => s.DefectPercentage))
                }
            };

            Labels = stats.Select(s => s.WelderName).ToArray();
        }
    }
}