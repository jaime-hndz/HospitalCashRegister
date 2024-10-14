namespace HospitalCashRegister.Models
{
    public class CashRegister
    {
        public string Id { get; set; } = Guid.Empty.ToString();
        public string CashierId { get; set; } = Guid.Empty.ToString();
        public string BranchId { get; set; } = Guid.Empty.ToString();
        public DateTime OpeningDate { get; set; } = DateTime.Now;
        public DateTime? ClosingDate { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal CashInflow { get; set; } = 0.00M;
        public decimal CashOutflow { get; set; } = 0.00M;
        public decimal FinalAmount { get; set; } = 0.00M;
        public int CashRegisterStatusId { get; set; } = 0;
        public Cashier? Cashier { get; set; } = default!;
        public Branch? Branch { get; set; } = default!;

        public List<Transaction> transactions { get; set; } = new List<Transaction>();
    }

    public enum CashRegisterStatus
    {
        Open,
        Closed

    }
}