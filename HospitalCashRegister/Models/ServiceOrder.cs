namespace HospitalCashRegister.Models
{
    public class ServiceOrder
    {
        public string Id { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string ServiceId { get; set; } = string.Empty;
        public MedicalService MedicalService { get; set; } = default!;
    }
}
