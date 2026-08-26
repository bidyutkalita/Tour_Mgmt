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
        var tour = new Tour { TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000, Locations = "Goa Beach", TourInfo = "Beautiful beaches" };

        await _repository.AddAsync(tour);

        var savedTour = await _context.Tours.FirstOrDefaultAsync(t => t.TourName == "Goa Tour");
        savedTour.Should().NotBeNull();
        savedTour!.Place.Should().Be("Goa");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        _context.Tours.AddRange(
            new Tour { TourName = "Tour 1", Place = "Place 1", Days = 3, Price = 10000, Locations = "Loc 1", TourInfo = "Info 1" },
            new Tour { TourName = "Tour 2", Place = "Place 2", Days = 5, Price = 20000, Locations = "Loc 2", TourInfo = "Info 2" }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ShouldReturnTour()
    {
        var tour = new Tour { TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 25000, Locations = "Dal Lake", TourInfo = "Paradise" };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(tour.TourId);

        result.Should().NotBeNull();
        result!.TourName.Should().Be("Kashmir Tour");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTourFromDatabase()
    {
        var tour = new Tour { TourName = "Delete Tour", Place = "Place", Days = 3, Price = 5000, Locations = "Loc", TourInfo = "Info" };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        await _repository.DeleteAsync(tour.TourId);

        var deletedTour = await _context.Tours.FindAsync(tour.TourId);
        deletedTour.Should().BeNull();
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingTours()
    {
        _context.Tours.AddRange(
            new Tour { TourName = "Goa Beach Tour", Place = "Goa", Days = 5, Price = 15000, Locations = "Goa Beach", TourInfo = "Beaches" },
            new Tour { TourName = "Kashmir Valley", Place = "Kashmir", Days = 7, Price = 25000, Locations = "Dal Lake", TourInfo = "Mountains" }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.SearchAsync("goa");

        result.Should().HaveCount(1);
        result.First().TourName.Should().Contain("Goa");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
