using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Bookings;

/// <summary>
/// Page model for the create booking page.
/// </summary>
public class CreateModel : PageModel
{
    private readonly BookingService _bookingService;
    private readonly TourService _tourService;
    private readonly ILogger<CreateModel> _logger;

    /// <summary>Gets or sets the booking form data.</summary>
    [BindProperty]
    public BookingFormViewModel Booking { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="CreateModel"/>.
    /// </summary>
    public CreateModel(BookingService bookingService, TourService tourService, ILogger<CreateModel> logger)
    {
        _bookingService = bookingService;
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the create booking page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(int? tourId, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
        {
            return RedirectToPage("/Users/Login");
        }

        Booking.Email = HttpContext.Session.GetString("UserEmail") ?? string.Empty;
        Booking.FirstName = HttpContext.Session.GetString("UserName")?.Split(' ').FirstOrDefault() ?? string.Empty;

        if (tourId.HasValue)
        {
            try
            {
                var tour = await _tourService.GetByIdAsync(tourId.Value, cancellationToken);
                if (tour != null)
                {
                    Booking.TourName = tour.TourName;
                    Booking.Place = tour.Place;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not load tour {TourId} for booking pre-fill", tourId);
            }
        }

        return Page();
    }

    /// <summary>
    /// Handles POST requests to create a new booking.
    /// </summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
        {
            return RedirectToPage("/Users/Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var booking = new Booking
            {
                TourName = Booking.TourName,
                Place = Booking.Place,
                Email = Booking.Email,
                FirstName = Booking.FirstName
            };

            await _bookingService.CreateAsync(booking, cancellationToken);
            TempData["SuccessMessage"] = "Booking confirmed successfully!";
            return RedirectToPage("./MyBookings");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user {Email}", Booking.Email);
            TempData["ErrorMessage"] = "An error occurred while creating the booking.";
            return Page();
        }
    }
}
