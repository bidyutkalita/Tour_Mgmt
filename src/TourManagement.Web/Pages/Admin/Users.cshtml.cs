using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin users management page.
/// </summary>
public class UsersModel : PageModel
{
    private readonly UserService _userService;
    private readonly ILogger<UsersModel> _logger;

    /// <summary>Gets the list of users.</summary>
    public IEnumerable<UserProfileViewModel> Users { get; private set; } = new List<UserProfileViewModel>();

    /// <summary>Gets or sets the search term.</summary>
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="UsersModel"/>.
    /// </summary>
    public UsersModel(UserService userService, ILogger<UsersModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the users management page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            var users = string.IsNullOrWhiteSpace(SearchTerm)
                ? await _userService.GetAllAsync(cancellationToken)
                : await _userService.SearchAsync(SearchTerm, cancellationToken);

            Users = users.Select(u => new UserProfileViewModel
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Gender = u.Gender,
                Dob = u.Dob,
                Street = u.Street,
                City = u.City,
                State = u.State
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users list");
            TempData["ErrorMessage"] = "An error occurred while loading users.";
        }

        return Page();
    }

    /// <summary>
    /// Handles POST requests to delete a user.
    /// </summary>
    public async Task<IActionResult> OnPostDeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Admin/Login");
        }

        try
        {
            await _userService.DeleteAsync(email, cancellationToken);
            TempData["SuccessMessage"] = $"User {email} deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Email}", email);
            TempData["ErrorMessage"] = "An error occurred while deleting the user.";
        }

        return RedirectToPage();
    }
}
