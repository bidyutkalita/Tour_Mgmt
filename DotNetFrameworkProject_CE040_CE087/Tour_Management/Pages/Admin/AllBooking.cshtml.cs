using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tour_Management.Data;
using Tour_Management.Models;

// Replaces allbooking.aspx + allbooking.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage).
// Uses EF Core DbContext with Azure SQL connection resiliency (cr-dotnet-0013).

namespace Tour_Management.Pages.Admin
{
    public class AllBookingModel : PageModel
    {
        private readonly TourManagementDbContext _dbContext;
        private readonly ILogger<AllBookingModel> _logger;

        public AllBookingModel(TourManagementDbContext dbContext, ILogger<AllBookingModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IList<Booking> Bookings { get; set; } = new List<Booking>();

        /// <summary>
        /// Loads all bookings using EF Core with Azure SQL connection resiliency.
        /// </summary>
        public async Task OnGetAsync()
        {
            try
            {
                Bookings = await _dbContext.Bookings.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading all bookings.");
                Bookings = new List<Booking>();
            }
        }
    }
}
