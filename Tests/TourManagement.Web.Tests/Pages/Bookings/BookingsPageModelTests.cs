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
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Web.Pages.Bookings;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.Pages.Bookings;

/// <summary>
/// Unit tests for Bookings/MyBookings page model.
/// </summary>
public class MyBookingsModelTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<BookingService>> _mockServiceLogger;
    private readonly Mock<ILogger<MyBookingsModel>> _mockPageLogger;
    private readonly BookingService _bookingService;

    public MyBookingsModelTests()
    {
        _mockBookingRepo = new Mock<IBookingRepository>();
        _mockServiceLogger = new Mock<ILogger<BookingService>>();
        _mockPageLogger = new Mock<ILogger<MyBookingsModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _bookingService = new BookingService(_mockBookingRepo.Object, _mapper, _mockServiceLogger.Object);
    }

    private MyBookingsModel CreatePageModel(string? sessionEmail = null)
    {
        var model = new MyBookingsModel(_bookingService, _mockPageLogger.Object);
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
        Assert.Equal("/Users/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedIn_ReturnsPageWithBookings()
    {
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Goa Tour", Place = "Goa", Email = "user@example.com", FirstName = "John", BookingDate = DateTime.UtcNow },
            new Booking { BookingId = 2, TourName = "Kashmir Tour", Place = "Kashmir", Email = "user@example.com", FirstName = "John", BookingDate = DateTime.UtcNow }
        };
        _mockBookingRepo.Setup(r => r.GetByUserEmailAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        var model = CreatePageModel(sessionEmail: "user@example.com");

        var result = await model.OnGetAsync();

        Assert.IsType<PageResult>(result);
        var bookingList = new List<BookingViewModel>(model.Bookings);
        Assert.Equal(2, bookingList.Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionThrown_SetsErrorAndReturnsPage()
    {
        _mockBookingRepo.Setup(r => r.GetByUserEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel(sessionEmail: "user@example.com");

        var result = await model.OnGetAsync();

        Assert.IsType<PageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }

    [Fact]
    public async Task OnPostCancelAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var model = CreatePageModel();

        var result = await model.OnPostCancelAsync(1);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Users/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnPostCancelAsync_WhenLoggedIn_DeletesBookingAndRedirects()
    {
        _mockBookingRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockBookingRepo.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var model = CreatePageModel(sessionEmail: "user@example.com");

        var result = await model.OnPostCancelAsync(1);

        Assert.IsType<RedirectToPageResult>(result);
        Assert.True(model.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public async Task OnPostCancelAsync_WhenExceptionThrown_SetsErrorAndRedirects()
    {
        _mockBookingRepo.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel(sessionEmail: "user@example.com");

        var result = await model.OnPostCancelAsync(1);

        Assert.IsType<RedirectToPageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }
}

/// <summary>
/// Unit tests for Bookings/AllBookings page model.
/// </summary>
public class AllBookingsModelTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<BookingService>> _mockServiceLogger;
    private readonly Mock<ILogger<AllBookingsModel>> _mockPageLogger;
    private readonly BookingService _bookingService;

    public AllBookingsModelTests()
    {
        _mockBookingRepo = new Mock<IBookingRepository>();
        _mockServiceLogger = new Mock<ILogger<BookingService>>();
        _mockPageLogger = new Mock<ILogger<AllBookingsModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _bookingService = new BookingService(_mockBookingRepo.Object, _mapper, _mockServiceLogger.Object);
    }

    private AllBookingsModel CreatePageModel(bool isAdmin = true)
    {
        var model = new AllBookingsModel(_bookingService, _mockPageLogger.Object);
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
    public async Task OnGetAsync_WhenAdminWithNoSearch_ReturnsAllBookings()
    {
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Goa Tour", Place = "Goa", Email = "a@a.com", FirstName = "Alice", BookingDate = DateTime.UtcNow },
            new Booking { BookingId = 2, TourName = "Kashmir Tour", Place = "Kashmir", Email = "b@b.com", FirstName = "Bob", BookingDate = DateTime.UtcNow }
        };
        _mockBookingRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var model = CreatePageModel();

        var result = await model.OnGetAsync();

        Assert.IsType<PageResult>(result);
        var bookingList = new List<BookingViewModel>(model.Bookings);
        Assert.Equal(2, bookingList.Count);
    }

    [Fact]
    public async Task OnGetAsync_WhenAdminWithSearchTerm_CallsSearchAsync()
    {
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, TourName = "Goa Tour", Place = "Goa", Email = "a@a.com", FirstName = "Alice", BookingDate = DateTime.UtcNow }
        };
        _mockBookingRepo.Setup(r => r.SearchAsync("Goa", It.IsAny<CancellationToken>())).ReturnsAsync(bookings);

        var model = CreatePageModel();
        model.SearchTerm = "Goa";

        var result = await model.OnGetAsync();

        _mockBookingRepo.Verify(r => r.SearchAsync("Goa", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionThrown_SetsErrorAndReturnsPage()
    {
        _mockBookingRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
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

        var result = await model.OnPostDeleteAsync(1);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenAdmin_DeletesAndRedirects()
    {
        _mockBookingRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockBookingRepo.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var model = CreatePageModel();

        var result = await model.OnPostDeleteAsync(1);

        Assert.IsType<RedirectToPageResult>(result);
        Assert.True(model.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public async Task OnPostDeleteAsync_WhenExceptionThrown_SetsErrorAndRedirects()
    {
        _mockBookingRepo.Setup(r => r.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel();

        var result = await model.OnPostDeleteAsync(1);

        Assert.IsType<RedirectToPageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }
}

/// <summary>
/// Unit tests for Bookings/Create page model.
/// </summary>
public class BookingsCreateModelTests
{
    private readonly Mock<IBookingRepository> _mockBookingRepo;
    private readonly Mock<ITourRepository> _mockTourRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<BookingService>> _mockBookingServiceLogger;
    private readonly Mock<ILogger<TourService>> _mockTourServiceLogger;
    private readonly Mock<ILogger<CreateModel>> _mockPageLogger;
    private readonly BookingService _bookingService;
    private readonly TourService _tourService;

    public BookingsCreateModelTests()
    {
        _mockBookingRepo = new Mock<IBookingRepository>();
        _mockTourRepo = new Mock<ITourRepository>();
        _mockBookingServiceLogger = new Mock<ILogger<BookingService>>();
        _mockTourServiceLogger = new Mock<ILogger<TourService>>();
        _mockPageLogger = new Mock<ILogger<CreateModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _bookingService = new BookingService(_mockBookingRepo.Object, _mapper, _mockBookingServiceLogger.Object);
        _tourService = new TourService(_mockTourRepo.Object, _mapper, _mockTourServiceLogger.Object);
    }

    private CreateModel CreatePageModel(string? sessionEmail = null, string? sessionUserName = null)
    {
        var model = new CreateModel(_bookingService, _tourService, _mockPageLogger.Object);
        var httpContext = new DefaultHttpContext();
        var session = new MockSession();
        if (sessionEmail != null) session.SetString("UserEmail", sessionEmail);
        if (sessionUserName != null) session.SetString("UserName", sessionUserName);
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

        var result = await model.OnGetAsync(tourId: null);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Users/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenLoggedIn_ReturnsPage()
    {
        var model = CreatePageModel(sessionEmail: "user@example.com", sessionUserName: "John Doe");

        var result = await model.OnGetAsync(tourId: null);

        Assert.IsType<PageResult>(result);
        Assert.Equal("user@example.com", model.Booking.Email);
    }

    [Fact]
    public async Task OnGetAsync_WithTourId_PreFillsBookingFromTour()
    {
        var tour = new Tour
        {
            TourId = 1,
            TourName = "Goa Tour",
            Place = "Goa",
            Days = 5,
            Price = 15000m,
            Locations = "Baga",
            TourInfo = "Beaches"
        };
        _mockTourRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var model = CreatePageModel(sessionEmail: "user@example.com", sessionUserName: "John Doe");

        var result = await model.OnGetAsync(tourId: 1);

        Assert.IsType<PageResult>(result);
        Assert.Equal("Goa Tour", model.Booking.TourName);
        Assert.Equal("Goa", model.Booking.Place);
    }

    [Fact]
    public async Task OnPostAsync_WhenNotLoggedIn_RedirectsToLogin()
    {
        var model = CreatePageModel();

        var result = await model.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Users/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModel_ReturnsPage()
    {
        var model = CreatePageModel(sessionEmail: "user@example.com");
        model.ModelState.AddModelError("TourName", "Required");

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WithValidData_CreatesBookingAndRedirects()
    {
        _mockBookingRepo.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var model = CreatePageModel(sessionEmail: "user@example.com");
        model.Booking = new BookingFormViewModel
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "user@example.com",
            FirstName = "John"
        };

        var result = await model.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./MyBookings", redirect.PageName);
        Assert.True(model.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public async Task OnPostAsync_WhenExceptionThrown_SetsErrorAndReturnsPage()
    {
        _mockBookingRepo.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel(sessionEmail: "user@example.com");
        model.Booking = new BookingFormViewModel
        {
            TourName = "Goa Tour",
            Place = "Goa",
            Email = "user@example.com",
            FirstName = "John"
        };

        var result = await model.OnPostAsync();

        Assert.IsType<PageResult>(result);
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }
}

/// <summary>
/// Shared mock session for Bookings page tests.
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
