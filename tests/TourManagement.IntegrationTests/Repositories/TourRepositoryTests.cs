using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

namespace TourManagement.IntegrationTests.Repositories;

/// <summary>
/// Integration tests for TourRepository using in-memory database.
/// </summary>
public class TourRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly TourRepository _repository;

    public TourRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        var mockLogger = new Mock<ILogger<TourRepository>>();
        _repository = new TourRepository(_context, mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTourToDatabase()
    {
        // Arrange
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 10000,
            Locations = "Test Locations",
            TourInfo = "Test Info",
            IsActive = true
        };

        // Act
        var result = await _repository.AddAsync(tour);

        // Assert
        result.TourId.Should().BeGreaterThan(0);
        var dbTour = await _context.Tours.FindAsync(result.TourId);
        dbTour.Should().NotBeNull();
        dbTour!.TourName.Should().Be("Test Tour");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveTours()
    {
        // Arrange
        _context.Tours.AddRange(
            new Tour { TourName = "Active Tour", Place = "Place1", Days = 3, Price = 5000, Locations = "L1", TourInfo = "Info1", IsActive = true },
            new Tour { TourName = "Inactive Tour", Place = "Place2", Days = 4, Price = 6000, Locations = "L2", TourInfo = "Info2", IsActive = false }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().TourName.Should().Be("Active Tour");
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteTour()
    {
        // Arrange
        var tour = new Tour { TourName = "Tour to Delete", Place = "Place", Days = 2, Price = 3000, Locations = "L", TourInfo = "Info", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(tour.TourId);

        // Assert
        var dbTour = await _context.Tours.FindAsync(tour.TourId);
        dbTour.Should().NotBeNull();
        dbTour!.IsActive.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
