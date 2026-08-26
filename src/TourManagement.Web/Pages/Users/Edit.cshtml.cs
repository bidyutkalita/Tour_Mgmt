using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for the edit user profile page.
/// </summary>
public class EditModel : PageModel
{
    private readonly UserService _userService;
    private readonly ILogger<EditModel> _logger;

    /// <summary>Gets or sets the edit form input.</summary>
    [BindProperty]
    public UserUpdateViewModel Input { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of <see cref="EditModel"/>.
    /// </summary>
    public EditModel(UserService userService, ILogger<EditModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the edit profile page.
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
                return RedirectToPage("./Login");
            }

            Input = new UserUpdateViewModel
            {
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
            _logger.LogError(ex, "Error loading profile for edit, user {Email}", email);
            return RedirectToPage("./Profile");
        }
    }

    /// <summary>
    /// Handles POST requests to update the user profile.
    /// </summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToPage("./Login");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var updatedUser = new UserInfo
            {
                Email = email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                Dob = Input.Dob,
                Street = Input.Street,
                City = Input.City,
                State = Input.State
            };

            await _userService.UpdateAsync(email, updatedUser, Input.Password, cancellationToken);
            HttpContext.Session.SetString("UserName", $"{Input.FirstName} {Input.LastName}");
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToPage("./Profile");
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {Email}", email);
            TempData["ErrorMessage"] = "An error occurred while updating your profile.";
            return Page();
        }
    }
}
