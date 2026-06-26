using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    [Table("districts")]
    public class District
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        [Column("dispatcher_phone")]
        public string DispatcherPhone { get; set; } = string.Empty;

        public virtual ICollection<Brigade> Brigades { get; set; } = new List<Brigade>();
        public virtual ICollection<Incident> Incidents { get; set; } = new List<Incident>();
    }
}
