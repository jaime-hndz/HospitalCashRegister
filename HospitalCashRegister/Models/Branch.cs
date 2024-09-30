namespace HospitalCashRegister.Models
{
    public class Branch
    {
        public string Id { get; set; } = Guid.Empty.ToString();
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}