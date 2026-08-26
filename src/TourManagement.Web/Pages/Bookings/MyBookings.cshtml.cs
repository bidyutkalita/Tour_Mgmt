using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for the user's bookings page.
/// </summary>
public class MyBookingsModel : PageModel
{
    private readonly BookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    /// <summary>Gets the list of user's bookings.</summary>
    public IEnumerable<BookingViewModel> Bookings { get; private set; } = new List<BookingViewModel>();

    /// <summary>
    /// Initializes a new instance of <see cref="MyBookingsModel"/>.
    /// </summary>
    public MyBookingsModel(BookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the my bookings page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            var bookings = await _bookingService.GetByUserEmailAsync(email, cancellationToken);
            Bookings = bookings.Select(b => new BookingViewModel
            {
                BookingId = b.BookingId,
                TourName = b.TourName,
                Place = b.Place,
                Email = b.Email,
                FirstName = b.FirstName,
                BookingDate = b.BookingDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings for user {Email}", email);
            TempData["ErrorMessage"] = "An error occurred while loading your bookings.";
        }

        return Page();
    }

    /// <summary>
    /// Handles POST requests to cancel a booking.
    /// </summary>
    public async Task<IActionResult> OnPostCancelAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            await _bookingService.DeleteAsync(bookingId, cancellationToken);
            TempData["SuccessMessage"] = "Booking cancelled successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking {BookingId}", bookingId);
            TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
        }

        return RedirectToPage();
    }
}
