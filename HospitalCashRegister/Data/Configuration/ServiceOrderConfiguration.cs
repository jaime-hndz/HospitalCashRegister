using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalCashRegister.Data.Configuration
{
    public class ServiceOrderConfiguration : IEntityTypeConfiguration<Models.ServiceOrder>
    {
        public void Configure(EntityTypeBuilder<Models.ServiceOrder> builder)
        {
            builder.ToTable(nameof(Models.ServiceOrder));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("_id");
            builder.Property(x => x.TransactionId).HasColumnName("TransactionId");
            builder.Property(x => x.ServiceId).HasColumnName("ServiceId");

            builder.HasOne(x => x.MedicalService).WithMany().HasForeignKey(x => x.ServiceId);

        }
    }
}
