using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(200);
        builder.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
        builder.Property(e => e.NationalId).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Department).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Salary).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(e => e.HireDate).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();

        builder.Property(e => e.StaffType)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasIndex(e => e.Email).IsUnique();
        builder.HasIndex(e => e.NationalId).IsUnique();
        builder.HasIndex(e => new { e.StaffType, e.Status });

        builder.HasMany(e => e.Shifts)
            .WithOne(s => s.Employee)
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

