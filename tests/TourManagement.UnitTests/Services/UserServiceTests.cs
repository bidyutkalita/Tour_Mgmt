using Xunit;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.UnitTests.Services;

/// <summary>
/// Unit tests for UserService.
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    /// <summary>Initializes test dependencies.</summary>
    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new UserService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllUsers()
    {
        // Arrange
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "user1@test.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "hash", Dob = DateTime.Now.AddYears(-25), Street = "123 St", City = "Mumbai", State = "MH" },
            new UserInfo { Email = "user2@test.com", FirstName = "Jane", LastName = "Smith", Gender = "Female", Password = "hash", Dob = DateTime.Now.AddYears(-30), Street = "456 Ave", City = "Delhi", State = "DL" }
        };
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateEmail_ShouldThrowDuplicateEntityException()
    {
        // Arrange
        var dto = new UserCreateDto
        {
            Email = "existing@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Dob = DateTime.Now.AddYears(-25),
            Street = "123 St",
            City = "Mumbai",
            State = "MH"
        };
        _mockRepository.Setup(r => r.ExistsAsync("existing@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => _service.CreateAsync(dto));
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new UserInfo
        {
            Email = "user@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = hashedPassword,
            Dob = DateTime.Now.AddYears(-25),
            Street = "123 St",
            City = "Mumbai",
            State = "MH",
            IsActive = true
        };
        _mockRepository.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.ValidateLoginAsync("user@test.com", "password123");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("user@test.com");
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("correctpassword");
        var user = new UserInfo
        {
            Email = "user@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = hashedPassword,
            Dob = DateTime.Now.AddYears(-25),
            Street = "123 St",
            City = "Mumbai",
            State = "MH",
            IsActive = true
        };
        _mockRepository.Setup(r => r.GetByEmailAsync("user@test.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _service.ValidateLoginAsync("user@test.com", "wrongpassword");

        // Assert
        result.Should().BeNull();
    }
}
