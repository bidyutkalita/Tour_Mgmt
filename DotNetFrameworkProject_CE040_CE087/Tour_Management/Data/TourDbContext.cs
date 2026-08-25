using Microsoft.EntityFrameworkCore;
using Tour_Management.Models;

namespace Tour_Management.Data
{
    /// <summary>
    /// Entity Framework Core DbContext for Tour Management application.
    /// Replaces direct SqlConnection usage with EF Core connection pooling,
    /// retry logic, and Azure SQL Database integration with transient fault handling.
    /// </summary>
    public class TourDbContext : DbContext
    {
        public TourDbContext(DbContextOptions<TourDbContext> options) : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<UserInfo> UserInfos { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tour>(entity =>
            {
                entity.ToTable("Tour");
                entity.HasKey(e => e.TourId);
                entity.Property(e => e.TourId).HasColumnName("TOUR_ID");
                entity.Property(e => e.TourName).HasColumnName("TOUR_NAME").IsRequired().HasMaxLength(200);
                entity.Property(e => e.Place).HasColumnName("PLACE").HasMaxLength(200);
                entity.Property(e => e.Days).HasColumnName("DAYS");
                entity.Property(e => e.Price).HasColumnName("PRICE");
                entity.Property(e => e.Locations).HasColumnName("LOCATIONS").HasMaxLength(500);
                entity.Property(e => e.TourInfo).HasColumnName("TOUR_INFO").HasMaxLength(250);
                entity.Property(e => e.Pic).HasColumnName("pic").HasMaxLength(500);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.ToTable("UserInfo");
                entity.HasKey(e => e.Email);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Gender).HasMaxLength(10);
                entity.Property(e => e.Password).HasMaxLength(200);
                entity.Property(e => e.Dob).HasColumnName("dob");
                entity.Property(e => e.Street).HasMaxLength(200);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(100);
            });

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("booking");
                entity.HasKey(e => e.TourId);
                entity.Property(e => e.TourId).HasColumnName("TOUR_ID");
                entity.Property(e => e.TourName).HasColumnName("TOUR_NAME").HasMaxLength(200);
                entity.Property(e => e.Place).HasColumnName("PLACE").HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.FirstName).HasMaxLength(100);
            });
        }
    }
}
