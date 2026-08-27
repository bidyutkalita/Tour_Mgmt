using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin bookings management page.
/// </summary>
[Authorize(Roles = "Admin")]
public class BookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<BookingsModel> _logger;

    /// <summary>Gets the list of all bookings.</summary>
    public IEnumerable<BookingListViewModel> Bookings { get; private set; } = Enumerable.Empty<BookingListViewModel>();

    /// <summary>Initializes a new instance of BookingsModel.</summary>
    public BookingsModel(IBookingService bookingService, ILogger<BookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the admin bookings page.</summary>
    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var bookings = await _bookingService.GetAllAsync(cancellationToken);
            Bookings = bookings.Select(b => new BookingListViewModel
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
            _logger.LogError(ex, "Error loading all bookings for admin");
        }
    }
}
