using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class SubjectsPage : Page
    {
        private CollegeContext _context;
        private Subject _selectedSubject;

        public SubjectsPage()
        {
            InitializeComponent();
            _context = new CollegeContext();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            SubjectsGrid.ItemsSource = _context.Subjects.ToList();
        }

        private void SubjectsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SubjectsGrid.SelectedItem is Subject subject)
            {
                _selectedSubject = subject;

                TxtName.Text = subject.Name;
                TxtHours.Text = subject.TotalHours.ToString();
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text) ||
                string.IsNullOrWhiteSpace(TxtHours.Text))
            {
                MessageBox.Show("Заполни все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtHours.Text, out _))
            {
                MessageBox.Show("Количество часов должно быть целым числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var newSubject = new Subject
            {
                Name = TxtName.Text.Trim(),
                TotalHours = int.Parse(TxtHours.Text.Trim())
            };

            try
            {
                _context.Subjects.Add(newSubject);
                _context.SaveChanges();
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubject == null)
            {
                MessageBox.Show("Выбери дисциплину в таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ValidateForm()) return;

            try
            {
                _selectedSubject.Name = TxtName.Text.Trim();
                _selectedSubject.TotalHours = int.Parse(TxtHours.Text.Trim());

                _context.Subjects.Update(_selectedSubject);
                _context.SaveChanges();

                SubjectsGrid.ItemsSource = null;
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка БД", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSubject == null)
            {
                MessageBox.Show("Выбери дисциплину для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Удалить дисциплину?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Subjects.Remove(_selectedSubject);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
                catch (Exception)
                {
                    MessageBox.Show("Нельзя удалить дисциплину, пока она есть в расписании!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedSubject = null;
            SubjectsGrid.SelectedItem = null;

            TxtName.Clear();
            TxtHours.Clear();
        }
    }
}