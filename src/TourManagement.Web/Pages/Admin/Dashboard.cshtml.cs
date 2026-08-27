using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin dashboard.
/// </summary>
[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IUserService _userService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<DashboardModel> _logger;

    /// <summary>Gets the total number of tours.</summary>
    public int TotalTours { get; private set; }

    /// <summary>Gets the total number of users.</summary>
    public int TotalUsers { get; private set; }

    /// <summary>Gets the total number of bookings.</summary>
    public int TotalBookings { get; private set; }

    /// <summary>Gets the recent bookings for the dashboard.</summary>
    public IEnumerable<BookingListViewModel> RecentBookings { get; private set; } = Enumerable.Empty<BookingListViewModel>();

    /// <summary>Initializes a new instance of DashboardModel.</summary>
    public DashboardModel(
        ITourService tourService,
        IUserService userService,
        IBookingService bookingService,
        ILogger<DashboardModel> logger)
    {
        _tourService = tourService;
        _userService = userService;
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the admin dashboard.</summary>
    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var tours = await _tourService.GetAllAsync(cancellationToken);
            var users = await _userService.GetAllAsync(cancellationToken);
            var bookings = await _bookingService.GetAllAsync(cancellationToken);

            TotalTours = tours.Count();
            TotalUsers = users.Count();
            TotalBookings = bookings.Count();

            RecentBookings = bookings.Take(5).Select(b => new BookingListViewModel
            {
                BookingId = b.BookingId,
                TourName = b.TourName,
                Place = b.Place,
                Email = b.Email,
                FirstName = b.FirstName,
                CreatedDate = b.CreatedDate,
                IsActive = b.IsActive
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
        }
    }
}
