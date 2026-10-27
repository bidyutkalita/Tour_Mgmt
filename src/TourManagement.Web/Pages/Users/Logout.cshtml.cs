using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user logout.
/// </summary>
public class LogoutModel : PageModel
{
    /// <summary>Handles GET requests for logout.</summary>
    public IActionResult OnGet()
    {
        HttpContext.Session.Remove("UserEmail");
        HttpContext.Session.Remove("UserName");
        return RedirectToPage("/Users/Login");
    }
}
