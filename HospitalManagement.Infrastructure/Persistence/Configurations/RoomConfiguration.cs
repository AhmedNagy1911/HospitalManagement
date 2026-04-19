using HospitalManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HospitalManagement.Infrastructure.Persistence.Configurations;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RoomNumber).IsRequired().HasMaxLength(20);
        builder.Property(r => r.Description).HasMaxLength(500);
        builder.Property(r => r.Floor).IsRequired();
        builder.Property(r => r.CreatedAt).IsRequired();

        builder.Property(r => r.Type)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.HasIndex(r => r.RoomNumber).IsUnique();
        builder.HasIndex(r => new { r.Floor, r.Status });

        builder.HasMany(r => r.Beds)
            .WithOne(b => b.Room)
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
