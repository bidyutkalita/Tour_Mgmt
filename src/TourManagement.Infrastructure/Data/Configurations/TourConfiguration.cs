using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the Tour entity.
/// </summary>
public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("tour");

        builder.HasKey(t => t.TourId);

        builder.Property(t => t.TourId)
            .HasColumnName("tour_id")
            .UseIdentityAlwaysColumn();

        builder.Property(t => t.TourName)
            .IsRequired()
            .HasColumnName("tour_name")
            .HasMaxLength(20);

        builder.Property(t => t.Place)
            .IsRequired()
            .HasColumnName("place")
            .HasMaxLength(20);

        builder.Property(t => t.Days)
            .IsRequired()
            .HasColumnName("days")
            .HasColumnType("numeric(2,0)");

        builder.Property(t => t.Price)
            .IsRequired()
            .HasColumnName("price")
            .HasColumnType("numeric(6,0)");

        builder.Property(t => t.Locations)
            .IsRequired()
            .HasColumnName("locations")
            .HasMaxLength(100);

        builder.Property(t => t.TourInfo)
            .IsRequired()
            .HasColumnName("tour_info")
            .HasMaxLength(200);

        builder.Property(t => t.Pic)
            .HasColumnName("pic")
            .HasMaxLength(200);
    }
}
