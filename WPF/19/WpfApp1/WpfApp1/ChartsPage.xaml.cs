using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using WpfApp1.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfApp1
{
    public partial class ChartsPage : Page, INotifyPropertyChanged
    {
        private ISeries[] _pieSeries;
        private ISeries[] _lineSeries;
        private Axis[] _xAxes;

        public event PropertyChangedEventHandler PropertyChanged;

        public ISeries[] PieSeries
        {
            get => _pieSeries;
            set
            {
                _pieSeries = value;
                OnPropertyChanged();
            }
        }

        public ISeries[] LineSeries
        {
            get => _lineSeries;
            set
            {
                _lineSeries = value;
                OnPropertyChanged();
            }
        }

        public Axis[] XAxes
        {
            get => _xAxes;
            set
            {
                _xAxes = value;
                OnPropertyChanged();
            }
        }

        public ChartsPage()
        {
            InitializeComponent();
            DataContext = this;

            DpStart.SelectedDate = DateTime.Now.AddMonths(-1);
            DpEnd.SelectedDate = DateTime.Now;

            LoadChartData();
        }

        private void LoadChartData()
        {
            if (!DpStart.SelectedDate.HasValue || !DpEnd.SelectedDate.HasValue) return;

            DateTime startDate = DpStart.SelectedDate.Value.Date;
            DateTime endDate = DpEnd.SelectedDate.Value.Date.AddDays(1).AddTicks(-1);

            using (var context = new ElkiTorgContext())
            {
                var baseQuery = context.OrderDetails
                    .Include(od => od.Order)
                    .Include(od => od.Product)
                    .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate);

                var topProducts = baseQuery
                    .GroupBy(od => od.Product.ModelName)
                    .Select(g => new
                    {
                        Name = g.Key,
                        TotalSold = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(x => x.TotalSold)
                    .Take(5)
                    .ToList();

                var pieSeriesList = new List<ISeries>();
                foreach (var item in topProducts)
                {
                    pieSeriesList.Add(new PieSeries<int>
                    {
                        Values = new[] { item.TotalSold },
                        Name = item.Name,
                        DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                        DataLabelsFormatter = point => $"{point.Coordinate.PrimaryValue} шт"
                    });
                }
                PieSeries = pieSeriesList.ToArray();

                var revenueData = baseQuery
                    .ToList()
                    .GroupBy(od => od.Order.OrderDate.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Revenue = g.Sum(od => od.Quantity * od.Product.Price)
                    })
                    .OrderBy(x => x.Date)
                    .ToList();

                LineSeries = new ISeries[]
                {
                    new LineSeries<double>
                    {
                        Values = revenueData.Select(x => (double)x.Revenue).ToArray(),
                        Name = "Выручка",
                        Fill = null,
                        GeometrySize = 10,
                        YToolTipLabelFormatter = chartPoint => $"{chartPoint.Coordinate.PrimaryValue} руб"
                    }
                };

                XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = revenueData.Select(x => x.Date.ToString("dd.MM.yyyy")).ToArray(),
                        Name = "Даты",
                        LabelsRotation = 15
                    }
                };
            }
        }

        private void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (DpStart.SelectedDate > DpEnd.SelectedDate)
            {
                MessageBox.Show("Начальная дата не может быть больше конечной.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            LoadChartData();
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}