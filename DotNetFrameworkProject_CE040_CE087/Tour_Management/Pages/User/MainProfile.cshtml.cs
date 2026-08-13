using Microsoft.AspNetCore.Mvc.RazorPages;

// Replaces MainProfilePage.aspx + MainProfilePage.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage).
// Stateless page - user identity read from session (no server affinity required).

namespace Tour_Management.Pages.User
{
    public class MainProfileModel : PageModel
    {
        private readonly ILogger<MainProfileModel> _logger;

        public MainProfileModel(ILogger<MainProfileModel> logger)
        {
            _logger = logger;
        }

        public string UserEmail { get; set; } = string.Empty;

        public void OnGet()
        {
            UserEmail = HttpContext.Session.GetString("UserEmail") ?? "Guest";
            _logger.LogInformation("User '{UserEmail}' accessed main profile.", UserEmail);
        }
    }
}
