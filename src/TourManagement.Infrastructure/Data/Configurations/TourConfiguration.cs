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
        builder.ToTable("tour");

        builder.HasKey(t => t.TourId);

        builder.Property(t => t.TourId)
            .HasColumnName("tour_id")
            .UseIdentityAlwaysColumn();

        builder.Property(t => t.TourName)
            .HasColumnName("tour_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Place)
            .HasColumnName("place")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.Days)
            .HasColumnName("days")
            .IsRequired();

        builder.Property(t => t.Price)
            .HasColumnName("price")
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(t => t.Locations)
            .HasColumnName("locations")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.TourInfo)
            .HasColumnName("tour_info")
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(t => t.Pic)
            .HasColumnName("pic")
            .HasMaxLength(500);

        builder.Property(t => t.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.Property(t => t.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        // Relationship: Tour has many Bookings
        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Tour)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
