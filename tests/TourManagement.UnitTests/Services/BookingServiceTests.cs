using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

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
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Goa Tour", Place = "Goa", Email = "user@test.com", FirstName = "John" },
            new Booking { BookingId = 2, TourName = "Kashmir Tour", Place = "Kashmir", Email = "user2@test.com", FirstName = "Jane" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByUserEmailAsync_ShouldReturnUserBookings()
    {
        var email = "user@test.com";
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Goa Tour", Place = "Goa", Email = email, FirstName = "John" }
        };
        _mockRepository.Setup(r => r.GetByUserEmailAsync(email, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var result = await _service.GetByUserEmailAsync(email);

        result.Should().HaveCount(1);
        result.First().Email.Should().Be(email);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateAndReturnBooking()
    {
        var booking = new Booking { TourName = "Goa Tour", Place = "Goa", Email = "user@test.com", FirstName = "John" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(booking);

        result.Should().NotBeNull();
        result.TourName.Should().Be("Goa Tour");
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenBookingNotFound_ShouldThrowNotFoundException()
    {
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        await _service.Invoking(s => s.DeleteAsync(999)).Should().ThrowAsync<NotFoundException>();
    }
}
