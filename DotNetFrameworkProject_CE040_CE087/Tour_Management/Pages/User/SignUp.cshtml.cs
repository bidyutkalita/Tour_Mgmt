using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Tour_Management.Data;
using Tour_Management.Models;

// Replaces SignUpForm.aspx + SignUpForm.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage) and cr-dotnet-0013 (SqlConnection Direct Usage).
// Uses EF Core DbContext with Azure SQL connection resiliency instead of direct SqlConnection.
// Configuration is read from environment variables / appsettings.json (cr-dotnet-0010).

namespace Tour_Management.Pages.User
{
    public class SignUpModel : PageModel
    {
        private readonly TourManagementDbContext _dbContext;
        private readonly ILogger<SignUpModel> _logger;

        public SignUpModel(TourManagementDbContext dbContext, ILogger<SignUpModel> logger)
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
            [MaxLength(100)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;

            [MaxLength(100)]
            [Display(Name = "Last Name")]
            public string? LastName { get; set; }

            [MaxLength(20)]
            public string? Gender { get; set; }

            [Required]
            [MinLength(6)]
            [MaxLength(200)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [MaxLength(20)]
            [Display(Name = "Date of Birth")]
            public string? Dob { get; set; }

            [MaxLength(200)]
            public string? Street { get; set; }

            [MaxLength(100)]
            public string? City { get; set; }

            [MaxLength(100)]
            public string? State { get; set; }
        }

        public void OnGet()
        {
        }

        /// <summary>
        /// Handles user registration using EF Core with Azure SQL connection resiliency.
        /// Replaces direct SqlConnection + SqlCommand pattern (cr-dotnet-0013).
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Use EF Core DbContext instead of direct SqlConnection (cr-dotnet-0013)
                var user = new UserInfo
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

                _dbContext.UserInfos.Add(user);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("User '{Email}' registered successfully.", Input.Email);
                return RedirectToPage("/User/Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user '{Email}'.", Input.Email);
                StatusMessage = "Registration failed. Please try again.";
                IsSuccess = false;
                return Page();
            }
        }
    }
}
