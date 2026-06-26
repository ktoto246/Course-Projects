using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    [Table("work_logs")]
    public class WorkLog
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("incident_id")]
        public int IncidentId { get; set; }

        [Required]
        [Column("brigade_id")]
        public int BrigadeId { get; set; }

        [Required]
        [Column("departure_time")]
        public DateTime DepartureTime { get; set; }

        [Column("fix_time")]
        public DateTime? FixTime { get; set; }

        [ForeignKey(nameof(IncidentId))]
        public virtual Incident Incident { get; set; } = null!;

        [ForeignKey(nameof(BrigadeId))]
        public virtual Brigade Brigade { get; set; } = null!;
    }
}
