﻿using Microsoft.EntityFrameworkCore;
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
            builder.Property(x => x.ServiceId).HasColumnName("ServiceId");
            builder.Property(x => x.CashRegisterId).HasColumnName("ServiceId");
            builder.Property(x => x.TransactionTypeId).HasColumnName("TransactionTypeId");
            builder.Property(x => x.TransactionStatusId).HasColumnName("TransactionStatusId");

            builder.Property(x => x.Amount).HasColumnName("Amount");
            builder.Property(x => x.Date).HasColumnName("Date");

            builder.HasOne<Models.Cashier>(x => x.Cashier).WithMany().HasForeignKey(t => t.CashierId);
            builder.HasOne<Models.Patient>(x => x.Patient).WithMany().HasForeignKey(t => t.PatientId);
            builder.HasOne<Models.MedicalService>(x => x.MedicalService).WithMany().HasForeignKey(t => t.ServiceId);



        }
    }
}
