using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class SchedulePage : Page
    {
        private CollegeContext _context;
        private Schedule _selectedSchedule;

        public SchedulePage()
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
            ScheduleGrid.ItemsSource = _context.Schedules
                .Include(s => s.Group)
                .Include(s => s.Subject)
                .Include(s => s.Teacher)
                .ToList();

            CmbGroup.ItemsSource = _context.Groups.ToList();
            CmbSubject.ItemsSource = _context.Subjects.ToList();
            CmbTeacher.ItemsSource = _context.Teachers.ToList();
        }

        private void ScheduleGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ScheduleGrid.SelectedItem is Schedule schedule)
            {
                _selectedSchedule = schedule;

                DpClassDate.SelectedDate = schedule.ClassDate;
                TxtLessonNumber.Text = schedule.LessonNumber.ToString();
                CmbGroup.SelectedValue = schedule.GroupId;
                CmbSubject.SelectedValue = schedule.SubjectId;
                CmbTeacher.SelectedValue = schedule.TeacherId;
                TxtRoom.Text = schedule.Room;

                foreach (ComboBoxItem item in CmbClassType.Items)
                {
                    if (item.Content.ToString() == schedule.ClassType)
                    {
                        CmbClassType.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private bool ValidateForm()
        {
            if (DpClassDate.SelectedDate == null ||
                string.IsNullOrWhiteSpace(TxtLessonNumber.Text) ||
                CmbGroup.SelectedValue == null ||
                CmbSubject.SelectedValue == null ||
                CmbTeacher.SelectedValue == null ||
                string.IsNullOrWhiteSpace(TxtRoom.Text) ||
                CmbClassType.SelectedItem == null)
            {
                MessageBox.Show("Заполни все обязательные поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TxtLessonNumber.Text, out _))
            {
                MessageBox.Show("Номер пары должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm()) return;

            var newSchedule = new Schedule
            {
                ClassDate = DpClassDate.SelectedDate.Value,
                LessonNumber = int.Parse(TxtLessonNumber.Text.Trim()),
                GroupId = (int)CmbGroup.SelectedValue,
                SubjectId = (int)CmbSubject.SelectedValue,
                TeacherId = (int)CmbTeacher.SelectedValue,
                Room = TxtRoom.Text.Trim(),
                ClassType = ((ComboBoxItem)CmbClassType.SelectedItem).Content.ToString()
            };

            try
            {
                _context.Schedules.Add(newSchedule);
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
            if (_selectedSchedule == null)
            {
                MessageBox.Show("Выбери занятие в таблице!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!ValidateForm()) return;

            try
            {
                _selectedSchedule.ClassDate = DpClassDate.SelectedDate.Value;
                _selectedSchedule.LessonNumber = int.Parse(TxtLessonNumber.Text.Trim());
                _selectedSchedule.GroupId = (int)CmbGroup.SelectedValue;
                _selectedSchedule.SubjectId = (int)CmbSubject.SelectedValue;
                _selectedSchedule.TeacherId = (int)CmbTeacher.SelectedValue;
                _selectedSchedule.Room = TxtRoom.Text.Trim();
                _selectedSchedule.ClassType = ((ComboBoxItem)CmbClassType.SelectedItem).Content.ToString();

                _context.Schedules.Update(_selectedSchedule);
                _context.SaveChanges();

                ScheduleGrid.ItemsSource = null;
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
            if (_selectedSchedule == null)
            {
                MessageBox.Show("Выбери занятие для удаления!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (MessageBox.Show("Удалить занятие из расписания?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Schedules.Remove(_selectedSchedule);
                    _context.SaveChanges();
                    LoadData();
                    ClearForm();
                }
                catch (Exception)
                {
                    MessageBox.Show("Нельзя удалить занятие, пока по нему есть отметки в журнале!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            _selectedSchedule = null;
            ScheduleGrid.SelectedItem = null;

            DpClassDate.SelectedDate = null;
            TxtLessonNumber.Clear();
            CmbGroup.SelectedItem = null;
            CmbSubject.SelectedItem = null;
            CmbTeacher.SelectedItem = null;
            TxtRoom.Clear();
            CmbClassType.SelectedItem = null;
        }
    }
}