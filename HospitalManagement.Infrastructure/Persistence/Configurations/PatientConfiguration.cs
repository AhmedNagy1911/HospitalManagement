using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.LastName).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(200);
        builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Gender).IsRequired().HasMaxLength(10);
        builder.Property(p => p.Address).HasMaxLength(500);
        builder.Property(p => p.BloodType).HasMaxLength(5);
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();

        // Unique email index
        builder.HasIndex(p => p.Email).IsUnique();

        // Cascade delete medical histories when patient is hard-deleted
        builder.HasMany(p => p.MedicalHistories)
            .WithOne(mh => mh.Patient)
            .HasForeignKey(mh => mh.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}