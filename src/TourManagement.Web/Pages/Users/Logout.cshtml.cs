using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user logout.
/// </summary>
public class LogoutModel : PageModel
{
    private readonly ILogger<LogoutModel> _logger;

    /// <summary>Initializes a new instance of LogoutModel.</summary>
    public LogoutModel(ILogger<LogoutModel> logger)
    {
        _logger = logger;
    }

    /// <summary>Handles POST requests to log out the user.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        var email = User.Identity?.Name;
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("User '{Email}' logged out", email);
        return RedirectToPage("/Index");
    }
}
