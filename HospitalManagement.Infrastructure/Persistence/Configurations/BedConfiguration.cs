using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class BedConfiguration : IEntityTypeConfiguration<Bed>
{
    public void Configure(EntityTypeBuilder<Bed> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.BedNumber).IsRequired().HasMaxLength(20);

        builder.Property(b => b.Status)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        // Unique bed number per room
        builder.HasIndex(b => new { b.RoomId, b.BedNumber }).IsUnique();

        // الـ Patient FK — nullable لما السرير فاضي
        builder.HasOne(b => b.Patient)
            .WithMany()
            .HasForeignKey(b => b.PatientId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}