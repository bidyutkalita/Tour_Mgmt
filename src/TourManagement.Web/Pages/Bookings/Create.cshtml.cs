using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for creating a new booking.
/// </summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public BookingCreateViewModel Input { get; set; } = new();

    public CreateModel(IBookingService bookingService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    public IActionResult OnGet(string? tourName = null, string? place = null)
    {
        // Pre-populate from query string (from tour listing page)
        Input.TourName = tourName ?? string.Empty;
        Input.Place = place ?? string.Empty;

        // Pre-populate email from logged-in user
        var email = User.FindFirstValue(ClaimTypes.Name);
        if (!string.IsNullOrEmpty(email))
        {
            Input.Email = email;
            Input.FirstName = User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manually map ViewModel to DTO
            var dto = new BookingCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Email = Input.Email,
                FirstName = Input.FirstName
            };

            await _bookingService.CreateAsync(dto, cancellationToken);
            TempData["SuccessMessage"] = $"Booking for '{Input.TourName}' confirmed successfully!";
            return RedirectToPage("MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour {TourName}", Input.TourName);
            ModelState.AddModelError(string.Empty, "An error occurred while processing your booking. Please try again.");
            return Page();
        }
    }
}
