using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("Автомобили")]
    public class Автомобиль
    {
        [Key]
        public int ID_Авто { get; set; }
        public string Гос_Номер { get; set; }
        public string Марка_Модель { get; set; }
        public string Класс { get; set; }
        public decimal Цена_Сутки { get; set; }
        public int Год_Выпуска { get; set; }
        public int Текущий_Пробег { get; set; }
        public string Статус { get; set; }
        public virtual ICollection<Аренда> Аренды { get; set; } = new List<Аренда>();
    }

    [Table("Клиенты")]
    public class Клиент
    {
        [Key]
        public int ID_Клиента { get; set; }
        public string ФИО { get; set; }
        public string Паспорт { get; set; }
        public string ВУ { get; set; }
        public string Телефон { get; set; }
        public bool Черный_Список { get; set; }

        public virtual ICollection<Аренда> Аренды { get; set; } = new List<Аренда>();
    }

    [Table("Аренда")]
    public class Аренда
    {
        [Key]
        public int ID_Аренды { get; set; }

        public int ID_Авто { get; set; }
        public int ID_Клиента { get; set; }

        public DateTime Дата_Выдачи { get; set; }
        public DateTime Дата_Возврата_План { get; set; }
        public DateTime? Дата_Возврата_Факт { get; set; }
        public decimal? Сумма_Итого { get; set; }
        public decimal Залог { get; set; }
        public string Статус { get; set; }

        [ForeignKey("ID_Авто")]
        public virtual Автомобиль Автомобиль { get; set; }

        [ForeignKey("ID_Клиента")]
        public virtual Клиент Клиент { get; set; }
        public virtual ICollection<ДТП> ДТП_Записи { get; set; } = new List<ДТП>();
    }

    [Table("ДТП")]
    public class ДТП
    {
        [Key]
        public int ID_ДТП { get; set; }

        public int ID_Аренды { get; set; }

        public DateTime Дата_ДТП { get; set; }
        public string Описание { get; set; }
        public decimal Сумма_Ущерба { get; set; }
        public bool Вина_Клиента { get; set; }

        [ForeignKey("ID_Аренды")]
        public virtual Аренда Аренда { get; set; }
    }
}