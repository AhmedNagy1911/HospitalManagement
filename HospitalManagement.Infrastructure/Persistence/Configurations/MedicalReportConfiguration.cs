using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class MedicalReportConfiguration : IEntityTypeConfiguration<MedicalReport>
{
    public void Configure(EntityTypeBuilder<MedicalReport> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Title).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Notes).HasMaxLength(2000);
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.Property(r => r.ReportType)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasIndex(r => new { r.PatientId, r.ReportType, r.Status });
        builder.HasIndex(r => r.AppointmentId);

        builder.HasOne(r => r.Appointment)
            .WithMany()
            .HasForeignKey(r => r.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Patient)
            .WithMany()
            .HasForeignKey(r => r.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Doctor)
            .WithMany()
            .HasForeignKey(r => r.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        // One-to-One relations
        builder.HasOne(r => r.LabResult)
            .WithOne(l => l.MedicalReport)
            .HasForeignKey<LabResult>(l => l.MedicalReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.RadiologyResult)
            .WithOne(rad => rad.MedicalReport)
            .HasForeignKey<RadiologyResult>(rad => rad.MedicalReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.GeneralReportDetail)
            .WithOne(g => g.MedicalReport)
            .HasForeignKey<GeneralReportDetail>(g => g.MedicalReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
