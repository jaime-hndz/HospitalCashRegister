namespace HospitalCashRegister.Models
{
    public class Receipt
    {
        public string Id { get; set; } = Guid.Empty.ToString();
        public string TransactionId { get; set; } = Guid.Empty.ToString();
        public DateTime GenerationDate { get; set; } = DateTime.Now;
        public string? Details { get; set; }
        public Transaction? Transaction { get; set; } = default!;
    }
}