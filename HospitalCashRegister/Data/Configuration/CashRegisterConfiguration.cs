using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalCashRegister.Data.Configuration
{
    public class CashRegisterConfiguration : IEntityTypeConfiguration<Models.CashRegister>
    {
        public void Configure(EntityTypeBuilder<Models.CashRegister> builder)
        {
            builder.ToTable(nameof(Models.CashRegister));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("_id");
            builder.Property(x => x.CashierId).HasColumnName("CashierId");
            builder.Property(x => x.BranchId).HasColumnName("BranchId");
            builder.Property(x => x.OpeningDate).HasColumnName("OpeningDate");
            builder.Property(x => x.InitialAmount).HasColumnName("InitialAmount");
            builder.Property(x => x.CashInflow).HasColumnName("CashInflow");
            builder.Property(x => x.CashOutflow).HasColumnName("CashOutflow");
            builder.Property(x => x.FinalAmount).HasColumnName("FinalAmount");
            builder.Property(x => x.CashRegisterStatusId).HasColumnName("CashRegisterStatusId");
            builder.Ignore(x => x.transactions);
            builder.HasOne<Models.Cashier>(x => x.Cashier).WithMany().HasForeignKey(t => t.CashierId);
            builder.HasOne<Models.Branch>(x => x.Branch).WithMany().HasForeignKey(t => t.BranchId);


        }
    }
}
