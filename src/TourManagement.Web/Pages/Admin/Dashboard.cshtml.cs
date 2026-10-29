using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin dashboard.
/// </summary>
[Authorize(Roles = "Admin")]
public class DashboardModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IBookingService _bookingService;
    private readonly IUserService _userService;
    private readonly ILogger<DashboardModel> _logger;

    public int TotalTours { get; private set; }
    public int TotalBookings { get; private set; }
    public int TotalUsers { get; private set; }

    public DashboardModel(
        ITourService tourService,
        IBookingService bookingService,
        IUserService userService,
        ILogger<DashboardModel> logger)
    {
        _tourService = tourService;
        _bookingService = bookingService;
        _userService = userService;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var tours = await _tourService.GetAllAsync(cancellationToken);
            var bookings = await _bookingService.GetAllAsync(cancellationToken);
            var users = await _userService.GetAllAsync(cancellationToken);

            TotalTours = tours.Count();
            TotalBookings = bookings.Count();
            TotalUsers = users.Count();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard data");
        }
    }
}
