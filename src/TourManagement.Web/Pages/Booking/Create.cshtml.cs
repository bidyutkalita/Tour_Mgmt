using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Booking;

/// <summary>
/// Page model for creating a new booking.
/// </summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(IBookingService bookingService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    [BindProperty]
    public BookingFormViewModel BookingForm { get; set; } = new();

    public void OnGet(string? tourName, string? place)
    {
        // Pre-fill from query string if coming from tour listing
        if (!string.IsNullOrEmpty(tourName))
            BookingForm.TourName = tourName;
        if (!string.IsNullOrEmpty(place))
            BookingForm.Place = place;

        // Pre-fill user info from session
        var email = HttpContext.Session.GetString("UserEmail");
        var firstName = HttpContext.Session.GetString("UserFirstName");
        if (!string.IsNullOrEmpty(email))
            BookingForm.Email = email;
        if (!string.IsNullOrEmpty(firstName))
            BookingForm.FirstName = firstName;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var dto = new BookingCreateDto
            {
                TourName = BookingForm.TourName,
                Place = BookingForm.Place,
                Email = BookingForm.Email,
                FirstName = BookingForm.FirstName
            };

            await _bookingService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("Booking created for tour {TourName} by {Email}", dto.TourName, dto.Email);
            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            return RedirectToPage("MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking.");
            return Page();
        }
    }
}
