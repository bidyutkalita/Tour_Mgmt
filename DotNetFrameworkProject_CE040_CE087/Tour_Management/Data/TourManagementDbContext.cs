using Microsoft.EntityFrameworkCore;
using Tour_Management.Models;

namespace Tour_Management.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for Tour Management application.
    /// Replaces direct SqlConnection usage with EF Core connection pooling,
    /// built-in retry logic, and Azure SQL Database integration with
    /// transient fault handling (cr-dotnet-0013).
    /// </summary>
    public class TourManagementDbContext : DbContext
    {
        public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<UserInfo> UserInfos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.ToTable("Tour");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TourName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Place).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("booking");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TourName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Place).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("UserInfo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(200);
            });
        }
    }
}
