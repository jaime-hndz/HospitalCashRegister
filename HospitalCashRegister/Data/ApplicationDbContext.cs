using HospitalCashRegister.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalCashRegister.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Branches = Set<Branch>();
            Cashiers = Set<Cashier>();
            Patients = Set<Patient>();
            MedicalServices = Set<MedicalService>();
            Transactions = Set<Transaction>();
            CashRegisters = Set<CashRegister>();
            Receipts = Set<Receipt>();
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<MedicalService> MedicalServices { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CashRegister> CashRegisters { get; set; }
        public DbSet<Receipt> Receipts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configura las relaciones entre tablas
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
