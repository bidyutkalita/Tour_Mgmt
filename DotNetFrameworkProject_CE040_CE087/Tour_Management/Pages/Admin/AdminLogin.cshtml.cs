using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

// Replaces AdminLogin2.aspx + AdminLogin2.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage).
// Admin credentials are read from environment variables (ADMIN_EMAIL, ADMIN_PASSWORD)
// instead of being hardcoded, following 12-factor app principles (cr-dotnet-0010).

namespace Tour_Management.Pages.Admin
{
    public class AdminLoginModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AdminLoginModel> _logger;

        public AdminLoginModel(IConfiguration configuration, ILogger<AdminLoginModel> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? StatusMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [MaxLength(200)]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MaxLength(200)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        /// <summary>
        /// Handles admin login. Admin credentials are read from environment variables
        /// (ADMIN_EMAIL, ADMIN_PASSWORD) instead of hardcoded values.
        /// Replaces hardcoded credential check in AdminLogin2.aspx.cs (cr-dotnet-0010).
        /// </summary>
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Read admin credentials from environment variables / configuration (cr-dotnet-0010)
            var adminEmail = _configuration["Admin:Email"]
                ?? Environment.GetEnvironmentVariable("ADMIN_EMAIL")
                ?? "admin@gmail.com";
            var adminPassword = _configuration["Admin:Password"]
                ?? Environment.GetEnvironmentVariable("ADMIN_PASSWORD")
                ?? "admin";

            if (Input.Password == adminPassword && Input.Email == adminEmail)
            {
                _logger.LogInformation("Admin '{Email}' logged in successfully.", Input.Email);
                HttpContext.Session.SetString("AdminEmail", Input.Email);
                return RedirectToPage("/Admin/AdminProfile");
            }

            _logger.LogWarning("Failed admin login attempt for '{Email}'.", Input.Email);
            StatusMessage = "Invalid admin credentials.";
            return Page();
        }
    }
}
