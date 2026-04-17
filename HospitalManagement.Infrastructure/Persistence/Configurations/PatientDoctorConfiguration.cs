using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class PatientDoctorConfiguration : IEntityTypeConfiguration<PatientDoctor>
{
    public void Configure(EntityTypeBuilder<PatientDoctor> builder)
    {
        // Composite PK — no duplicate assignments
        builder.HasKey(pd => new { pd.PatientId, pd.DoctorId });

        builder.HasOne(pd => pd.Patient)
            .WithMany(p => p.PatientDoctors)
            .HasForeignKey(pd => pd.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict so deleting a Doctor doesn't cascade-delete the patient
        builder.HasOne(pd => pd.Doctor)
            .WithMany()
            .HasForeignKey(pd => pd.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(pd => pd.AssignedAt).IsRequired();
    }
}
