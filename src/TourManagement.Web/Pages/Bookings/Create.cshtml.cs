using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

    /// <summary>Initializes a new instance of CreateModel.</summary>
    public CreateModel(IBookingService bookingService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Gets or sets the booking input model.</summary>
    [BindProperty]
    public BookingCreateViewModel Input { get; set; } = new();

    /// <summary>Gets or sets the error message.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Handles GET requests.</summary>
    public IActionResult OnGet(int? tourId, string? tourName)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
            return RedirectToPage("/Users/Login");

        if (tourId.HasValue)
            Input.TourId = tourId.Value;

        if (!string.IsNullOrEmpty(tourName))
            Input.TourName = tourName;

        // Pre-fill user name from session
        var userName = HttpContext.Session.GetString("UserName");
        if (!string.IsNullOrEmpty(userName))
            Input.FirstName = userName;

        return Page();
    }

    /// <summary>Handles POST requests for creating a booking.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
            return RedirectToPage("/Users/Login");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            var dto = new BookingCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Email = Input.Email,
                FirstName = Input.FirstName,
                TourId = Input.TourId
            };

            await _bookingService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("Booking created for tour: {TourName}", Input.TourName);
            TempData["SuccessMessage"] = "Booking successful!";
            return RedirectToPage("/Bookings/MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourName}", Input.TourName);
            ErrorMessage = "An error occurred while processing your booking.";
            return Page();
        }
    }
}
