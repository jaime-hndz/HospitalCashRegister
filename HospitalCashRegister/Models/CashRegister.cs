namespace HospitalCashRegister.Models
{
    public class CashRegister
    {
        public string Id { get; set; } = Guid.Empty.ToString();
        public string CashierId { get; set; } = Guid.Empty.ToString();
        public DateTime OpeningDate { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal CashInflow { get; set; } = 0.00M;
        public decimal CashOutflow { get; set; } = 0.00M;
        public decimal FinalAmount { get; set; } = 0.00M;
        public CashRegisterStatus CashRegisterStatusId { get; set; } = CashRegisterStatus.Open;
    }

    public enum CashRegisterStatus
    {
        Open,
        Closed

    }
}