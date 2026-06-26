using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class DealsPage : Page
    {
        private AppDbContext _context;

        public DealsPage()
        {
            InitializeComponent();
            _context = new AppDbContext();
            LoadComboBoxes();
            LoadData();
        }

        private void LoadComboBoxes()
        {
            CmbClients.ItemsSource = _context.Clients.ToList();
            CmbEmployees.ItemsSource = _context.Employees.ToList();
            CmbProducts.ItemsSource = _context.Products.ToList();
        }

        private void LoadData(string searchText = "")
        {
            var query = _context.Deals
                .Include(d => d.Client)
                .Include(d => d.Employee)
                .Include(d => d.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(d => d.Client.CompanyName.Contains(searchText) ||
                                         d.Employee.FullName.Contains(searchText));
            }

            DataGridDeals.ItemsSource = query.ToList();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData(TxtSearch.Text.Trim());
        }

        private void CalculateTotal()
        {
            if (CmbProducts.SelectedItem is Product selectedProduct &&
                int.TryParse(TxtQuantity.Text, out int quantity) && quantity > 0)
            {
                TxtTotalAmount.Text = (selectedProduct.BasePrice * quantity).ToString("0.00");
            }
            else
            {
                TxtTotalAmount.Text = "0";
            }
        }

        private void CmbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateTotal();
        }

        private void TxtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalculateTotal();
        }

        private void DataGridDeals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGridDeals.SelectedItem is Deal selectedDeal)
            {
                CmbClients.SelectedValue = selectedDeal.ClientId;
                CmbEmployees.SelectedValue = selectedDeal.EmployeeId;
                CmbProducts.SelectedValue = selectedDeal.ProductId;
                TxtQuantity.Text = selectedDeal.Quantity.ToString();
                TxtTotalAmount.Text = selectedDeal.TotalAmount.ToString();
            }
        }

        private bool ValidateInputs()
        {
            if (CmbClients.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (CmbEmployees.SelectedValue == null)
            {
                MessageBox.Show("Выберите сотрудника.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (CmbProducts.SelectedValue == null)
            {
                MessageBox.Show("Выберите продукт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(TxtQuantity.Text, out int qty) || qty <= 0)
            {
                MessageBox.Show("Количество должно быть целым положительным числом.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs()) return;

            var newDeal = new Deal
            {
                ClientId = (int)CmbClients.SelectedValue,
                EmployeeId = (int)CmbEmployees.SelectedValue,
                ProductId = (int)CmbProducts.SelectedValue,
                Quantity = int.Parse(TxtQuantity.Text.Trim()),
                TotalAmount = decimal.Parse(TxtTotalAmount.Text.Trim()),
                DealDate = DateTime.Now
            };

            _context.Deals.Add(newDeal);
            _context.SaveChanges();

            ClearFields();
            LoadData(TxtSearch.Text.Trim());
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridDeals.SelectedItem is Deal selectedDeal)
            {
                if (!ValidateInputs()) return;

                selectedDeal.ClientId = (int)CmbClients.SelectedValue;
                selectedDeal.EmployeeId = (int)CmbEmployees.SelectedValue;
                selectedDeal.ProductId = (int)CmbProducts.SelectedValue;
                selectedDeal.Quantity = int.Parse(TxtQuantity.Text.Trim());
                selectedDeal.TotalAmount = decimal.Parse(TxtTotalAmount.Text.Trim());

                _context.SaveChanges();

                ClearFields();
                LoadData(TxtSearch.Text.Trim());
            }
            else
            {
                MessageBox.Show("Выберите сделку для изменения.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridDeals.SelectedItem is Deal selectedDeal)
            {
                if (MessageBox.Show($"Удалить сделку ID {selectedDeal.Id}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _context.Deals.Remove(selectedDeal);
                    _context.SaveChanges();
                    ClearFields();
                    LoadData(TxtSearch.Text.Trim());
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            CmbClients.SelectedItem = null;
            CmbEmployees.SelectedItem = null;
            CmbProducts.SelectedItem = null;
            TxtQuantity.Clear();
            TxtTotalAmount.Clear();
            DataGridDeals.SelectedItem = null;
        }
    }
}