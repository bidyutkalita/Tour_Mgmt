using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for deleting a tour.
/// </summary>
public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    /// <summary>Initializes a new instance of DeleteModel.</summary>
    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Gets or sets the tour to delete.</summary>
    public TourDto? Tour { get; set; }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        try
        {
            Tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (Tour == null)
                return NotFound();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for delete: {TourId}", id);
            return RedirectToPage("/Tours/Manage");
        }
    }

    /// <summary>Handles POST requests for confirming deletion.</summary>
    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        try
        {
            await _tourService.DeleteAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Tour deleted successfully.";
            return RedirectToPage("/Tours/Manage");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour {TourId}", id);
            TempData["ErrorMessage"] = "Error deleting tour.";
            return RedirectToPage("/Tours/Manage");
        }
    }
}
