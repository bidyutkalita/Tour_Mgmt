using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for viewing all bookings (admin only).
/// </summary>
public class AllBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<AllBookingsModel> _logger;

    /// <summary>Initializes a new instance of AllBookingsModel.</summary>
    public AllBookingsModel(IBookingService bookingService, ILogger<AllBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Gets or sets all bookings.</summary>
    public IEnumerable<BookingDto> Bookings { get; set; } = Enumerable.Empty<BookingDto>();

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        try
        {
            Bookings = await _bookingService.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading all bookings");
        }

        return Page();
    }

    /// <summary>Handles POST requests for deleting a booking.</summary>
    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        try
        {
            await _bookingService.DeleteAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Booking deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking {BookingId}", id);
            TempData["ErrorMessage"] = "Error deleting booking.";
        }

        return RedirectToPage();
    }
}
