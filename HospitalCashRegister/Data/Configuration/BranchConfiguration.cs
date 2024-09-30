using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalCashRegister.Data.Configuration
{
    public class BranchConfiguration : IEntityTypeConfiguration<Models.Branch>
    {
        public void Configure(EntityTypeBuilder<Models.Branch> builder)
        {
            builder.ToTable(nameof(Models.Branch));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("_id");
            builder.Property(x => x.Name).HasColumnName("Name");
            builder.Property(x => x.Created).HasColumnName("Created");
            builder.Property(x => x.Modified).HasColumnName("Modified");
            builder.Property(x => x.Status).HasColumnName("Status");


        }
    }
}
