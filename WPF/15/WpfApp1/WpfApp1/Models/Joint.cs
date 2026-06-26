namespace WpfApp1.Models
{
    public class Joint
    {
        public int Id { get; set; }
        public string JointNumber { get; set; } = string.Empty;
        public int WelderId { get; set; }
        public int PipelineId { get; set; }
        public int MachineId { get; set; }
        public DateTime WeldingDate { get; set; }
        public Welder? Welder { get; set; }
        public Pipeline? Pipeline { get; set; }
        public WeldingMachine? Machine { get; set; }
    }
}
