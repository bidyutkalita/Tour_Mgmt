using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for the user profile page.
/// </summary>
[Authorize]
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly IBookingService _bookingService;
    private readonly ILogger<ProfileModel> _logger;

    /// <summary>Gets the user details to display.</summary>
    public UserDetailsViewModel? User { get; private set; }

    /// <summary>Gets the recent bookings for the user.</summary>
    public IEnumerable<BookingListViewModel> RecentBookings { get; private set; } = Enumerable.Empty<BookingListViewModel>();

    /// <summary>Initializes a new instance of ProfileModel.</summary>
    public ProfileModel(IUserService userService, IBookingService bookingService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _bookingService = bookingService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the profile page.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var email = HttpContext.User.Identity?.Name;
            if (string.IsNullOrEmpty(email))
                return RedirectToPage("Login");

            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user is null)
                return RedirectToPage("Login");

            User = new UserDetailsViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Dob = user.Dob,
                Street = user.Street,
                City = user.City,
                State = user.State,
                CreatedDate = user.CreatedDate,
                IsActive = user.IsActive
            };

            var bookings = await _bookingService.GetByEmailAsync(email, cancellationToken);
            RecentBookings = bookings.Take(5).Select(b => new BookingListViewModel
            {
                BookingId = b.BookingId,
                TourName = b.TourName,
                Place = b.Place,
                Email = b.Email,
                FirstName = b.FirstName,
                CreatedDate = b.CreatedDate,
                IsActive = b.IsActive
            });

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user profile");
            return RedirectToPage("/Index");
        }
    }
}
