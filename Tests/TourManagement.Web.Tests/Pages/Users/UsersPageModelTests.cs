using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Web.Pages.Users;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.Pages.Users;

/// <summary>
/// Unit tests for Users/Login page model.
/// </summary>
public class UsersLoginModelTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _mockServiceLogger;
    private readonly Mock<ILogger<LoginModel>> _mockPageLogger;
    private readonly UserService _userService;

    public UsersLoginModelTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockServiceLogger = new Mock<ILogger<UserService>>();
        _mockPageLogger = new Mock<ILogger<LoginModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _userService = new UserService(_mockUserRepo.Object, _mapper, _mockServiceLogger.Object);
    }

    private LoginModel CreatePageModel(string? sessionEmail = null)
    {
        var model = new LoginModel(_userService, _mockPageLogger.Object);
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        if (sessionEmail != null) session.SetString("UserEmail", sessionEmail);
        httpContext.Session = session;
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public void OnGet_WhenAlreadyLoggedIn_RedirectsToProfile()
    {
        var model = CreatePageModel(sessionEmail: "user@example.com");

        var result = model.OnGet();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Users/Profile", redirect.PageName);
    }

    [Fact]
    public void OnGet_WhenNotLoggedIn_ReturnsPage()
    {
        var model = CreatePageModel();

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModel_ReturnsPage()
    {
        var model = CreatePageModel();
        model.ModelState.AddModelError("Email", "Required");

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidCredentials_SetsErrorAndReturnsPage()
    {
        _mockUserRepo.Setup(r => r.GetByEmailAsync("bad@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        var model = CreatePageModel();
        model.Input = new LoginViewModel { Email = "bad@example.com", Password = "wrong" };

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }

    [Fact]
    public async Task OnPostAsync_WithValidCredentials_SetsSessionAndRedirects()
    {
        var user = new UserInfo
        {
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = BCrypt.Net.BCrypt.HashPassword("password123")
        };
        _mockUserRepo.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var model = CreatePageModel();
        model.Input = new LoginViewModel { Email = "user@example.com", Password = "password123" };

        var result = await model.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Users/Profile", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenExceptionThrown_SetsErrorAndReturnsPage()
    {
        _mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel();
        model.Input = new LoginViewModel { Email = "user@example.com", Password = "pass" };

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }
}

/// <summary>
/// Unit tests for Users/Register page model.
/// </summary>
public class UsersRegisterModelTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _mockServiceLogger;
    private readonly Mock<ILogger<RegisterModel>> _mockPageLogger;
    private readonly UserService _userService;

    public UsersRegisterModelTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockServiceLogger = new Mock<ILogger<UserService>>();
        _mockPageLogger = new Mock<ILogger<RegisterModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _userService = new UserService(_mockUserRepo.Object, _mapper, _mockServiceLogger.Object);
    }

    private RegisterModel CreatePageModel(string? sessionEmail = null)
    {
        var model = new RegisterModel(_userService, _mockPageLogger.Object);
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        if (sessionEmail != null) session.SetString("UserEmail", sessionEmail);
        httpContext.Session = session;
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public void OnGet_WhenAlreadyLoggedIn_RedirectsToProfile()
    {
        var model = CreatePageModel(sessionEmail: "user@example.com");

        var result = model.OnGet();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Users/Profile", redirect.PageName);
    }

    [Fact]
    public void OnGet_WhenNotLoggedIn_ReturnsPage()
    {
        var model = CreatePageModel();

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModel_ReturnsPage()
    {
        var model = CreatePageModel();
        model.ModelState.AddModelError("Email", "Required");

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WithValidData_CreatesUserAndRedirects()
    {
        _mockUserRepo.Setup(r => r.ExistsAsync("new@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<UserInfo>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var model = CreatePageModel();
        model.Input = new RegisterViewModel
        {
            Email = "new@example.com",
            FirstName = "New",
            LastName = "User",
            Gender = "Male",
            Password = "password123",
            Dob = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "Springfield",
            State = "IL"
        };

        var result = await model.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Login", redirect.PageName);
        Assert.True(model.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public async Task OnPostAsync_WhenDuplicateEmail_AddsModelError()
    {
        _mockUserRepo.Setup(r => r.ExistsAsync("dup@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var model = CreatePageModel();
        model.Input = new RegisterViewModel
        {
            Email = "dup@example.com",
            FirstName = "Dup",
            LastName = "User",
            Gender = "Male",
            Password = "password123",
            Dob = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "Springfield",
            State = "IL"
        };

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.False(model.ModelState.IsValid);
    }

    [Fact]
    public async Task OnPostAsync_WhenExceptionThrown_SetsErrorAndReturnsPage()
    {
        _mockUserRepo.Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel();
        model.Input = new RegisterViewModel
        {
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            Dob = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "Springfield",
            State = "IL"
        };

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }
}

/// <summary>
/// Unit tests for Users/Logout page model.
/// </summary>
public class UsersLogoutModelTests
{
    private readonly Mock<ILogger<LogoutModel>> _mockLogger;

    public UsersLogoutModelTests()
    {
        _mockLogger = new Mock<ILogger<LogoutModel>>();
    }

    private LogoutModel CreatePageModel(string? sessionEmail = null)
    {
        var model = new LogoutModel(_mockLogger.Object);
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        if (sessionEmail != null) session.SetString("UserEmail", sessionEmail);
        httpContext.Session = session;
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public void OnGet_ClearsSessionAndRedirects()
    {
        var model = CreatePageModel(sessionEmail: "user@example.com");

        var result = model.OnGet();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
        Assert.True(model.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public void OnGet_WhenNoSession_StillRedirects()
    {
        var model = CreatePageModel();

        var result = model.OnGet();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }
}

/// <summary>
/// Unit tests for Users/Profile page model.
/// </summary>
public class UsersProfileModelTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _mockServiceLogger;
    private readonly Mock<ILogger<ProfileModel>> _mockPageLogger;
    private readonly UserService _userService;

    public UsersProfileModelTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockServiceLogger = new Mock<ILogger<UserService>>();
        _mockPageLogger = new Mock<ILogger<ProfileModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _userService = new UserService(_mockUserRepo.Object, _mapper, _mockServiceLogger.Object);
    }

    private ProfileModel CreatePageModel(string? sessionEmail = null)
    {
        var model = new ProfileModel(_userService, _mockPageLogger.Object);
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        if (sessionEmail != null) session.SetString("UserEmail", sessionEmail);
        httpContext.Session = session;
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var model = CreatePageModel();

        var result = await model.OnGetAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Login", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserNotFound_ClearsSessionAndRedirects()
    {
        _mockUserRepo.Setup(r => r.GetByEmailAsync("ghost@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserInfo?)null);

        var model = CreatePageModel(sessionEmail: "ghost@example.com");

        var result = await model.OnGetAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Login", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenUserExists_ReturnsPageWithProfile()
    {
        var user = new UserInfo
        {
            Email = "user@example.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Dob = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "Springfield",
            State = "IL"
        };
        _mockUserRepo.Setup(r => r.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var model = CreatePageModel(sessionEmail: "user@example.com");

        var result = await model.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.Profile);
        Assert.Equal("John", model.Profile!.FirstName);
        Assert.Equal("Doe", model.Profile.LastName);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionThrown_SetsErrorAndReturnsPage()
    {
        _mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel(sessionEmail: "user@example.com");

        var result = await model.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }
}

/// <summary>
/// Shared mock session for Users page tests.
/// </summary>
internal class MockSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();

    public bool IsAvailable => true;
    public string Id => Guid.NewGuid().ToString();
    public IEnumerable<string> Keys => _store.Keys;

    public void Clear() => _store.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _store.Remove(key);
    public void Set(string key, byte[] value) => _store[key] = value;
    public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
}
