using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for cancelling/deleting a booking.
/// </summary>
public class DeleteModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<DeleteModel> _logger;

    /// <summary>Gets the booking to be cancelled.</summary>
    public BookingDeleteViewModel? Booking { get; private set; }

    /// <summary>Initializes a new instance of DeleteModel.</summary>
    public DeleteModel(IBookingService bookingService, ILogger<DeleteModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the booking cancellation confirmation page.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var booking = await _bookingService.GetByIdAsync(id, cancellationToken);
            if (booking is null)
                return NotFound();

            Booking = new BookingDeleteViewModel
            {
                BookingId = booking.BookingId,
                TourName = booking.TourName,
                Place = booking.Place,
                Email = booking.Email,
                FirstName = booking.FirstName
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading booking for cancellation with ID {BookingId}", id);
            return RedirectToPage("MyBookings");
        }
    }

    /// <summary>Handles POST requests to cancel a booking.</summary>
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _bookingService.DeleteAsync(id, cancellationToken);
            if (!result)
                return NotFound();

            TempData["SuccessMessage"] = "Booking cancelled successfully!";
            return RedirectToPage("MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling booking with ID {BookingId}", id);
            TempData["ErrorMessage"] = "An error occurred while cancelling the booking.";
            return RedirectToPage("MyBookings");
        }
    }
}
