using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin login.
/// Admin credentials are stored in configuration (not in the database).
/// </summary>
public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginModel> _logger;

    [BindProperty]
    public AdminLoginViewModel Input { get; set; } = new();

    public LoginModel(IConfiguration configuration, ILogger<LoginModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Validate admin credentials from configuration
            var adminEmail = _configuration["AdminCredentials:Email"] ?? "admin@gmail.com";
            var adminPassword = _configuration["AdminCredentials:Password"] ?? "admin";

            if (Input.Email != adminEmail || Input.Password != adminPassword)
            {
                ModelState.AddModelError(string.Empty, "Invalid admin credentials.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, Input.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(4)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("Admin logged in successfully");
            return RedirectToPage("Dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login");
            ModelState.AddModelError(string.Empty, "An error occurred during login.");
            return Page();
        }
    }
}
