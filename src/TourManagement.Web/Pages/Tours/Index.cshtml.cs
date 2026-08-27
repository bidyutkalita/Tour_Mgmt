using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for the tours list page.
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    /// <summary>Gets the list of tours to display.</summary>
    public IEnumerable<TourListViewModel> Tours { get; private set; } = Enumerable.Empty<TourListViewModel>();

    /// <summary>Gets the current search term.</summary>
    public string? SearchTerm { get; private set; }

    /// <summary>Initializes a new instance of IndexModel.</summary>
    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the tours list page.</summary>
    public async Task OnGetAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        try
        {
            SearchTerm = searchTerm;
            var tours = string.IsNullOrWhiteSpace(searchTerm)
                ? await _tourService.GetAllAsync(cancellationToken)
                : await _tourService.SearchAsync(searchTerm, cancellationToken);

            Tours = tours.Select(t => new TourListViewModel
            {
                TourId = t.TourId,
                TourName = t.TourName,
                Place = t.Place,
                Days = t.Days,
                Price = t.Price,
                Pic = t.Pic,
                IsActive = t.IsActive
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours list");
            Tours = Enumerable.Empty<TourListViewModel>();
        }
    }
}
