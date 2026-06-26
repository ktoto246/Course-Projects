using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class MillPage : Page
    {
        private AppDbContext _context;
        private List<Mill> _mills = new List<Mill>();
        private int currentId = 0;

        public MillPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadData();
        }

        private void LoadData()
        {
            _mills = _context.Mills.ToList();
            dataGrid.ItemsSource = _mills;
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox searchBox = sender as TextBox;
            if (searchBox == null) return;

            string filter = searchBox.Text.ToLower();
            var filtered = _mills.Where(m =>
                m.MillName.ToLower().Contains(filter) ||
                m.YearBuilt.ToString().Contains(filter)).ToList();

            dataGrid.ItemsSource = filtered;
        }

        private void OnDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem is Mill selectedMill)
            {
                txtName.Text = selectedMill.MillName;
                txtYear.Text = selectedMill.YearBuilt.ToString();
                currentId = selectedMill.MillId;
            }
        }

        private void OnAddClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || !int.TryParse(txtYear.Text, out int year))
            {
                MessageBox.Show("Введите корректное название и год постройки (только цифры)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var mill = new Mill
            {
                MillName = txtName.Text,
                YearBuilt = year
            };

            _context.Mills.Add(mill);
            _context.SaveChanges();
            LoadData();
            ClearFields();
        }

        private void OnEditClick(object sender, RoutedEventArgs e)
        {
            if (currentId > 0)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) || !int.TryParse(txtYear.Text, out int year))
                {
                    MessageBox.Show("Введите корректное название и год постройки (только цифры)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var mill = _context.Mills.Find(currentId);
                if (mill != null)
                {
                    mill.MillName = txtName.Text;
                    mill.YearBuilt = year;

                    _context.SaveChanges();
                    LoadData();
                    ClearFields();
                }
            }
        }

        private void OnDeleteClick(object sender, RoutedEventArgs e)
        {
            if (currentId > 0)
            {
                var result = MessageBox.Show("Точно удалить мельницу? Это может удалить связанные заказы!", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    var mill = _context.Mills.Find(currentId);
                    if (mill != null)
                    {
                        _context.Mills.Remove(mill);
                        _context.SaveChanges();
                        LoadData();
                        ClearFields();
                    }
                }
            }
        }

        private void OnClearClick(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            txtName.Text = "";
            txtYear.Text = "";
            currentId = 0;
            dataGrid.SelectedItem = null;
        }
    }
}