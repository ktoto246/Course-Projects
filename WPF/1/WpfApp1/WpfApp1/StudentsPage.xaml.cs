using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class StudentsPage : Page
    {
        private CollegeContext _context;
        private Student _selectedStudent;

        public StudentsPage()
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
            StudentsGrid.ItemsSource = _context.Students.Include(s => s.Group).ToList();
            CmbGroup.ItemsSource = _context.Groups.ToList();
        }

        private void StudentsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StudentsGrid.SelectedItem is Student student)
            {
                _selectedStudent = student;

                TxtLastName.Text = student.LastName;
                TxtFirstName.Text = student.FirstName;
                TxtMiddleName.Text = student.MiddleName;
                TxtRecordBook.Text = student.RecordBookNumber;
                CmbGroup.SelectedValue = student.GroupId;
                TxtPhone.Text = student.Phone;
                TxtYear.Text = student.EnrollmentYear.ToString();
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(TxtLastName.Text) ||
                string.IsNullOrWhiteSpace(TxtFirstName.Text) ||
                string.IsNullOrWhiteSpace(TxtRecordBook.Text) ||
                CmbGroup.SelectedValue == null ||
                string.IsNullOrWhiteSpace(TxtYear.Text))
            {
                MessageBox.Show("Заполни все обязательные поля со звездочкой!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtYear.Text, out _))
            {
                MessageBox.Show("Год поступления должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var newStudent = new Student
            {
                LastName = TxtLastName.Text.Trim(),
                FirstName = TxtFirstName.Text.Trim(),
                MiddleName = TxtMiddleName.Text.Trim(),
                RecordBookNumber = TxtRecordBook.Text.Trim(),
                GroupId = (int)CmbGroup.SelectedValue,
                Phone = TxtPhone.Text.Trim(),
                EnrollmentYear = int.Parse(TxtYear.Text.Trim())
            };

            try
            {
                _context.Students.Add(newStudent);
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
            if (_selectedStudent == null)
            {
                MessageBox.Show("Сначала выбери студента в таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ValidateForm()) return;

            try
            {
                _selectedStudent.LastName = TxtLastName.Text.Trim();
                _selectedStudent.FirstName = TxtFirstName.Text.Trim();
                _selectedStudent.MiddleName = TxtMiddleName.Text.Trim();
                _selectedStudent.RecordBookNumber = TxtRecordBook.Text.Trim();
                _selectedStudent.GroupId = (int)CmbGroup.SelectedValue;
                _selectedStudent.Phone = TxtPhone.Text.Trim();
                _selectedStudent.EnrollmentYear = int.Parse(TxtYear.Text.Trim());

                _context.Students.Update(_selectedStudent);
                _context.SaveChanges();

                StudentsGrid.ItemsSource = null;
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
            if (_selectedStudent == null)
            {
                MessageBox.Show("Выбери студента для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Точно снести студента?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Students.Remove(_selectedStudent);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
                catch (Exception)
                {
                    MessageBox.Show("Нельзя удалить студента, пока у него есть записи в журнале!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedStudent = null;
            StudentsGrid.SelectedItem = null;

            TxtLastName.Clear();
            TxtFirstName.Clear();
            TxtMiddleName.Clear();
            TxtRecordBook.Clear();
            CmbGroup.SelectedItem = null;
            TxtPhone.Clear();
            TxtYear.Clear();
        }
    }
}