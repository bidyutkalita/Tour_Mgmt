using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Exceptions;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Users;

/// <summary>
/// Page model for user registration.
/// </summary>
public class RegisterModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<RegisterModel> _logger;

    /// <summary>Gets or sets the input view model bound from the form.</summary>
    [BindProperty]
    public UserRegisterViewModel Input { get; set; } = new();

    /// <summary>Initializes a new instance of RegisterModel.</summary>
    public RegisterModel(IUserService userService, ILogger<RegisterModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the registration page.</summary>
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Index");
        return Page();
    }

    /// <summary>Handles POST requests to register a new user.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var dto = new UserCreateDto
            {
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Gender = Input.Gender,
                Password = Input.Password,
                Dob = Input.Dob,
                Street = Input.Street,
                City = Input.City,
                State = Input.State
            };

            var created = await _userService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("User '{Email}' registered successfully", created.Email);
            TempData["SuccessMessage"] = "Registration successful! Please log in.";
            return RedirectToPage("Login");
        }
        catch (DuplicateEntityException)
        {
            ModelState.AddModelError("Input.Email", "An account with this email already exists.");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user '{Email}'", Input.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
            return Page();
        }
    }
}
