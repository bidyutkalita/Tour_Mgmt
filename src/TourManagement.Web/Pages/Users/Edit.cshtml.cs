using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for editing a user profile.
/// </summary>
[Authorize]
public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<EditModel> _logger;

    /// <summary>Gets or sets the input view model bound from the form.</summary>
    [BindProperty]
    public UserEditViewModel Input { get; set; } = new();

    /// <summary>Initializes a new instance of EditModel.</summary>
    public EditModel(IUserService userService, ILogger<EditModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the edit profile page.</summary>
    public async Task<IActionResult> OnGetAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserEmail = HttpContext.User.Identity?.Name;
            if (currentUserEmail != email && !User.IsInRole("Admin"))
                return Forbid();

            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user is null)
                return NotFound();

            Input = new UserEditViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Dob = user.Dob,
                Street = user.Street,
                City = user.City,
                State = user.State,
                IsActive = user.IsActive
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit with email {Email}", email);
            return RedirectToPage("Profile");
        }
    }

    /// <summary>Handles POST requests to update a user profile.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var dto = new UserUpdateDto
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                Dob = Input.Dob,
                Street = Input.Street,
                City = Input.City,
                State = Input.State,
                IsActive = Input.IsActive
            };

            var updated = await _userService.UpdateAsync(Input.Email, dto, cancellationToken);
            if (updated is null)
                return NotFound();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage("Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for {Email}", Input.Email);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the profile. Please try again.");
            return Page();
        }
    }
}
