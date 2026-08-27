using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for the tour details page.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DetailsModel> _logger;

    /// <summary>Gets the tour details to display.</summary>
    public TourDetailsViewModel? Tour { get; private set; }

    /// <summary>Initializes a new instance of DetailsModel.</summary>
    public DetailsModel(ITourService tourService, ILogger<DetailsModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the tour details page.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (tour is null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                return NotFound();
            }

            Tour = new TourDetailsViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                Pic = tour.Pic,
                CreatedDate = tour.CreatedDate,
                IsActive = tour.IsActive
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour details for ID {TourId}", id);
            return RedirectToPage("Index");
        }
    }
}
