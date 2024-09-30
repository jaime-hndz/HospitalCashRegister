namespace HospitalCashRegister.Models
{
    public class Cashier
    {
        public string Id { get; set; } = Guid.Empty.ToString();
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public bool Admin { get; set; } = false;
        public string? BranchId { get; set; }
        public bool Status { get; set; } = true;
        public Branch? Branch { get; set; } = default!;
    }
}