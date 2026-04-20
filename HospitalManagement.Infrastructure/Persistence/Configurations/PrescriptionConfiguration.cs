using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
{
    public void Configure(EntityTypeBuilder<Prescription> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Notes).HasMaxLength(2000);
        builder.Property(p => p.IssuedAt).IsRequired();
        builder.Property(p => p.ExpiryDate).IsRequired();

        builder.Property(p => p.Status)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        // Prescription واحدة لكل Appointment
        builder.HasIndex(p => p.AppointmentId).IsUnique();
        builder.HasIndex(p => new { p.PatientId, p.Status });

        builder.HasOne(p => p.Appointment)
            .WithMany()
            .HasForeignKey(p => p.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Patient)
            .WithMany()
            .HasForeignKey(p => p.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Doctor)
            .WithMany()
            .HasForeignKey(p => p.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Medications)
            .WithOne(m => m.Prescription)
            .HasForeignKey(m => m.PrescriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}


// ============================================================
// PrescriptionMedicationConfiguration.cs
// ============================================================
public class PrescriptionMedicationConfiguration : IEntityTypeConfiguration<PrescriptionMedication>
{
    public void Configure(EntityTypeBuilder<PrescriptionMedication> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.MedicationName).IsRequired().HasMaxLength(200);
        builder.Property(m => m.Dosage).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Frequency).IsRequired().HasMaxLength(100);
        builder.Property(m => m.DurationInDays).IsRequired();
        builder.Property(m => m.Instructions).HasMaxLength(500);
    }
}
