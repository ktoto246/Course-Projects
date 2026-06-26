using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuButton_Checked(object sender, RoutedEventArgs e)
        {
            if (MainFrame == null) return;

            var radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.Tag != null)
            {
                string pageName = radioButton.Tag.ToString();

                MainFrame.Navigate(new Uri(pageName, UriKind.Relative));
            }
        }
    }
}