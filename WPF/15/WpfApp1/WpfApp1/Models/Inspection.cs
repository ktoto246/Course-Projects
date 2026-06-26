namespace WpfApp1.Models
{
    public class Inspection
    {
        public int Id { get; set; }
        public int JointId { get; set; }
        public string InspectionMethod { get; set; } = string.Empty;
        public bool IsPassed { get; set; }
        public DateTime InspectionDate { get; set; }
        public Joint? Joint { get; set; }
    }
}
