using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core entity configuration for the UserInfo entity.
/// </summary>
public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    /// <summary>Configures the UserInfo entity mapping.</summary>
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("UserInfo");

        builder.HasKey(u => u.Email);

        builder.Property(u => u.Email)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Gender)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(u => u.Password)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.Dob)
            .HasColumnName("dob")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(u => u.Street)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.City)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.State)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        // Relationship: UserInfo has many Bookings
        builder.HasMany(u => u.Bookings)
            .WithOne()
            .HasForeignKey(b => b.Email)
            .HasPrincipalKey(u => u.Email)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
