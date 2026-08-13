using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the Booking entity.
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking");

        builder.HasKey(b => b.BookingId);

        builder.Property(b => b.BookingId)
            .HasColumnName("booking_id")
            .UseIdentityAlwaysColumn();

        builder.Property(b => b.TourName)
            .HasColumnName("tour_name")
            .HasMaxLength(50);

        builder.Property(b => b.Place)
            .HasColumnName("place")
            .HasMaxLength(50);

        builder.Property(b => b.Email)
            .HasColumnName("email")
            .HasMaxLength(50);

        builder.Property(b => b.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(50);

        builder.Property(b => b.BookingDate)
            .HasColumnName("booking_date")
            .HasDefaultValueSql("NOW()");

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.Email)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
