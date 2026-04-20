using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(i => i.Notes).HasMaxLength(1000);
        builder.Property(i => i.IssuedAt).IsRequired();

        builder.Property(i => i.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Discount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Status)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(i => i.PaymentMethod)
            .HasConversion<string>().HasMaxLength(20);

        // Unique — appointment واحد بتكون ليه invoice واحدة بس
        builder.HasIndex(i => i.AppointmentId).IsUnique();
        builder.HasIndex(i => i.InvoiceNumber).IsUnique();
        builder.HasIndex(i => new { i.PatientId, i.Status });

        builder.HasOne(i => i.Appointment)
            .WithMany()
            .HasForeignKey(i => i.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Patient)
            .WithMany()
            .HasForeignKey(i => i.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Doctor)
            .WithMany()
            .HasForeignKey(i => i.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
