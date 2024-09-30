using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalCashRegister.Data.Configuration
{
    public class PatientConfiguration : IEntityTypeConfiguration<Models.Patient>
    {
        public void Configure(EntityTypeBuilder<Models.Patient> builder)
        {
            builder.ToTable(nameof(Models.Patient));

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("_id");
            builder.Property(x => x.Name).HasColumnName("Name");
            builder.Property(x => x.Document).HasColumnName("Document");
            builder.Property(x => x.PhoneNumber1).HasColumnName("PhoneNumber1");
            builder.Property(x => x.PhoneNumber2).HasColumnName("PhoneNumber2");
            builder.Property(x => x.Address).HasColumnName("Address");
            builder.Property(x => x.Status).HasColumnName("Status");


        }
    }
}
