using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for tour details.
/// </summary>
public class DetailsModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DetailsModel> _logger;

    /// <summary>Initializes a new instance of DetailsModel.</summary>
    public DetailsModel(ITourService tourService, ILogger<DetailsModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Gets or sets the tour details.</summary>
    public TourDto? Tour { get; set; }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserEmail") == null &&
            HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Users/Login");

        try
        {
            Tour = await _tourService.GetByIdAsync(id, cancellationToken);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour details for ID {TourId}", id);
            return RedirectToPage("/Tours/Index");
        }
    }
}
