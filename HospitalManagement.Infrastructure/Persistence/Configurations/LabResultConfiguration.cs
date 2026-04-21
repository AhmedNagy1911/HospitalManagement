using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class LabResultConfiguration : IEntityTypeConfiguration<LabResult>
{
    public void Configure(EntityTypeBuilder<LabResult> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.TestName).IsRequired().HasMaxLength(200);
        builder.Property(l => l.Result).IsRequired().HasMaxLength(500);
        builder.Property(l => l.NormalRange).HasMaxLength(100);
        builder.Property(l => l.Unit).HasMaxLength(50);
        builder.Property(l => l.IsNormal).IsRequired();
    }
}