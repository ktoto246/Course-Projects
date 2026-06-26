using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    [Table("Типы_СВТ")]
    public class ТипСвт
    {
        [Key]
        public int IDТипа { get; set; }
        [Required]
        public string Название { get; set; }
        public virtual ICollection<Техника> Техника { get; set; }
    }

    [Table("Сотрудники")]
    public class Сотрудник
    {
        [Key]
        public int IDСотрудника { get; set; }
        [Required]
        public string ФИО { get; set; }
        public string Отдел { get; set; }
        public virtual ICollection<Техника> Техника { get; set; }
    }

    [Table("Техника")]
    public class Техника
    {
        [Key]
        public int IDТехники { get; set; }
        [Required]
        public string Инвентарный_номер { get; set; }
        public string Модель { get; set; }
        public string Статус { get; set; }

        public int? ID_Типа { get; set; }
        [ForeignKey("ID_Типа")]
        public virtual ТипСвт ТипСвт { get; set; }

        public int? ID_Сотрудника { get; set; }
        [ForeignKey("ID_Сотрудника")]
        public virtual Сотрудник Сотрудник { get; set; }

        public virtual ICollection<Комплектующее> Комплектующие { get; set; }
    }

    [Table("Комплектующие")]
    public class Комплектующее
    {
        [Key]
        public int IDКомплектующего { get; set; }
        [Required]
        public string Название { get; set; }
        public string Модель { get; set; }
        public string Серийный_номер { get; set; }

        public int? ID_Техники { get; set; }
        [ForeignKey("ID_Техники")]
        public virtual Техника Техника { get; set; }
    }
}