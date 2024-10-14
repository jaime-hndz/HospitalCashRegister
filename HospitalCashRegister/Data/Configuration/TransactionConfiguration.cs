using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalCashRegister.Data.Configuration
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Models.Transaction>
    {
        public void Configure(EntityTypeBuilder<Models.Transaction> builder)
        {
            builder.ToTable(nameof(Models.Transaction));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("_id");
            builder.Property(x => x.CashierId).HasColumnName("CashierId");
            builder.Property(x => x.PatientId).HasColumnName("PatientId");
            /*builder.Property(x => x.MedicalServiceId).HasColumnName("MedicalServiceId");*/
            builder.Property(x => x.CashRegisterId).HasColumnName("CashRegisterId");
            builder.Property(x => x.TransactionTypeId).HasColumnName("TransactionTypeId");
            builder.Property(x => x.TransactionStatusId).HasColumnName("TransactionStatusId");
            builder.Property(x => x.Amount).HasColumnName("Amount");
            builder.Property(x => x.Date).HasColumnName("Date");
            builder.Property(x => x.Comment).HasColumnName("Comment");
            builder.Property(x => x.CashDetails).HasColumnName("CashDetails");
            builder.Ignore(x => x.ServiceIds);

            builder.HasMany(x => x.ServiceOrders).WithOne().HasForeignKey(x => x.TransactionId);
            builder.HasOne<Models.Cashier>(x => x.Cashier).WithMany().HasForeignKey(t => t.CashierId);
            builder.HasOne<Models.Patient>(x => x.Patient).WithMany().HasForeignKey(t => t.PatientId);
            /*builder.HasOne<Models.MedicalService>(x => x.MedicalService).WithMany().HasForeignKey(t => t.MedicalServiceId);*/



        }
    }
}
