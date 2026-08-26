using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core entity configuration for Tour.
/// </summary>
public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("tour");

        builder.HasKey(t => t.TourId);

        builder.Property(t => t.TourId)
            .HasColumnName("tour_id")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.TourName)
            .HasColumnName("tour_name")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Place)
            .HasColumnName("place")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Days)
            .HasColumnName("days")
            .IsRequired();

        builder.Property(t => t.Price)
            .HasColumnName("price")
            .IsRequired()
            .HasColumnType("numeric(9,2)");

        builder.Property(t => t.Locations)
            .HasColumnName("locations")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(t => t.TourInfo)
            .HasColumnName("tour_info")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Pic)
            .HasColumnName("pic")
            .HasMaxLength(200);

        builder.HasMany(t => t.Bookings)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
