using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for the all bookings admin page.
/// </summary>
public class AllBookingsModel : PageModel
{
    private readonly BookingService _bookingService;
    private readonly ILogger<AllBookingsModel> _logger;

    /// <summary>Gets the list of all bookings.</summary>
    public IEnumerable<BookingViewModel> Bookings { get; private set; } = new List<BookingViewModel>();

    /// <summary>Gets or sets the search term.</summary>
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="AllBookingsModel"/>.
    /// </summary>
    public AllBookingsModel(BookingService bookingService, ILogger<AllBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the all bookings page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            var bookings = string.IsNullOrWhiteSpace(SearchTerm)
                ? await _bookingService.GetAllAsync(cancellationToken)
                : await _bookingService.SearchAsync(SearchTerm, cancellationToken);

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
            _logger.LogError(ex, "Error loading all bookings");
            TempData["ErrorMessage"] = "An error occurred while loading bookings.";
        }

        return Page();
    }

    /// <summary>
    /// Handles POST requests to delete a booking.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteAsync(int bookingId, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            await _bookingService.DeleteAsync(bookingId, cancellationToken);
            TempData["SuccessMessage"] = $"Booking #{bookingId} deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking {BookingId}", bookingId);
            TempData["ErrorMessage"] = "An error occurred while deleting the booking.";
        }

        return RedirectToPage();
    }
}
