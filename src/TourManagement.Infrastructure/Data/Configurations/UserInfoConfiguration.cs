using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core configuration for the UserInfo entity.
/// </summary>
public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("user_info");

        builder.HasKey(u => u.Email);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasColumnName("email")
            .HasMaxLength(50);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasColumnName("first_name")
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasColumnName("last_name")
            .HasMaxLength(50);

        builder.Property(u => u.Gender)
            .IsRequired()
            .HasColumnName("gender")
            .HasMaxLength(10);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasColumnName("password")
            .HasMaxLength(256);

        builder.Property(u => u.Dob)
            .IsRequired()
            .HasColumnName("dob")
            .HasColumnType("date");

        builder.Property(u => u.Street)
            .IsRequired()
            .HasColumnName("street")
            .HasMaxLength(50);

        builder.Property(u => u.City)
            .IsRequired()
            .HasColumnName("city")
            .HasMaxLength(50);

        builder.Property(u => u.State)
            .IsRequired()
            .HasColumnName("state")
            .HasMaxLength(50);

        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.Email)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
