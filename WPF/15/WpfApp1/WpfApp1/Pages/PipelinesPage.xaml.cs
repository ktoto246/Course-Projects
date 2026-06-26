using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Data;
using WpfApp1.Models;

namespace WpfApp1.Pages
{
    public partial class PipelinesPage : Page
    {
        private readonly GasRegionDbContext _context = new();
        private ObservableCollection<Pipeline> _pipelines;
        private Pipeline? _selectedPipeline;
        private bool _isEditing = false;

        public PipelinesPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _pipelines = new ObservableCollection<Pipeline>(_context.Pipelines.ToList());
                PipelinesDataGrid.ItemsSource = _pipelines;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Заполните все обязательные поля");
                    return;
                }

                if (_isEditing && _selectedPipeline != null)
                {
                    _selectedPipeline.Name = NameTextBox.Text;
                    _context.Pipelines.Update(_selectedPipeline);
                }
                else
                {
                    var newPipeline = new Pipeline
                    {
                        Name = NameTextBox.Text
                    };
                    _context.Pipelines.Add(newPipeline);
                    _pipelines.Add(newPipeline);
                }

                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedPipeline == null)
            {
                MessageBox.Show("Выберите трубопровод для удаления");
                return;
            }

            if (MessageBox.Show("Удалить выбранный трубопровод?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Pipelines.Remove(_selectedPipeline);
                    _context.SaveChanges();
                    _pipelines.Remove(_selectedPipeline);
                    ClearForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                }
            }
        }

        private void PipelinesDataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (PipelinesDataGrid.SelectedItem is Pipeline pipeline)
            {
                _selectedPipeline = pipeline;
                NameTextBox.Text = pipeline.Name;
                _isEditing = true;
            }
        }

        private void ClearForm()
        {
            NameTextBox.Text = string.Empty;
            _selectedPipeline = null;
            _isEditing = false;
        }
    }
}
