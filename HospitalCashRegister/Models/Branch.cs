namespace HospitalCashRegister.Models
{
    public class Branch
    {
        public string Id { get; set; } = Guid.Empty.ToString();
        public string Name { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        public bool Status { get; set; } = true;
    }
}