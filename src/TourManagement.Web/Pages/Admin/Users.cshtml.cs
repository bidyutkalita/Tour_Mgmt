using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for admin management of users.
/// </summary>
[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersModel> _logger;

    public IEnumerable<UserDto> Users { get; private set; } = Enumerable.Empty<UserDto>();

    public UsersModel(IUserService userService, ILogger<UsersModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Users = await _userService.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users for admin");
            Users = Enumerable.Empty<UserDto>();
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            await _userService.DeleteAsync(email, cancellationToken);
            TempData["SuccessMessage"] = $"User '{email}' deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {Email}", email);
            TempData["ErrorMessage"] = "An error occurred while deleting the user.";
        }
        return RedirectToPage();
    }
}
