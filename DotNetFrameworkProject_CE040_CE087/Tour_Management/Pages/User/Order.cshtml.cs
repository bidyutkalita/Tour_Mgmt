using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Tour_Management.Data;
using Tour_Management.Models;

// Replaces Order.aspx + Order.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage) and cr-dotnet-0013 (SqlConnection Direct Usage).
// Uses EF Core DbContext with Azure SQL connection resiliency instead of direct SqlConnection.
// Configuration is read from environment variables / appsettings.json (cr-dotnet-0010).

namespace Tour_Management.Pages.User
{
    public class OrderModel : PageModel
    {
        private readonly TourManagementDbContext _dbContext;
        private readonly ILogger<OrderModel> _logger;

        public OrderModel(TourManagementDbContext dbContext, ILogger<OrderModel> logger)
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
            [MaxLength(200)]
            [Display(Name = "Tour Name")]
            public string TourName { get; set; } = string.Empty;

            [Required]
            [MaxLength(200)]
            public string Place { get; set; } = string.Empty;

            [Required]
            [EmailAddress]
            [MaxLength(200)]
            public string Email { get; set; } = string.Empty;

            [Required]
            [MaxLength(200)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; } = string.Empty;
        }

        public void OnGet(string? tourName, string? place)
        {
            // Pre-populate from query string when navigating from DisplayTours
            if (!string.IsNullOrEmpty(tourName))
                Input.TourName = tourName;
            if (!string.IsNullOrEmpty(place))
                Input.Place = place;
        }

        /// <summary>
        /// Handles booking creation using EF Core with Azure SQL connection resiliency.
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
                var booking = new Booking
                {
                    TourName = Input.TourName,
                    Place = Input.Place,
                    Email = Input.Email,
                    FirstName = Input.FirstName
                };

                _dbContext.Bookings.Add(booking);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Booking created for tour '{TourName}' by '{Email}'.", Input.TourName, Input.Email);
                return RedirectToPage("/User/MyBooking");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking for tour '{TourName}'.", Input.TourName);
                StatusMessage = "Booking failed. Please try again.";
                IsSuccess = false;
                return Page();
            }
        }
    }
}
