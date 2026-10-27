using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for listing all users (admin only).
/// </summary>
public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    /// <summary>Initializes a new instance of IndexModel.</summary>
    public IndexModel(IUserService userService, ILogger<IndexModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Gets or sets the list of users.</summary>
    public IEnumerable<UserDto> Users { get; set; } = Enumerable.Empty<UserDto>();

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        try
        {
            Users = await _userService.GetAllAsync(cancellationToken);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users list");
            return Page();
        }
    }

    /// <summary>Handles POST requests for deleting a user.</summary>
    public async Task<IActionResult> OnPostDeleteAsync(string email, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        try
        {
            await _userService.DeleteAsync(email, cancellationToken);
            TempData["SuccessMessage"] = "User deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {Email}", email);
            TempData["ErrorMessage"] = "Error deleting user.";
        }
        return RedirectToPage();
    }
}
