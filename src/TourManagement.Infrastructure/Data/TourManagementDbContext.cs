using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data;

/// <summary>
/// Entity Framework Core database context for the Tour Management application.
/// </summary>
public class TourManagementDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="TourManagementDbContext"/>.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options)
        : base(options)
    {
    }

    /// <summary>Gets or sets the UserInfo DbSet.</summary>
    public DbSet<UserInfo> UserInfos { get; set; } = null!;

    /// <summary>Gets or sets the Tour DbSet.</summary>
    public DbSet<Tour> Tours { get; set; } = null!;

    /// <summary>Gets or sets the Booking DbSet.</summary>
    public DbSet<Booking> Bookings { get; set; } = null!;

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema to "public" for PostgreSQL
        modelBuilder.HasDefaultSchema("public");

        // Enable PostgreSQL extensions
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TourManagementDbContext).Assembly);
    }
}
