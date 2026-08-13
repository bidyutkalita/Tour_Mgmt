using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tour;

/// <summary>
/// Page model for the Tour listing page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public IEnumerable<TourViewModel> Tours { get; set; } = Enumerable.Empty<TourViewModel>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public bool IsAdmin => HttpContext.Session.GetString("IsAdmin") == "true";

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<TourDto> tours;
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                tours = await _tourService.SearchAsync(SearchTerm, cancellationToken);
            }
            else
            {
                tours = await _tourService.GetAllAsync(cancellationToken);
            }

            Tours = tours.Select(t => new TourViewModel
            {
                TourId = t.TourId,
                TourName = t.TourName,
                Place = t.Place,
                Days = t.Days,
                Price = t.Price,
                Locations = t.Locations,
                TourInfo = t.TourInfo,
                Pic = t.Pic
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours");
            ModelState.AddModelError(string.Empty, "An error occurred while loading tours.");
        }
    }
}
