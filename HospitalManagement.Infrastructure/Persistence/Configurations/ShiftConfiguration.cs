using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ShiftDate).IsRequired();
        builder.Property(s => s.Notes).HasMaxLength(500);
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.Property(s => s.ShiftType)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        // Unique: نفس الموظف مش ينفع عنده شيفتين نفس النوع في نفس اليوم
        builder.HasIndex(s => new { s.EmployeeId, s.ShiftDate, s.ShiftType }).IsUnique();
    }
}