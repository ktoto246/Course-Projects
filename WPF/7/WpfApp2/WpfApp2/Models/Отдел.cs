using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("Отделы")]
    public class Отдел
    {
        [Key]
        [Column("IDОтдела")] 
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("Название")]
        public string Название { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("Телефон")]
        public string? Телефон { get; set; }

        public ICollection<РабочееМесто> РабочиеМеста { get; set; } = new List<РабочееМесто>();
    }
}