using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for deleting a tour.
/// </summary>
public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    /// <summary>Gets the tour to be deleted.</summary>
    public TourDeleteViewModel? Tour { get; private set; }

    /// <summary>Initializes a new instance of DeleteModel.</summary>
    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the delete confirmation page.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (tour is null)
                return NotFound();

            Tour = new TourDeleteViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for delete with ID {TourId}", id);
            return RedirectToPage("Index");
        }
    }

    /// <summary>Handles POST requests to delete a tour.</summary>
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _tourService.DeleteAsync(id, cancellationToken);
            if (!result)
                return NotFound();

            TempData["SuccessMessage"] = "Tour deleted successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the tour.";
            return RedirectToPage("Index");
        }
    }
}
