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
/// Unit tests for BookingService.
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new BookingService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Goa Tour", Email = "user@test.com", FirstName = "John" },
            new Booking { BookingId = 2, TourName = "Kashmir Tour", Email = "user2@test.com", FirstName = "Jane" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUserBookings()
    {
        // Arrange
        var email = "user@test.com";
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Goa Tour", Email = email, FirstName = "John" }
        };
        _mockRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetByEmailAsync(email);

        // Assert
        result.Should().HaveCount(1);
        result.First().Email.Should().Be(email);
    }

    [Fact]
    public async Task CreateAsync_WithValidDto_ShouldReturnCreatedBooking()
    {
        // Arrange
        var dto = new BookingCreateDto
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "user@test.com",
            FirstName = "John"
        };
        var created = new Booking
        {
            BookingId = 1,
            TourName = dto.TourName,
            Place = dto.Place,
            Email = dto.Email,
            FirstName = dto.FirstName,
            BookingDate = DateTime.UtcNow
        };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.BookingId.Should().Be(1);
        result.TourName.Should().Be("Goa Tour");
    }
}
