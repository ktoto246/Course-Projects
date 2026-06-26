using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    [Table("brigades")]
    public class Brigade
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("district_id")]
        public int DistrictId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("call_sign")]
        public string CallSign { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("leader_name")]
        public string LeaderName { get; set; } = string.Empty;

        [ForeignKey(nameof(DistrictId))]
        public virtual District District { get; set; } = null!;
        public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
        public ICollection<Worker> Workers { get; set; } = new List<Worker>();
    }
}
