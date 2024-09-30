using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalCashRegister.Data.Configuration
{
    public class MedicalServiceConfiguration : IEntityTypeConfiguration<Models.MedicalService>
    {
        public void Configure(EntityTypeBuilder<Models.MedicalService> builder)
        {
            builder.ToTable(nameof(Models.MedicalService));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("_id");
            builder.Property(x => x.Name).HasColumnName("Name");
            builder.Property(x => x.Description).HasColumnName("Description");
            builder.Property(x => x.Price).HasColumnName("Price");
            builder.Property(x => x.Status).HasColumnName("Status");


        }
    }
}
