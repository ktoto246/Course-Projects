using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    internal class Edinica
    {
        public int ID_Edinica { get; set; }
        public int DonorID { get; set; }
        public string Component { get; set; }
        public int FK_Status { get; set; }
        public DateTime Date_Sbora { get; set; }
        public DateTime? Date_Freeze { get; set; }
        public string BloodGroup { get; set; }
        public bool? Rh { get; set; }
        public Status StatusObj { get; set; }
        public string RhString
        {
            get
            {
                if (Rh == true) return "Rh+";
                if (Rh == false) return "Rh-";
                return "Не указано";
            }
        }
        public string DateFreezeString
        {
            get
            {
                return Date_Freeze?.ToString("dd.MM.yyyy") ?? "Не заморожена";
            }
        }
        public string Status
        {
            get { return StatusObj?.StatusName; }
        }
    }
}
