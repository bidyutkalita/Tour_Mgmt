using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for the tour details page.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly TourService _tourService;
    private readonly ILogger<DetailsModel> _logger;

    /// <summary>Gets the tour to display.</summary>
    public TourViewModel? Tour { get; private set; }

    /// <summary>
    /// Initializes a new instance of <see cref="DetailsModel"/>.
    /// </summary>
    public DetailsModel(TourService tourService, ILogger<DetailsModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>
    /// Handles GET requests for the tour details page.
    /// </summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tour = await _tourService.GetDtoByIdAsync(id, cancellationToken);
            if (tour == null)
            {
                return NotFound();
            }

            Tour = new TourViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                Pic = tour.Pic
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour details for ID {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading tour details.";
            return RedirectToPage("./Index");
        }
    }
}
