using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tour_Management.Data;
using Tour_Management.Models;

// Replaces mybooking.aspx + mybooking.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage).
// Uses EF Core DbContext with Azure SQL connection resiliency (cr-dotnet-0013).

namespace Tour_Management.Pages.User
{
    public class MyBookingModel : PageModel
    {
        private readonly TourManagementDbContext _dbContext;
        private readonly ILogger<MyBookingModel> _logger;

        public MyBookingModel(TourManagementDbContext dbContext, ILogger<MyBookingModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IList<Booking> Bookings { get; set; } = new List<Booking>();

        /// <summary>
        /// Loads bookings for the current user using EF Core with Azure SQL connection resiliency.
        /// </summary>
        public async Task OnGetAsync()
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");

                if (!string.IsNullOrEmpty(userEmail))
                {
                    Bookings = await _dbContext.Bookings
                        .AsNoTracking()
                        .Where(b => b.Email == userEmail)
                        .ToListAsync();
                }
                else
                {
                    Bookings = new List<Booking>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading bookings.");
                Bookings = new List<Booking>();
            }
        }
    }
}
