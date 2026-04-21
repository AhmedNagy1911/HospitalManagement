using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class GeneralReportDetailConfiguration : IEntityTypeConfiguration<GeneralReportDetail>
{
    public void Configure(EntityTypeBuilder<GeneralReportDetail> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Diagnosis).IsRequired().HasMaxLength(1000);
        builder.Property(g => g.Treatment).IsRequired().HasMaxLength(1000);
        builder.Property(g => g.Recommendations).IsRequired().HasMaxLength(2000);
        builder.Property(g => g.FollowUpInstructions).HasMaxLength(1000);
    }
}
