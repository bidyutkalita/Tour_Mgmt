using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Tour_Management.Data;

// Replaces userlogin.aspx + userlogin.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage) and cr-dotnet-0013 (SqlConnection Direct Usage).
// Uses EF Core DbContext with Azure SQL connection resiliency instead of direct SqlConnection.
// Configuration is read from environment variables / appsettings.json (cr-dotnet-0010).

namespace Tour_Management.Pages.User
{
    public class LoginModel : PageModel
    {
        private readonly TourManagementDbContext _dbContext;
        private readonly ILogger<LoginModel> _logger;

        public LoginModel(TourManagementDbContext dbContext, ILogger<LoginModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? StatusMessage { get; set; }
        public bool IsSuccess { get; set; }

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
        /// Handles user login using EF Core with Azure SQL connection resiliency.
        /// Replaces direct SqlConnection + SqlCommand with raw SQL string concatenation (cr-dotnet-0013).
        /// Uses parameterized EF Core query to prevent SQL injection.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Use EF Core DbContext with parameterized query instead of direct SqlConnection (cr-dotnet-0013)
                var user = await _dbContext.UserInfos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email == Input.Email && u.Password == Input.Password);

                if (user != null)
                {
                    _logger.LogInformation("User '{Email}' logged in successfully.", Input.Email);
                    // Store user email in session (stateless-compatible)
                    HttpContext.Session.SetString("UserEmail", Input.Email);
                    return RedirectToPage("/User/MainProfile");
                }
                else
                {
                    _logger.LogWarning("Failed login attempt for '{Email}'.", Input.Email);
                    StatusMessage = "Password is not correct";
                    IsSuccess = false;
                    return Page();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for '{Email}'.", Input.Email);
                StatusMessage = "An error occurred during login. Please try again.";
                IsSuccess = false;
                return Page();
            }
        }
    }
}
