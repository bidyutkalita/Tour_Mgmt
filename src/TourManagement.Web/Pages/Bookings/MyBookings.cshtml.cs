using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for displaying the current user's bookings.
/// </summary>
[Authorize]
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    /// <summary>Gets the list of bookings for the current user.</summary>
    public IEnumerable<BookingListViewModel> Bookings { get; private set; } = Enumerable.Empty<BookingListViewModel>();

    /// <summary>Initializes a new instance of MyBookingsModel.</summary>
    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the my bookings page.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var email = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("/Users/Login");

            var bookings = await _bookingService.GetByEmailAsync(email, cancellationToken);
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
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings for current user");
            return RedirectToPage("/Index");
        }
    }
}
