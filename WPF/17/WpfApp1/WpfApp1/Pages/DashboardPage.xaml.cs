using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadStats();
        }

        private void LoadStats()
        {
            using var context = new PrikhoperskoeDbContext();

            var activeIncidents = context.Incidents.Count(i => i.Status == "В обработке" || i.Status == "Бригада выехала");
            var activeBrigades = context.WorkLogs.Count(w => w.FixTime == null);
            var totalDistricts = context.Districts.Count();

            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fixedIncidents = context.Incidents.Count(i => i.Status == "Устранено" && i.CreatedAt >= firstDayOfMonth);

            txtActiveIncidents.Text = activeIncidents.ToString();
            txtActiveBrigades.Text = activeBrigades.ToString();
            txtTotalDistricts.Text = totalDistricts.ToString();
            txtFixedIncidents.Text = fixedIncidents.ToString();
        }
    }
}