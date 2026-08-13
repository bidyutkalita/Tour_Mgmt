using Microsoft.AspNetCore.Mvc.RazorPages;

// Replaces AdminProfile.aspx + AdminProfile.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage).
// Stateless page - admin identity read from session (no server affinity required).

namespace Tour_Management.Pages.Admin
{
    public class AdminProfileModel : PageModel
    {
        private readonly ILogger<AdminProfileModel> _logger;

        public AdminProfileModel(ILogger<AdminProfileModel> logger)
        {
            _logger = logger;
        }

        public string AdminEmail { get; set; } = string.Empty;

        public void OnGet()
        {
            AdminEmail = HttpContext.Session.GetString("AdminEmail") ?? "Admin";
            _logger.LogInformation("Admin '{AdminEmail}' accessed admin profile.", AdminEmail);
        }
    }
}
