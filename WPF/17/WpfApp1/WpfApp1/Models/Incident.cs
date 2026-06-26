using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    [Table("incidents")]
    public class Incident
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("district_id")]
        public int DistrictId { get; set; }

        [Required]
        [Column("equipment_type_id")]
        public int EquipmentTypeId { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [MaxLength(50)]
        [Column("status")]
        public string Status { get; set; } = "В обработке";

        [ForeignKey(nameof(DistrictId))]
        public virtual District District { get; set; } = null!;

        [ForeignKey(nameof(EquipmentTypeId))]
        public virtual EquipmentType EquipmentType { get; set; } = null!;

        public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
