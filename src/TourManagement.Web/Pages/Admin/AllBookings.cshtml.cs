using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin view of all bookings.
/// </summary>
[Authorize(Roles = "Admin")]
public class AllBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<AllBookingsModel> _logger;

    public IEnumerable<BookingDto> Bookings { get; private set; } = Enumerable.Empty<BookingDto>();

    public AllBookingsModel(IBookingService bookingService, ILogger<AllBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Bookings = await _bookingService.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading all bookings for admin");
            Bookings = Enumerable.Empty<BookingDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _bookingService.DeleteAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Booking deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking ID {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the booking.";
        }
        return RedirectToPage();
    }
}
