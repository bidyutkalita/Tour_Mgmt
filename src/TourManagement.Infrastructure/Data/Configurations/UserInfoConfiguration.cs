using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core entity configuration for UserInfo.
/// </summary>
public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    /// <inheritdoc/>
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("user_info");

        builder.HasKey(u => u.Email);

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .HasColumnName("last_name")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Gender)
            .HasColumnName("gender")
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(u => u.Password)
            .HasColumnName("password")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Dob)
            .HasColumnName("dob")
            .IsRequired()
            .HasColumnType("date");

        builder.Property(u => u.Street)
            .HasColumnName("street")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.City)
            .HasColumnName("city")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.State)
            .HasColumnName("state")
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.Email)
            .HasPrincipalKey(u => u.Email)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
