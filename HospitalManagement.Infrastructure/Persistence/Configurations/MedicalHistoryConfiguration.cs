using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class MedicalHistoryConfiguration : IEntityTypeConfiguration<MedicalHistory>
{
    public void Configure(EntityTypeBuilder<MedicalHistory> builder)
    {
        builder.HasKey(mh => mh.Id);

        builder.Property(mh => mh.Diagnosis).IsRequired().HasMaxLength(500);
        builder.Property(mh => mh.Treatment).IsRequired().HasMaxLength(1000);
        builder.Property(mh => mh.Notes).HasMaxLength(2000);
        builder.Property(mh => mh.RecordedAt).IsRequired();
    }
}