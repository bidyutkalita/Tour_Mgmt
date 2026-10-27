using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user profile.
/// </summary>
public class ProfileModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<ProfileModel> _logger;

    /// <summary>Initializes a new instance of ProfileModel.</summary>
    public ProfileModel(IUserService userService, ILogger<ProfileModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Gets or sets the user profile view model.</summary>
    public UserProfileViewModel? Profile { get; set; }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
            return RedirectToPage("/Users/Login");

        try
        {
            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user == null)
                return RedirectToPage("/Users/Login");

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
            _logger.LogError(ex, "Error loading profile for {Email}", email);
            return RedirectToPage("/Users/Login");
        }
    }
}
