namespace HospitalCashRegister.Models
{
    public class Transaction
    {
        public string Id { get; set; } = string.Empty;
        public string CashierId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        /*public string MedicalServiceId { get; set; } = string.Empty;*/
        public List<string> ServiceIds { get; set; } = new List<string>();
        public string CashRegisterId { get; set; } = string.Empty;
        public TransactionType TransactionTypeId { get; set; } = 0;
        public TransctionStatus TransactionStatusId { get; set; } = 0;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Comment { get; set; }
        public Cashier? Cashier { get; set; } = default!;
        public Patient? Patient { get; set; } = default!;
        public MedicalService? MedicalService { get; set; } = default!;
        public CashRegister? CashRegister { get; set; } = default!;
        public IEnumerable<ServiceOrder> ServiceOrders { get; set; } = new List<ServiceOrder>();

    }

    public enum TransactionType
    {
        CashInflow,
        CashOutflow,
        ServicePayment

    }
    public enum TransctionStatus
    {
        pending,
        applied,
        rollback
    }
}