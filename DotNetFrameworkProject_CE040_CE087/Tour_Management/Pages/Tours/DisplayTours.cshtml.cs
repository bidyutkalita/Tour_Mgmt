using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tour_Management.Data;
using Tour_Management.Models;

// Replaces DisplayTours.aspx + DisplayTours.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage).
// Uses EF Core DbContext with Azure SQL connection resiliency (cr-dotnet-0013).

namespace Tour_Management.Pages.Tours
{
    public class DisplayToursModel : PageModel
    {
        private readonly TourManagementDbContext _dbContext;
        private readonly ILogger<DisplayToursModel> _logger;

        public DisplayToursModel(TourManagementDbContext dbContext, ILogger<DisplayToursModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IList<Tour> Tours { get; set; } = new List<Tour>();

        /// <summary>
        /// Loads all tours using EF Core with Azure SQL connection resiliency.
        /// Replaces direct SqlConnection + SqlCommand pattern (cr-dotnet-0013).
        /// </summary>
        public async Task OnGetAsync()
        {
            try
            {
                Tours = await _dbContext.Tours.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tours.");
                Tours = new List<Tour>();
            }
        }
    }
}
