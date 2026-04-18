using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Reason).IsRequired().HasMaxLength(500);
        builder.Property(a => a.Notes).HasMaxLength(1000);
        builder.Property(a => a.CancelReason).HasMaxLength(500);
        builder.Property(a => a.DurationInMinutes).IsRequired();
        builder.Property(a => a.AppointmentDate).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();

        // نخزن الـ enum كـ string عشان يكون readable في الـ DB
        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        // Index للبحث السريع
        builder.HasIndex(a => a.AppointmentDate);
        builder.HasIndex(a => new { a.DoctorId, a.AppointmentDate });

        builder.HasOne(a => a.Patient)
            .WithMany()
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Doctor)
            .WithMany()
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
