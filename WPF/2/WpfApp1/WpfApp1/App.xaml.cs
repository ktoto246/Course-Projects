using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var iconUri = new Uri("pack://application:,,,/Resources/icon.ico", UriKind.RelativeOrAbsolute);
            var iconBitmap = new BitmapImage(iconUri);
        }
    }
}
