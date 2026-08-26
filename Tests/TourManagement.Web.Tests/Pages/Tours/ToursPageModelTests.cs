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
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Web.Pages.Tours;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Tests.Pages.Tours;

/// <summary>
/// Unit tests for Tours/Index page model.
/// </summary>
public class ToursIndexModelTests
{
    private readonly Mock<ITourRepository> _mockTourRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TourService>> _mockTourLogger;
    private readonly Mock<ILogger<IndexModel>> _mockPageLogger;
    private readonly TourService _tourService;

    public ToursIndexModelTests()
    {
        _mockTourRepo = new Mock<ITourRepository>();
        _mockTourLogger = new Mock<ILogger<TourService>>();
        _mockPageLogger = new Mock<ILogger<IndexModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _tourService = new TourService(_mockTourRepo.Object, _mapper, _mockTourLogger.Object);
    }

    private IndexModel CreatePageModel()
    {
        var model = new IndexModel(_tourService, _mockPageLogger.Object);
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext
        {
            HttpContext = httpContext
        };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WithNoSearchTerm_ReturnsAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000m, Locations = "Baga", TourInfo = "Beaches" },
            new Tour { TourId = 2, TourName = "Kashmir Tour", Place = "Kashmir", Days = 7, Price = 25000m, Locations = "Dal Lake", TourInfo = "Paradise" }
        };
        _mockTourRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var model = CreatePageModel();

        // Act
        await model.OnGetAsync();

        // Assert
        Assert.NotNull(model.Tours);
        var tourList = new List<TourViewModel>(model.Tours);
        Assert.Equal(2, tourList.Count);
    }

    [Fact]
    public async Task OnGetAsync_WithSearchTerm_CallsSearchAsync()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000m, Locations = "Baga", TourInfo = "Beaches" }
        };
        _mockTourRepo.Setup(r => r.SearchAsync("Goa", It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var model = CreatePageModel();
        model.SearchTerm = "Goa";

        // Act
        await model.OnGetAsync();

        // Assert
        _mockTourRepo.Verify(r => r.SearchAsync("Goa", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionThrown_SetsErrorTempData()
    {
        // Arrange
        _mockTourRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel();

        // Act
        await model.OnGetAsync();

        // Assert
        Assert.True(model.TempData.ContainsKey("ErrorMessage"));
    }

    [Fact]
    public async Task OnGetAsync_WithWhitespaceSearchTerm_ReturnsAllTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { TourId = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000m, Locations = "Baga", TourInfo = "Beaches" }
        };
        _mockTourRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tours);

        var model = CreatePageModel();
        model.SearchTerm = "   "; // whitespace only

        // Act
        await model.OnGetAsync();

        // Assert
        _mockTourRepo.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}

/// <summary>
/// Unit tests for Tours/Details page model.
/// </summary>
public class ToursDetailsModelTests
{
    private readonly Mock<ITourRepository> _mockTourRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TourService>> _mockTourLogger;
    private readonly Mock<ILogger<DetailsModel>> _mockPageLogger;
    private readonly TourService _tourService;

    public ToursDetailsModelTests()
    {
        _mockTourRepo = new Mock<ITourRepository>();
        _mockTourLogger = new Mock<ILogger<TourService>>();
        _mockPageLogger = new Mock<ILogger<DetailsModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _tourService = new TourService(_mockTourRepo.Object, _mapper, _mockTourLogger.Object);
    }

    private DetailsModel CreatePageModel()
    {
        var model = new DetailsModel(_tourService, _mockPageLogger.Object);
        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        model.TempData = tempData;
        model.PageContext = new PageContext { HttpContext = httpContext };
        return model;
    }

    [Fact]
    public async Task OnGetAsync_WhenTourExists_ReturnsPage()
    {
        // Arrange
        var tour = new Tour { TourId = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000m, Locations = "Baga", TourInfo = "Beaches" };
        _mockTourRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var model = CreatePageModel();

        // Act
        var result = await model.OnGetAsync(1);

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.Tour);
        Assert.Equal("Goa Tour", model.Tour!.TourName);
    }

    [Fact]
    public async Task OnGetAsync_WhenTourNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockTourRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        var model = CreatePageModel();

        // Act
        var result = await model.OnGetAsync(999);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_WhenExceptionThrown_RedirectsToIndex()
    {
        // Arrange
        _mockTourRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("DB error"));

        var model = CreatePageModel();

        // Act
        var result = await model.OnGetAsync(1);

        // Assert
        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenTourExists_MapsAllProperties()
    {
        // Arrange
        var tour = new Tour
        {
            TourId = 5,
            TourName = "Kerala Tour",
            Place = "Kerala",
            Days = 6,
            Price = 20000m,
            Locations = "Backwaters",
            TourInfo = "God's own country",
            Pic = "kerala.jpg"
        };
        _mockTourRepo.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var model = CreatePageModel();

        // Act
        await model.OnGetAsync(5);

        // Assert
        Assert.NotNull(model.Tour);
        Assert.Equal(5, model.Tour!.TourId);
        Assert.Equal("Kerala Tour", model.Tour.TourName);
        Assert.Equal("Kerala", model.Tour.Place);
        Assert.Equal(6, model.Tour.Days);
        Assert.Equal(20000m, model.Tour.Price);
        Assert.Equal("Backwaters", model.Tour.Locations);
        Assert.Equal("God's own country", model.Tour.TourInfo);
        Assert.Equal("kerala.jpg", model.Tour.Pic);
    }
}

/// <summary>
/// Unit tests for Tours/Delete page model.
/// </summary>
public class ToursDeleteModelTests
{
    private readonly Mock<ITourRepository> _mockTourRepo;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<TourService>> _mockTourLogger;
    private readonly Mock<ILogger<DeleteModel>> _mockPageLogger;
    private readonly TourService _tourService;

    public ToursDeleteModelTests()
    {
        _mockTourRepo = new Mock<ITourRepository>();
        _mockTourLogger = new Mock<ILogger<TourService>>();
        _mockPageLogger = new Mock<ILogger<DeleteModel>>();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        _tourService = new TourService(_mockTourRepo.Object, _mapper, _mockTourLogger.Object);
    }

    private DeleteModel CreatePageModel(bool isAdmin = true)
    {
        var model = new DeleteModel(_tourService, _mockPageLogger.Object);
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

        var result = await model.OnGetAsync(1);

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnGetAsync_WhenAdminAndTourExists_ReturnsPage()
    {
        var tour = new Tour { TourId = 1, TourName = "Goa Tour", Place = "Goa", Days = 5, Price = 15000m, Locations = "Baga", TourInfo = "Beaches" };
        _mockTourRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var model = CreatePageModel();

        var result = await model.OnGetAsync(1);

        Assert.IsType<PageResult>(result);
        Assert.NotNull(model.Tour);
    }

    [Fact]
    public async Task OnGetAsync_WhenTourNotFound_ReturnsNotFound()
    {
        _mockTourRepo.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        var model = CreatePageModel();

        var result = await model.OnGetAsync(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_WhenNotAdmin_RedirectsToAdminLogin()
    {
        var model = CreatePageModel(isAdmin: false);

        var result = await model.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Admin/Login", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenTourIsNull_RedirectsToIndex()
    {
        var model = CreatePageModel();
        model.Tour = null;

        var result = await model.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirect.PageName);
    }

    [Fact]
    public async Task OnPostAsync_WhenTourExists_DeletesAndRedirects()
    {
        _mockTourRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _mockTourRepo.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var model = CreatePageModel();
        model.Tour = new TourViewModel { TourId = 1, TourName = "Goa Tour" };

        var result = await model.OnPostAsync();

        var redirect = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("./Index", redirect.PageName);
        Assert.True(model.TempData.ContainsKey("SuccessMessage"));
    }

    [Fact]
    public async Task OnPostAsync_WhenNotFoundException_ReturnsNotFound()
    {
        _mockTourRepo.Setup(r => r.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var model = CreatePageModel();
        model.Tour = new TourViewModel { TourId = 1, TourName = "Goa Tour" };

        var result = await model.OnPostAsync();

        Assert.IsType<NotFoundResult>(result);
    }
}

/// <summary>
/// Mock session for testing.
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
