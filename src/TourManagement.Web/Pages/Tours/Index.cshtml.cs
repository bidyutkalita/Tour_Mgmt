using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for listing all tours.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    /// <summary>Initializes a new instance of IndexModel.</summary>
    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Gets or sets the list of tours.</summary>
    public IEnumerable<TourDto> Tours { get; set; } = Enumerable.Empty<TourDto>();

    /// <summary>Gets or sets the search term.</summary>
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserEmail") == null &&
            HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Users/Login");

        try
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
                Tours = await _tourService.SearchAsync(SearchTerm, cancellationToken);
            else
                Tours = await _tourService.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours list");
        }

        return Page();
    }
}
