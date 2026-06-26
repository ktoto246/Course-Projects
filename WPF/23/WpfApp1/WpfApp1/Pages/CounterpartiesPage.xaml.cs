using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class CounterpartiesPage : Page
    {
        private AppDbContext _context;

        public CounterpartiesPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            CounterpartiesGrid.ItemsSource = _context.Counterparties.ToList();
        }

        private void CounterpartiesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CounterpartiesGrid.SelectedItem is Counterparty selected)
            {
                txtName.Text = selected.Name;
                cmbType.Text = selected.Type;
                txtINN.Text = selected.INN;
                txtPhone.Text = selected.Phone;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var newCounterparty = new Counterparty
            {
                Name = txtName.Text,
                Type = cmbType.Text,
                INN = txtINN.Text,
                Phone = txtPhone.Text
            };

            try
            {
                _context.Counterparties.Add(newCounterparty);
                _context.SaveChanges();
                ClearForm();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (CounterpartiesGrid.SelectedItem is Counterparty selected)
            {
                if (!ValidateInput()) return;

                selected.Name = txtName.Text;
                selected.Type = cmbType.Text;
                selected.INN = txtINN.Text;
                selected.Phone = txtPhone.Text;

                try
                {
                    _context.SaveChanges();
                    LoadData();
                    CounterpartiesGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (CounterpartiesGrid.SelectedItem is Counterparty selected)
            {
                if (MessageBox.Show("Точно удалить?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Counterparties.Remove(selected);
                        _context.SaveChanges();
                        ClearForm();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            CounterpartiesGrid.SelectedItem = null;
            txtName.Clear();
            cmbType.SelectedIndex = -1;
            txtINN.Clear();
            txtPhone.Clear();
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(cmbType.Text))
            {
                MessageBox.Show("Заполните обязательные поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}