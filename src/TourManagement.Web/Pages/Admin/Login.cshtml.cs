using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin login.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginModel> _logger;

    /// <summary>Initializes a new instance of LoginModel.</summary>
    public LoginModel(IConfiguration configuration, ILogger<LoginModel> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>Gets or sets the admin email.</summary>
    [BindProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the admin password.</summary>
    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets the error message.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Handles GET requests.</summary>
    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("AdminEmail") != null)
            return RedirectToPage("/Admin/Dashboard");
        return Page();
    }

    /// <summary>Handles POST requests for admin login.</summary>
    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        var adminEmail = _configuration["AdminCredentials:Email"] ?? "admin@gmail.com";
        var adminPassword = _configuration["AdminCredentials:Password"] ?? "admin";

        if (Email == adminEmail && Password == adminPassword)
        {
            HttpContext.Session.SetString("AdminEmail", Email);
            _logger.LogInformation("Admin logged in successfully");
            return RedirectToPage("/Admin/Dashboard");
        }

        ErrorMessage = "Invalid admin credentials.";
        return Page();
    }
}
