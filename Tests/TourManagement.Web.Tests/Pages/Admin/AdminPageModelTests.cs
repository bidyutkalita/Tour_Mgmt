using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Web.Pages.Admin;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.Pages.Admin;

/// <summary>
/// Unit tests for Admin/Login page model.
/// </summary>
public class AdminLoginModelTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<ILogger<LoginModel>> _mockLogger;

    public AdminLoginModelTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<LoginModel>>();
    }

    private LoginModel CreatePageModel(bool isAdmin = false)
    {
        var model = new LoginModel(_mockConfig.Object, _mockLogger.Object);
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        if (isAdmin) session.SetString("IsAdmin", "true");
        httpContext.Session = session;
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public void OnGet_WhenAlreadyAdmin_RedirectsToDashboard()
    {
        var model = CreatePageModel(isAdmin: true);

        var result = model.OnGet();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Dashboard", redirect.PageName);
    }

    [Fact]
    public void OnGet_WhenNotAdmin_ReturnsPage()
    {
        var model = CreatePageModel();

        var result = model.OnGet();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public void OnPost_WithInvalidModel_ReturnsPage()
    {
        var model = CreatePageModel();
        model.ModelState.AddModelError("Email", "Required");

        var result = model.OnPost();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public void OnPost_WithValidAdminCredentials_SetsSessionAndRedirects()
    {
        _mockConfig.Setup(c => c["AdminCredentials:Email"]).Returns("admin@gmail.com");
        _mockConfig.Setup(c => c["AdminCredentials:Password"]).Returns("admin");

        var model = CreatePageModel();
        model.Input = new LoginViewModel { Email = "admin@gmail.com", Password = "admin" };

        var result = model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Dashboard", redirect.PageName);
    }

    [Fact]
    public void OnPost_WithInvalidAdminCredentials_SetsErrorAndReturnsPage()
    {
        _mockConfig.Setup(c => c["AdminCredentials:Email"]).Returns("admin@gmail.com");
        _mockConfig.Setup(c => c["AdminCredentials:Password"]).Returns("admin");

        var model = CreatePageModel();
        model.Input = new LoginViewModel { Email = "wrong@gmail.com", Password = "wrongpass" };

        var result = model.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }

    [Fact]
    public void OnPost_WithDefaultCredentials_WhenConfigNull_UsesDefaults()
    {
        // When config returns null, defaults are used
        _mockConfig.Setup(c => c["AdminCredentials:Email"]).Returns((string?)null);
        _mockConfig.Setup(c => c["AdminCredentials:Password"]).Returns((string?)null);

        var model = CreatePageModel();
        model.Input = new LoginViewModel { Email = "admin@gmail.com", Password = "admin" };

        var result = model.OnPost();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Dashboard", redirect.PageName);
    }

    [Fact]
    public void OnPost_WithWrongPassword_SetsErrorMessage()
    {
        _mockConfig.Setup(c => c["AdminCredentials:Email"]).Returns("admin@gmail.com");
        _mockConfig.Setup(c => c["AdminCredentials:Password"]).Returns("correctpass");

        var model = CreatePageModel();
        model.Input = new LoginViewModel { Email = "admin@gmail.com", Password = "wrongpass" };

        var result = model.OnPost();

        Assert.IsType<PageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }
}

/// <summary>
/// Unit tests for Admin/Logout page model.
/// </summary>
public class AdminLogoutModelTests
{
    private readonly Mock<ILogger<LogoutModel>> _mockLogger;

    public AdminLogoutModelTests()
    {
        _mockLogger = new Mock<ILogger<LogoutModel>>();
    }

    private LogoutModel CreatePageModel(bool isAdmin = true)
    {
        var model = new LogoutModel(_mockLogger.Object);
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        if (isAdmin) session.SetString("IsAdmin", "true");
        httpContext.Session = session;
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public void OnGet_ClearsAdminSessionAndRedirects()
    {
        var model = CreatePageModel(isAdmin: true);

        var result = model.OnGet();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
        Assert.True(model.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public void OnGet_WhenNotAdmin_StillRedirects()
    {
        var model = CreatePageModel(isAdmin: false);

        var result = model.OnGet();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Index", redirect.PageName);
    }
}

/// <summary>
/// Unit tests for Admin/Dashboard page model.
/// </summary>
public class AdminDashboardModelTests
{
    private readonly Mock<ITourRepository> _mockTourRepo;
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TourService>> _mockTourLogger;
    private readonly Mock<ILogger<UserService>> _mockUserLogger;
    private readonly Mock<ILogger<BookingService>> _mockBookingLogger;
    private readonly Mock<ILogger<DashboardModel>> _mockPageLogger;
    private readonly TourService _tourService;
    private readonly UserService _userService;
    private readonly BookingService _bookingService;

    public AdminDashboardModelTests()
    {
        _mockTourRepo = new Mock<ITourRepository>();
        _mockUserRepo = new Mock<IUserRepository>();
        _mockBookingRepo = new Mock<IBookingRepository>();
        _mockTourLogger = new Mock<ILogger<TourService>>();
        _mockUserLogger = new Mock<ILogger<UserService>>();
        _mockBookingLogger = new Mock<ILogger<BookingService>>();
        _mockPageLogger = new Mock<ILogger<DashboardModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _tourService = new TourService(_mockTourRepo.Object, _mapper, _mockTourLogger.Object);
        _userService = new UserService(_mockUserRepo.Object, _mapper, _mockUserLogger.Object);
        _bookingService = new BookingService(_mockBookingRepo.Object, _mapper, _mockBookingLogger.Object);
    }

    private DashboardModel CreatePageModel(bool isAdmin = true)
    {
        var model = new DashboardModel(_tourService, _userService, _bookingService, _mockPageLogger.Object);
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        if (isAdmin) session.SetString("IsAdmin", "true");
        httpContext.Session = session;
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WhenNotAdmin_RedirectsToAdminLogin()
    {
        var model = CreatePageModel(isAdmin: false);

        var result = await model.OnGetAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenAdmin_ReturnsPageWithCounts()
    {
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "T1", Place = "P1", Days = 1, Price = 1000m, Locations = "L1", TourInfo = "I1" },
            new Tour { TourId = 2, TourName = "T2", Place = "P2", Days = 2, Price = 2000m, Locations = "L2", TourInfo = "I2" }
        };
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "a@a.com" },
            new UserInfo { Email = "b@b.com" },
            new UserInfo { Email = "c@c.com" }
        };
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "T1", Place = "P1", Email = "a@a.com", FirstName = "A" }
        };

        _mockTourRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);
        _mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
        _mockBookingRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var model = CreatePageModel();

        var result = await model.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(2, model.TotalTours);
        Assert.Equal(3, model.TotalUsers);
        Assert.Equal(1, model.TotalBookings);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionThrown_ReturnsPageWithZeroCounts()
    {
        _mockTourRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel();

        var result = await model.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.Equal(0, model.TotalTours);
        Assert.Equal(0, model.TotalUsers);
        Assert.Equal(0, model.TotalBookings);
    }
}

/// <summary>
/// Unit tests for Admin/Users page model.
/// </summary>
public class AdminUsersModelTests
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UserService>> _mockServiceLogger;
    private readonly Mock<ILogger<UsersModel>> _mockPageLogger;
    private readonly UserService _userService;

    public AdminUsersModelTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockServiceLogger = new Mock<ILogger<UserService>>();
        _mockPageLogger = new Mock<ILogger<UsersModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _userService = new UserService(_mockUserRepo.Object, _mapper, _mockServiceLogger.Object);
    }

    private UsersModel CreatePageModel(bool isAdmin = true)
    {
        var model = new UsersModel(_userService, _mockPageLogger.Object);
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        if (isAdmin) session.SetString("IsAdmin", "true");
        httpContext.Session = session;
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WhenNotAdmin_RedirectsToAdminLogin()
    {
        var model = CreatePageModel(isAdmin: false);

        var result = await model.OnGetAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenAdminWithNoSearch_ReturnsAllUsers()
    {
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "a@a.com", FirstName = "Alice", LastName = "A", Gender = "Female", Street = "S1", City = "C1", State = "ST1" },
            new UserInfo { Email = "b@b.com", FirstName = "Bob", LastName = "B", Gender = "Male", Street = "S2", City = "C2", State = "ST2" }
        };
        _mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var model = CreatePageModel();

        var result = await model.OnGetAsync();

        Assert.IsType<PageResult>(result);
        var userList = new List<UserProfileViewModel>(model.Users);
        Assert.Equal(2, userList.Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenAdminWithSearchTerm_CallsSearchAsync()
    {
        var users = new List<UserInfo>
        {
            new UserInfo { Email = "alice@a.com", FirstName = "Alice", LastName = "A", Gender = "Female", Street = "S1", City = "C1", State = "ST1" }
        };
        _mockUserRepo.Setup(r => r.SearchAsync("Alice", It.IsAny<CancellationToken>())).ReturnsAsync(users);

        var model = CreatePageModel();
        model.SearchTerm = "Alice";

        var result = await model.OnGetAsync();

        _mockUserRepo.Verify(r => r.SearchAsync("Alice", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionThrown_SetsErrorAndReturnsPage()
    {
        _mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel();

        var result = await model.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenNotAdmin_RedirectsToAdminLogin()
    {
        var model = CreatePageModel(isAdmin: false);

        var result = await model.OnPostDeleteAsync("user@example.com");

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenAdmin_DeletesUserAndRedirects()
    {
        _mockUserRepo.Setup(r => r.ExistsAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockUserRepo.Setup(r => r.DeleteAsync("user@example.com", It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var model = CreatePageModel();

        var result = await model.OnPostDeleteAsync("user@example.com");

        Assert.IsType<RedirectToPageResult>(result);
        Assert.True(model.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenExceptionThrown_SetsErrorAndRedirects()
    {
        _mockUserRepo.Setup(r => r.ExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel();

        var result = await model.OnPostDeleteAsync("user@example.com");

        Assert.IsType<RedirectToPageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }
}

/// <summary>
/// Shared mock session for Admin page tests.
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
