using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for displaying the current user's bookings.
/// </summary>
[Authorize]
public class MyBookingsModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<MyBookingsModel> _logger;

    public IEnumerable<BookingDto> Bookings { get; private set; } = Enumerable.Empty<BookingDto>();

    public MyBookingsModel(IBookingService bookingService, ILogger<MyBookingsModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            if (!string.IsNullOrEmpty(email))
            {
                Bookings = await _bookingService.GetByEmailAsync(email, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bookings for user");
            Bookings = Enumerable.Empty<BookingDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _bookingService.DeleteAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Booking cancelled successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking ID {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
        }
        return RedirectToPage();
    }
}
