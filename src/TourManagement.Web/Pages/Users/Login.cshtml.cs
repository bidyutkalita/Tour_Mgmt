using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user login.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<LoginModel> _logger;

    /// <summary>Gets or sets the input view model bound from the form.</summary>
    [BindProperty]
    public UserLoginViewModel Input { get; set; } = new();

    /// <summary>Initializes a new instance of LoginModel.</summary>
    public LoginModel(IUserService userService, ILogger<LoginModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the login page.</summary>
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");
        return Page();
    }

    /// <summary>Handles POST requests to authenticate a user.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var user = await _userService.ValidateLoginAsync(Input.Email, Input.Password, cancellationToken);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            _logger.LogInformation("User '{Email}' logged in successfully", user.Email);
            return RedirectToPage("/Users/Profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for '{Email}'", Input.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return Page();
        }
    }
}
