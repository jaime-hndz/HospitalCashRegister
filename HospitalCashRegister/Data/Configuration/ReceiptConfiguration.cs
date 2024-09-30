using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalCashRegister.Data.Configuration
{
    public class ReceiptConfiguration : IEntityTypeConfiguration<Models.Receipt>
    {
        public void Configure(EntityTypeBuilder<Models.Receipt> builder)
        {
            builder.ToTable(nameof(Models.Receipt));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("_id");
            builder.Property(x => x.TransactionId).HasColumnName("TransactionId");
            builder.Property(x => x.GenerationDate).HasColumnName("GenerationDate");
            builder.Property(x => x.Details).HasColumnName("Details");
            builder.HasOne<Models.Transaction>(x => x.Transaction).WithMany().HasForeignKey(t => t.TransactionId);



        }
    }
}
