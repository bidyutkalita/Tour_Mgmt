using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core entity configuration for the Booking entity.
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    /// <summary>Configures the Booking entity mapping.</summary>
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking");

        builder.HasKey(b => b.BookingId);

        builder.Property(b => b.BookingId)
            .HasColumnName("booking_id")
            .UseIdentityAlwaysColumn();

        builder.Property(b => b.TourName)
            .HasColumnName("tour_name")
            .HasMaxLength(100);

        builder.Property(b => b.Place)
            .HasColumnName("place")
            .HasMaxLength(100);

        builder.Property(b => b.Email)
            .HasColumnName("email")
            .HasMaxLength(100);

        builder.Property(b => b.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(100);

        builder.Property(b => b.CreatedDate)
            .HasColumnName("created_date")
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");

        builder.Property(b => b.IsActive)
            .HasColumnName("is_active")
            .HasDefaultValue(true);

        builder.Property(b => b.TourId)
            .HasColumnName("tour_id")
            .IsRequired(false);
    }
}
