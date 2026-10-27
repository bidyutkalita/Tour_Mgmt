using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for managing tours (admin).
/// </summary>
public class ManageModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<ManageModel> _logger;

    /// <summary>Initializes a new instance of ManageModel.</summary>
    public ManageModel(ITourService tourService, ILogger<ManageModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Gets or sets the list of tours.</summary>
    public IEnumerable<TourDto> Tours { get; set; } = Enumerable.Empty<TourDto>();

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        try
        {
            Tours = await _tourService.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours for management");
        }

        return Page();
    }

    /// <summary>Handles POST requests for deleting a tour.</summary>
    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        try
        {
            await _tourService.DeleteAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Tour deleted successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", id);
            TempData["ErrorMessage"] = "Error deleting tour.";
        }

        return RedirectToPage();
    }
}
