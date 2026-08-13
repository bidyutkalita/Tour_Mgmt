using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

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

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<UserService>>();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _service = new UserService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var createDto = new UserCreateDto { Email = "existing@test.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "pass123", Dob = DateTime.Now.AddYears(-25), Street = "123 Main St", City = "Mumbai", State = "Maharashtra" };
        _mockRepository.Setup(r => r.ExistsAsync("existing@test.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act & Assert
        await _service.Invoking(s => s.CreateAsync(createDto))
            .Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ValidateLoginAsync_WithValidCredentials_ShouldReturnUser()
    {
        // Arrange
        var user = new UserInfo { Email = "user@test.com", FirstName = "John", LastName = "Doe", Gender = "Male", Password = "pass123", Dob = DateTime.Now.AddYears(-25), Street = "123 Main St", City = "Mumbai", State = "Maharashtra" };
        var loginDto = new UserLoginDto { Email = "user@test.com", Password = "pass123" };
        _mockRepository.Setup(r => r.ValidateCredentialsAsync("user@test.com", "pass123", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        // Act
        var result = await _service.ValidateLoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("user@test.com");
    }

    [Fact]
    public async Task ValidateLoginAsync_WithInvalidCredentials_ShouldReturnNull()
    {
        // Arrange
        var loginDto = new UserLoginDto { Email = "user@test.com", Password = "wrongpass" };
        _mockRepository.Setup(r => r.ValidateCredentialsAsync("user@test.com", "wrongpass", It.IsAny<CancellationToken>())).ReturnsAsync((UserInfo?)null);

        // Act
        var result = await _service.ValidateLoginAsync(loginDto);

        // Assert
        result.Should().BeNull();
    }
}
