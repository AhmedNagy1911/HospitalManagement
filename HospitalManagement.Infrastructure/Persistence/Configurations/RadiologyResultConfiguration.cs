using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class RadiologyResultConfiguration : IEntityTypeConfiguration<RadiologyResult>
{
    public void Configure(EntityTypeBuilder<RadiologyResult> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.BodyPart).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Findings).IsRequired().HasMaxLength(2000);
        builder.Property(r => r.Impression).IsRequired().HasMaxLength(1000);
        builder.Property(r => r.ImageUrl).HasMaxLength(500);
        builder.Property(r => r.RadiologyType)
            .HasConversion<string>().HasMaxLength(20).IsRequired();
    }
}