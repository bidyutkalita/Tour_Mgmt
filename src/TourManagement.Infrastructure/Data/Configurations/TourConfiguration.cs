using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core entity configuration for the Tour entity.
/// </summary>
public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    /// <summary>Configures the Tour entity mapping.</summary>
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tour");

        builder.HasKey(t => t.TourId);

        builder.Property(t => t.TourId)
            .HasColumnName("TOUR_ID")
            .UseIdentityColumn();

        builder.Property(t => t.TourName)
            .HasColumnName("TOUR_NAME")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Place)
            .HasColumnName("PLACE")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Days)
            .HasColumnName("DAYS")
            .IsRequired();

        builder.Property(t => t.Price)
            .HasColumnName("PRICE")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(t => t.Locations)
            .HasColumnName("LOCATIONS")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.TourInfo)
            .HasColumnName("TOUR_INFO")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(t => t.Pic)
            .HasColumnName("pic")
            .HasMaxLength(500);

        builder.Property(t => t.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        // Relationship: Tour has many Bookings
        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Tour)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
