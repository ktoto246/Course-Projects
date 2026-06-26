using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class TransactionsPage : Page
    {
        private AppDbContext _context;

        public TransactionsPage()
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
            TransactionsGrid.ItemsSource = _context.FinancialTransactions
                .Include(t => t.Employee)
                .Include(t => t.Plot)
                .Include(t => t.Counterparty)
                .ToList();

            cmbEmployee.ItemsSource = _context.Employees.ToList();
            cmbPlot.ItemsSource = _context.Plots.ToList();
            cmbCounterparty.ItemsSource = _context.Counterparties.ToList();
        }

        private void TransactionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TransactionsGrid.SelectedItem is FinancialTransaction selected)
            {
                dpDate.SelectedDate = selected.TransactionDate;
                cmbType.Text = selected.Type;
                txtAmount.Text = selected.Amount.ToString();
                txtCategory.Text = selected.Category;
                txtDescription.Text = selected.Description;
                cmbEmployee.SelectedValue = selected.EmployeeID;
                cmbPlot.SelectedValue = selected.PlotID;
                cmbCounterparty.SelectedValue = selected.CounterpartyID;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            var newTransaction = new FinancialTransaction
            {
                TransactionDate = dpDate.SelectedDate.Value,
                Type = cmbType.Text,
                Amount = decimal.Parse(txtAmount.Text),
                Category = txtCategory.Text,
                Description = txtDescription.Text,
                EmployeeID = (int)cmbEmployee.SelectedValue,
                PlotID = (int?)cmbPlot.SelectedValue,
                CounterpartyID = (int?)cmbCounterparty.SelectedValue
            };

            try
            {
                _context.FinancialTransactions.Add(newTransaction);
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
            if (TransactionsGrid.SelectedItem is FinancialTransaction selected)
            {
                if (!ValidateInput()) return;

                selected.TransactionDate = dpDate.SelectedDate.Value;
                selected.Type = cmbType.Text;
                selected.Amount = decimal.Parse(txtAmount.Text);
                selected.Category = txtCategory.Text;
                selected.Description = txtDescription.Text;
                selected.EmployeeID = (int)cmbEmployee.SelectedValue;
                selected.PlotID = (int?)cmbPlot.SelectedValue;
                selected.CounterpartyID = (int?)cmbCounterparty.SelectedValue;

                try
                {
                    _context.SaveChanges();
                    LoadData();
                    TransactionsGrid.Items.Refresh();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (TransactionsGrid.SelectedItem is FinancialTransaction selected)
            {
                if (MessageBox.Show("Точно удалить?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.FinancialTransactions.Remove(selected);
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

        private void BtnClearCombos_Click(object sender, RoutedEventArgs e)
        {
            cmbPlot.SelectedIndex = -1;
            cmbCounterparty.SelectedIndex = -1;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            TransactionsGrid.SelectedItem = null;
            dpDate.SelectedDate = null;
            cmbType.SelectedIndex = -1;
            txtAmount.Clear();
            txtCategory.Clear();
            txtDescription.Clear();
            cmbEmployee.SelectedIndex = -1;
            cmbPlot.SelectedIndex = -1;
            cmbCounterparty.SelectedIndex = -1;
        }

        private bool ValidateInput()
        {
            if (dpDate.SelectedDate == null ||
                string.IsNullOrWhiteSpace(cmbType.Text) ||
                string.IsNullOrWhiteSpace(txtAmount.Text) ||
                string.IsNullOrWhiteSpace(txtCategory.Text) ||
                cmbEmployee.SelectedValue == null)
            {
                MessageBox.Show("Заполните обязательные поля.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!decimal.TryParse(txtAmount.Text, out _))
            {
                MessageBox.Show("В поле 'Сумма' должно быть число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}