namespace HospitalCashRegister.Models
{
    public class Transaction
    {
        public string Id { get; set; } = string.Empty;
        public string CashierId { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public string ServiceId { get; set; } = string.Empty;
        public string CashRegisterId { get; set; } = string.Empty;
        public TransactionType TransactionTypeId { get; set; } = TransactionType.CashInflow;
        public TransctionStatus TransactionStatusId { get; set; } = TransctionStatus.pending;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public Cashier? Cashier { get; set; } = default!;
        public Patient? Patient { get; set; } = default!;
        public MedicalService? MedicalService { get; set; } = default!;

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