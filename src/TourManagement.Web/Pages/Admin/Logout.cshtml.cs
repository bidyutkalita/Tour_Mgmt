using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin logout.
/// </summary>
public class LogoutModel : PageModel
{
    private readonly ILogger<LogoutModel> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="LogoutModel"/>.
    /// </summary>
    public LogoutModel(ILogger<LogoutModel> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests to log out the admin.
    /// </summary>
    public IActionResult OnGet()
    {
        HttpContext.Session.Remove("IsAdmin");
        HttpContext.Session.Remove("AdminEmail");
        _logger.LogInformation("Admin logged out");
        TempData["SuccessMessage"] = "Admin logged out successfully.";
        return RedirectToPage("/Index");
    }
}
