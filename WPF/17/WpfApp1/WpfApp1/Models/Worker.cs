using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp1.Models
{
    [Table("workers")]
    public class Worker
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("brigade_id")]
        public int BrigadeId { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("position")]
        public string Position { get; set; }

        public Brigade Brigade { get; set; }
    }
}