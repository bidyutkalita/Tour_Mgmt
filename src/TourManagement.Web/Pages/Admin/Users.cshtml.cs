using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin users management page.
/// </summary>
[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersModel> _logger;

    /// <summary>Gets the list of all users.</summary>
    public IEnumerable<UserListViewModel> Users { get; private set; } = Enumerable.Empty<UserListViewModel>();

    /// <summary>Initializes a new instance of UsersModel.</summary>
    public UsersModel(IUserService userService, ILogger<UsersModel> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the admin users page.</summary>
    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _userService.GetAllAsync(cancellationToken);
            Users = users.Select(u => new UserListViewModel
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Gender = u.Gender,
                City = u.City,
                State = u.State,
                IsActive = u.IsActive
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading all users for admin");
        }
    }
}
