using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for the user profile page.
/// </summary>
public class ProfileModel : PageModel
{
    private readonly UserService _userService;
    private readonly ILogger<ProfileModel> _logger;

    /// <summary>Gets the user profile to display.</summary>
    public UserProfileViewModel? Profile { get; private set; }

    /// <summary>
    /// Initializes a new instance of <see cref="ProfileModel"/>.
    /// </summary>
    public ProfileModel(UserService userService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the profile page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("./Login");
        }

        try
        {
            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                HttpContext.Session.Clear();
                return RedirectToPage("./Login");
            }

            Profile = new UserProfileViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Dob = user.Dob,
                Street = user.Street,
                City = user.City,
                State = user.State
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading profile for user {Email}", email);
            TempData["ErrorMessage"] = "An error occurred while loading your profile.";
            return Page();
        }
    }
}
