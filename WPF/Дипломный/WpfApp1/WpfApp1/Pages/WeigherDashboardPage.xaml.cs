using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;

namespace WpfApp1.Pages
{
    public partial class WeigherDashboardPage : Page
    {
        private AppDbContext _db;

        public WeigherDashboardPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            try
            {
                var today = DateTime.Today;

                var todayBatches = _db.GrainBatches
                    .Include(b => b.Crop)
                    .Where(b => b.ArrivalDate.Date == today)
                    .ToList();

                txtBatchesToday.Text = todayBatches.Count.ToString();
                txtWaitingLab.Text = _db.GrainBatches.Count(b => b.Status == "Ожидает анализа").ToString();
                txtNetWeightToday.Text = todayBatches.Sum(b => b.NetWeight).ToString("F1");

                var storages = _db.Storages.ToList();
                var maxStorage = storages.OrderByDescending(s => s.Capacity > 0 ? s.CurrentLoad / s.Capacity : 0).FirstOrDefault();

                if (maxStorage != null && maxStorage.Capacity > 0)
                {
                    txtMaxStorage.Text = maxStorage.Name;
                    txtMaxStoragePercent.Text = $"{(maxStorage.CurrentLoad / maxStorage.Capacity * 100):F0}% заполнен";
                }
                else
                {
                    txtMaxStorage.Text = "Нет данных";
                    txtMaxStoragePercent.Text = "—";
                }

                var statuses = todayBatches
                    .GroupBy(b => b.Status)
                    .Select(g => new StatusStat
                    {
                        StatusName = g.Key,
                        Count = g.Count(),
                        BadgeColor = GetStatusColor(g.Key)
                    }).ToList();
                icStatuses.ItemsSource = statuses;

                int maxCrops = todayBatches.Count > 0 ? todayBatches.GroupBy(b => b.CropId).Max(g => g.Count()) : 1;

                var crops = todayBatches
                    .GroupBy(b => b.Crop.Name)
                    .Select(g => new CropStat
                    {
                        CropName = g.Key,
                        Count = g.Count(),
                        MaxCount = maxCrops,
                        BarColor = GetRandomColor(g.Key)
                    }).OrderByDescending(c => c.Count).ToList();
                icCrops.ItemsSource = crops;

                var freeSpaceData = storages.Select(s =>
                {
                    decimal free = Math.Max(0, s.Capacity - s.CurrentLoad);
                    return new StorageFreeStat
                    {
                        StorageName = s.Name,
                        Capacity = (double)s.Capacity,
                        FreeSpace = (double)free,
                        FreeSpaceText = $"{free:F0} т",
                        BarColor = free < (s.Capacity * 0.2m) ? Brushes.IndianRed : Brushes.MediumSeaGreen
                    };
                }).ToList();
                icFreeSpace.ItemsSource = freeSpaceData;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных:\n{ex.Message}", "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private SolidColorBrush GetStatusColor(string status)
        {
            return status switch
            {
                "Ожидает анализа" => (SolidColorBrush)new BrushConverter().ConvertFrom("#F59E0B"),
                "Проведен анализ" => (SolidColorBrush)new BrushConverter().ConvertFrom("#3B82F6"),
                "Завершено" => (SolidColorBrush)new BrushConverter().ConvertFrom("#10B981"),
                _ => (SolidColorBrush)new BrushConverter().ConvertFrom("#94A3B8")
            };
        }

        private SolidColorBrush GetRandomColor(string seedString)
        {
            var colors = new[] { "#8B5CF6", "#F59E0B", "#10B981", "#EC4899", "#3B82F6" };
            int index = Math.Abs(seedString.GetHashCode()) % colors.Length;
            return (SolidColorBrush)new BrushConverter().ConvertFrom(colors[index]);
        }

        public class StatusStat
        {
            public string? StatusName { get; set; }
            public int Count { get; set; }
            public SolidColorBrush? BadgeColor { get; set; }
        }

        public class CropStat
        {
            public string? CropName { get; set; }
            public int Count { get; set; }
            public int MaxCount { get; set; }
            public SolidColorBrush? BarColor { get; set; }
        }

        public class StorageFreeStat
        {
            public string? StorageName { get; set; }
            public double Capacity { get; set; }
            public double FreeSpace { get; set; }
            public string? FreeSpaceText { get; set; }
            public SolidColorBrush? BarColor { get; set; }
        }
    }
}