using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tour_Management.Data;
using Tour_Management.Models;

// Replaces TourCrud.aspx + TourCrud.aspx.cs (Web Forms) with ASP.NET Core Razor Pages.
// Addresses cr-dotnet-0026 (Web Forms Usage) and cr-dotnet-0013 (SqlConnection Direct Usage).
// Uses EF Core DbContext with Azure SQL connection resiliency instead of direct SqlConnection.
// Configuration is read from environment variables / appsettings.json (cr-dotnet-0010).

namespace Tour_Management.Pages.Tours
{
    public class TourCrudModel : PageModel
    {
        private readonly TourManagementDbContext _dbContext;
        private readonly ILogger<TourCrudModel> _logger;

        public TourCrudModel(TourManagementDbContext dbContext, ILogger<TourCrudModel> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public IList<Tour> Tours { get; set; } = new List<Tour>();
        public string? StatusMessage { get; set; }
        public bool IsSuccess { get; set; }

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
                _logger.LogError(ex, "Error loading tours for CRUD.");
                Tours = new List<Tour>();
                StatusMessage = "Error loading tours.";
                IsSuccess = false;
            }
        }

        /// <summary>
        /// Handles tour deletion using EF Core (cr-dotnet-0013).
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var tour = await _dbContext.Tours.FindAsync(id);
                if (tour != null)
                {
                    _dbContext.Tours.Remove(tour);
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation("Tour with ID {TourId} deleted.", id);
                    StatusMessage = "Tour deleted successfully.";
                    IsSuccess = true;
                }
                else
                {
                    StatusMessage = "Tour not found.";
                    IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tour with ID {TourId}.", id);
                StatusMessage = "Error deleting tour.";
                IsSuccess = false;
            }

            Tours = await _dbContext.Tours.AsNoTracking().ToListAsync();
            return Page();
        }
    }
}
