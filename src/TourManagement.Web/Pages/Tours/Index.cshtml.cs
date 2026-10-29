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

    public IEnumerable<TourDto> Tours { get; private set; } = Enumerable.Empty<TourDto>();
    public string? SearchTerm { get; private set; }

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync(string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        try
        {
            SearchTerm = searchTerm;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                Tours = await _tourService.SearchAsync(searchTerm, cancellationToken);
            }
            else
            {
                Tours = await _tourService.GetAllAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours list");
            Tours = Enumerable.Empty<TourDto>();
        }
    }
}
