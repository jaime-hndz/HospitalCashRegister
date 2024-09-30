using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalCashRegister.Data.Configuration
{
    public class CashierConfiguration : IEntityTypeConfiguration<Models.Cashier>
    {
        public void Configure(EntityTypeBuilder<Models.Cashier> builder)
        {
            builder.ToTable(nameof(Models.Cashier));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("_id");
            builder.Property(x => x.Username).HasColumnName("Username");
            builder.Property(x => x.Password).HasColumnName("Password");
            builder.Property(x => x.FullName).HasColumnName("FullName");
            builder.Property(x => x.Admin).HasColumnName("Admin");
            builder.Property(x => x.BranchId).HasColumnName("BranchId");
            builder.Property(x => x.LastSeen).HasColumnName("LastSeen");
            builder.Property(x => x.Created).HasColumnName("Created");
            builder.Property(x => x.Modified).HasColumnName("Modified");
            builder.Property(x => x.Status).HasColumnName("Status");

            builder.HasOne<Models.Branch>(x => x.Branch).WithMany().HasForeignKey(t => t.BranchId);

        }
    }
}
