namespace HospitalCashRegister.Models
{
    public class MedicalService
    {
        public string Id { get; set; } = Guid.Empty.ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        public bool Status { get; set; } = true;
    }
}