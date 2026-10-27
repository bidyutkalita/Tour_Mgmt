using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Exceptions;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for editing user profile.
/// </summary>
public class EditModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<EditModel> _logger;

    /// <summary>Initializes a new instance of EditModel.</summary>
    public EditModel(IUserService userService, ILogger<EditModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Gets or sets the edit input model.</summary>
    [BindProperty]
    public UserEditViewModel Input { get; set; } = new();

    /// <summary>Gets or sets the error message.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(string email, CancellationToken cancellationToken)
    {
        var sessionEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(sessionEmail))
            return RedirectToPage("/Users/Login");

        try
        {
            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user == null)
                return NotFound();

            Input = new UserEditViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                City = user.City,
                Street = user.Street,
                State = user.State
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for edit: {Email}", email);
            return RedirectToPage("/Users/Profile");
        }
    }

    /// <summary>Handles POST requests for updating user profile.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return Page();

        var sessionEmail = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(sessionEmail))
            return RedirectToPage("/Users/Login");

        try
        {
            var dto = new UserUpdateDto
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                City = Input.City,
                Street = Input.Street,
                State = Input.State
            };

            await _userService.UpdateAsync(Input.Email, dto, cancellationToken);
            _logger.LogInformation("User {Email} profile updated successfully", Input.Email);
            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToPage("/Users/Profile");
        }
        catch (NotFoundException)
        {
            ErrorMessage = "User not found.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile: {Email}", Input.Email);
            ErrorMessage = "An error occurred while updating your profile.";
            return Page();
        }
    }
}
