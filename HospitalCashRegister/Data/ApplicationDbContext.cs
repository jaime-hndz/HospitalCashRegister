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
            modelBuilder.Entity<Cashier>()
                .HasOne<Branch>()
                .WithMany()
                .HasForeignKey(c => c.BranchId);

            modelBuilder.Entity<Transaction>()
                .HasOne<Cashier>()
                .WithMany()
                .HasForeignKey(t => t.CashierId);

            modelBuilder.Entity<Transaction>()
                .HasOne<Patient>()
                .WithMany()
                .HasForeignKey(t => t.PatientId);

            modelBuilder.Entity<Transaction>()
                .HasOne<MedicalService>()
                .WithMany()
                .HasForeignKey(t => t.ServiceId);

            modelBuilder.Entity<CashRegister>()
                .HasOne<Cashier>()
                .WithMany()
                .HasForeignKey(cr => cr.CashierId);

            modelBuilder.Entity<Receipt>()
                .HasOne<Transaction>()
                .WithMany()
                .HasForeignKey(r => r.TransactionId);
        }
    }
}
