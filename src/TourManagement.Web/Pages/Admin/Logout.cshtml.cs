using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin logout.
/// </summary>
public class LogoutModel : PageModel
{
    /// <summary>Handles GET requests for admin logout.</summary>
    public IActionResult OnGet()
    {
        HttpContext.Session.Remove("AdminEmail");
        return RedirectToPage("/Admin/Login");
    }
}
