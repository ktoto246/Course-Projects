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

                txtBatchesToday.Text = todayBatches.Count.ToString() + " шт.";
                txtWaitingLab.Text = _db.GrainBatches.Count(b => b.Status == "Ожидает анализа").ToString() + " шт.";
                txtNetWeightToday.Text = todayBatches.Sum(b => b.NetWeight).ToString("F1") + " т";

                var storages = _db.Storages.ToList();
                var maxStorage = storages.OrderByDescending(s => s.Capacity > 0 ? s.CurrentLoad / s.Capacity : 0).FirstOrDefault();
                if (maxStorage != null && maxStorage.Capacity > 0)
                {
                    double percent = (double)(maxStorage.CurrentLoad / maxStorage.Capacity) * 100;
                    txtMaxLoadedStorage.Text = $"{maxStorage.Name} ({percent:F0}%)";
                }

                var statusList = new List<string> { "Ожидает анализа", "Проведен анализ", "Завершено" };
                var statusStats = statusList.Select(s => new StatusStat
                {
                    StatusName = s,
                    Count = _db.GrainBatches.Count(b => b.ArrivalDate.Date == today && b.Status == s),
                    BadgeColor = GetStatusColor(s)
                }).ToList();
                icStatuses.ItemsSource = statusStats;

                var cropGroups = todayBatches.GroupBy(b => b.Crop.Name).ToList();
                int maxCropCount = cropGroups.Any() ? cropGroups.Max(g => g.Count()) : 1;
                var cropStats = cropGroups.Select(g => new CropStat
                {
                    CropName = g.Key,
                    Count = g.Count(),
                    MaxCount = maxCropCount,
                    BarColor = GetRandomColor(g.Key)
                }).ToList();
                icCrops.ItemsSource = cropStats;

                var storageStats = new List<StorageFreeStat>();
                foreach (var s in storages)
                {
                    var culturesOnStorage = _db.GrainBatches
                        .Include(b => b.Crop)
                        .Where(b => b.StorageId == s.Id && b.Status != "Завершено")
                        .Select(b => b.Crop.Name)
                        .Distinct()
                        .ToList();

                    string storedCropsText = culturesOnStorage.Any() ? string.Join(", ", culturesOnStorage) : "Пусто";
                    double capacity = (double)s.Capacity;
                    double currentLoad = (double)s.CurrentLoad;
                    double freeSpace = capacity - currentLoad;

                    bool isOverload = freeSpace < 0;
                    SolidColorBrush barColor = isOverload
                        ? (SolidColorBrush)new BrushConverter().ConvertFrom("#EF4444")
                        : (SolidColorBrush)new BrushConverter().ConvertFrom("#10B981");

                    string spaceText = isOverload
                        ? $"ПЕРЕВЕС: +{Math.Abs(freeSpace):F1} т / {capacity:F0} т"
                        : $"Свободно: {freeSpace:F1} т / {capacity:F0} т";

                    storageStats.Add(new StorageFreeStat
                    {
                        StorageName = s.Name,
                        Capacity = capacity,
                        CurrentLoad = currentLoad,
                        FreeSpace = isOverload ? capacity : currentLoad,
                        BarColor = barColor,
                        SpaceText = spaceText,
                        StoredCrops = storedCropsText
                    });
                }
                icFreeSpace.ItemsSource = storageStats;

                var expiringAlerts = _db.GrainBatches
                    .Where(b => b.Status != "Завершено")
                    .ToList()
                    .Where(b => (DateTime.Today - b.ArrivalDate.Date).TotalDays >= 30)
                    .Select(b => $"Транспорт {b.CarNumber} (Партия #{b.Id}) — Находится на элеваторе уже {(DateTime.Today - b.ArrivalDate.Date).TotalDays} дн. Срочно выписать счет!")
                    .ToList();

                if (expiringAlerts.Any())
                {
                    alertBorder.Visibility = Visibility.Visible;
                    icAlerts.ItemsSource = expiringAlerts;
                }
                else
                {
                    alertBorder.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка обновления оперативных данных весовой:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private SolidColorBrush GetRandomColor(string seed)
        {
            var colors = new[] { "#6366F1", "#3B82F6", "#10B981", "#F59E0B", "#EC4899" };
            int index = Math.Abs(seed.GetHashCode()) % colors.Length;
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
            public double CurrentLoad { get; set; }
            public double FreeSpace { get; set; }
            public SolidColorBrush? BarColor { get; set; }
            public string? SpaceText { get; set; }
            public string? StoredCrops { get; set; }
        }
    }
}