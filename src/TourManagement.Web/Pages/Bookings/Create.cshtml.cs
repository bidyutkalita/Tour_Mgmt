using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for creating a new booking.
/// </summary>
public class CreateModel : PageModel
{
    private readonly IBookingService _bookingService;
    private readonly ILogger<CreateModel> _logger;

    /// <summary>Gets or sets the input view model bound from the form.</summary>
    [BindProperty]
    public BookingCreateViewModel Input { get; set; } = new();

    /// <summary>Initializes a new instance of CreateModel.</summary>
    public CreateModel(IBookingService bookingService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the booking creation page.</summary>
    public IActionResult OnGet(int? tourId, string? tourName, string? place)
    {
        Input = new BookingCreateViewModel
        {
            TourId = tourId,
            TourName = tourName ?? string.Empty,
            Place = place ?? string.Empty,
            Email = User.Identity?.Name ?? string.Empty
        };

        if (User.Identity?.IsAuthenticated == true)
        {
            Input.Email = User.Identity.Name ?? string.Empty;
            var firstName = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.GivenName)?.Value;
            Input.FirstName = firstName ?? string.Empty;
        }

        return Page();
    }

    /// <summary>Handles POST requests to create a new booking.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
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

            var created = await _bookingService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("Booking created for tour '{TourName}' by '{Email}'", created.TourName, created.Email);
            TempData["SuccessMessage"] = $"Booking for '{created.TourName}' confirmed successfully!";
            return RedirectToPage("MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour '{TourName}'", Input.TourName);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the booking. Please try again.");
            return Page();
        }
    }
}
