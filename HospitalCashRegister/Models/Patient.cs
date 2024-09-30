namespace HospitalCashRegister.Models
{
    public class Patient
    {
        public string Id { get; set; } = Guid.Empty.ToString();
        public string Name { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string PhoneNumber1 { get; set; } = string.Empty;
        public string PhoneNumber2 { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}