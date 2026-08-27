using Xunit;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.UnitTests.Services;

/// <summary>
/// Unit tests for TourService.
/// </summary>
public class TourServiceTests
{
    private readonly Mock<ITourRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TourService>> _mockLogger;
    private readonly TourService _service;

    /// <summary>Initializes test dependencies.</summary>
    public TourServiceTests()
    {
        _mockRepository = new Mock<ITourRepository>();
        _mockLogger = new Mock<ILogger<TourService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new TourService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000, Locations = "Goa", TourInfo = "Beach tour", IsActive = true },
            new Tour { TourId = 2, TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 25000, Locations = "Kashmir", TourInfo = "Mountain tour", IsActive = true }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().TourName.Should().Be("Goa Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000, Locations = "Goa", TourInfo = "Beach tour" };
        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tour);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.TourId.Should().Be(1);
        result.TourName.Should().Be("Goa Tour");
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tour?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldCreateTour()
    {
        // Arrange
        var dto = new TourCreateDto
        {
            TourName = "New Tour",
            Place = "Mumbai",
            Days = 3,
            Price = 10000,
            Locations = "Mumbai",
            TourInfo = "City tour"
        };
        var createdTour = new Tour
        {
            TourId = 1,
            TourName = dto.TourName,
            Place = dto.Place,
            Days = dto.Days,
            Price = dto.Price,
            Locations = dto.Locations,
            TourInfo = dto.TourInfo,
            IsActive = true
        };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTour);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.TourName.Should().Be("New Tour");
        result.TourId.Should().Be(1);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithValidTerm_ShouldReturnMatchingTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Goa Beach Tour", Place = "Goa", Days = 5, Price = 15000, Locations = "Goa", TourInfo = "Beach tour" }
        };
        _mockRepository.Setup(r => r.SearchAsync("Goa", It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);

        // Act
        var result = await _service.SearchAsync("Goa");

        // Assert
        result.Should().HaveCount(1);
        result.First().TourName.Should().Contain("Goa");
    }
}
