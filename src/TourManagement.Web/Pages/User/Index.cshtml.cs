using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.User;

/// <summary>
/// Page model for the admin user management page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IUserService userService, ILogger<IndexModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public IEnumerable<UserProfileViewModel> Users { get; set; } = Enumerable.Empty<UserProfileViewModel>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

        try
        {
            var dtos = await _userService.GetAllAsync(cancellationToken);
            Users = dtos.Select(u => new UserProfileViewModel
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
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users");
            return RedirectToPage("/Admin/Dashboard");
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(string email, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/Admin/Login");

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
