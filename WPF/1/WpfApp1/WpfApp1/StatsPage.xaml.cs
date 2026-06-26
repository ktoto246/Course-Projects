using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class StatsPage : Page
    {
        private CollegeContext _context;

        public StatsPage()
        {
            InitializeComponent();
            _context = new CollegeContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                TxtTotalStudents.Text = _context.Students.Count().ToString();
                TxtTotalTeachers.Text = _context.Teachers.Count().ToString();

                var grades = _context.Attendances.Where(a => a.Grade != null).Select(a => a.Grade.Value).ToList();
                if (grades.Any())
                {
                    double avg = grades.Average();
                    TxtAverageGrade.Text = Math.Round(avg, 2).ToString();
                }
                else
                {
                    TxtAverageGrade.Text = "Нет данных";
                }

                var groupsData = _context.Groups
                    .Select(g => new
                    {
                        GroupName = g.Name,
                        StudentCount = g.Students.Count()
                    })
                    .ToList();

                StudentsChart.Series = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Студентов",
                        Values = new ChartValues<int>(groupsData.Select(g => g.StudentCount)),
                        Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(59, 130, 246))
                    }
                };

                AxisX.Labels = groupsData.Select(g => g.GroupName).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}