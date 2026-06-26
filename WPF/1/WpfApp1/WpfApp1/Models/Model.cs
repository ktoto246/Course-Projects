using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(50)]
        public string Position { get; set; }

        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }

    public class Group
    {
        [Key]
        public int GroupId { get; set; }

        [Required, MaxLength(20)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string Specialty { get; set; }

        [Required, MaxLength(20)]
        public string SpecialtyCode { get; set; }
        public int Course { get; set; }

        public int? CuratorId { get; set; }
        [ForeignKey("CuratorId")]
        public virtual Teacher Curator { get; set; }
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }

    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [Required, MaxLength(15)]
        public string RecordBookNumber { get; set; }

        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        public int EnrollmentYear { get; set; }

        // Навигационные свойства
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }

    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        public int TotalHours { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }

    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }

        public int GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        public int SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }

        public int TeacherId { get; set; }
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; }

        public DateTime ClassDate { get; set; }
        public int LessonNumber { get; set; }

        [Required, MaxLength(20)]
        public string Room { get; set; }

        [Required, MaxLength(20)]
        public string ClassType { get; set; }

        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }

    public class Attendance
    {
        [Key]
        public int RecordId { get; set; }

        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual Schedule Schedule { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; }

        public int? Grade { get; set; }

        [Required, MaxLength(20)]
        public string ControlType { get; set; }

        public DateTime EntryDate { get; set; }
    }
}