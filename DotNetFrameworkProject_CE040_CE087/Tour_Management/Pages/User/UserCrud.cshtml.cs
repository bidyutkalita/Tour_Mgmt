using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tour_Management.Data;
using Tour_Management.Models;

// Replaces usercrud.aspx + usercrud.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage).
// Uses EF Core DbContext with Azure SQL connection resiliency (cr-dotnet-0013).

namespace Tour_Management.Pages.User
{
    public class UserCrudModel : PageModel
    {
        private readonly TourManagementDbContext _dbContext;
        private readonly ILogger<UserCrudModel> _logger;

        public UserCrudModel(TourManagementDbContext dbContext, ILogger<UserCrudModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IList<UserInfo> Users { get; set; } = new List<UserInfo>();
        public string? StatusMessage { get; set; }
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Loads all users using EF Core with Azure SQL connection resiliency.
        /// </summary>
        public async Task OnGetAsync()
        {
            try
            {
                Users = await _dbContext.UserInfos.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading users.");
                Users = new List<UserInfo>();
                StatusMessage = "Error loading users.";
                IsSuccess = false;
            }
        }

        /// <summary>
        /// Handles user deletion using EF Core (cr-dotnet-0013).
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var user = await _dbContext.UserInfos.FindAsync(id);
                if (user != null)
                {
                    _dbContext.UserInfos.Remove(user);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("User with ID {UserId} deleted.", id);
                    StatusMessage = "User deleted successfully.";
                    IsSuccess = true;
                }
                else
                {
                    StatusMessage = "User not found.";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}.", id);
                StatusMessage = "Error deleting user.";
                IsSuccess = false;
            }

            Users = await _dbContext.UserInfos.AsNoTracking().ToListAsync();
            return Page();
        }
    }
}
