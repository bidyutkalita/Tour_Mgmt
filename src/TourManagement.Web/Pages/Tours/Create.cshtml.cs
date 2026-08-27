using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for creating a new tour.
/// </summary>
public class CreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CreateModel> _logger;

    /// <summary>Gets or sets the input view model bound from the form.</summary>
    [BindProperty]
    public TourCreateViewModel Input { get; set; } = new();

    /// <summary>Initializes a new instance of CreateModel.</summary>
    public CreateModel(ITourService tourService, IWebHostEnvironment environment, ILogger<CreateModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the create tour page.</summary>
    public IActionResult OnGet()
    {
        return Page();
    }

    /// <summary>Handles POST requests to create a new tour.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            string? picFileName = null;
            if (Input.PicFile != null && Input.PicFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);
                picFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Input.PicFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, picFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await Input.PicFile.CopyToAsync(stream, cancellationToken);
            }

            var dto = new TourCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                Pic = picFileName
            };

            var created = await _tourService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("Tour '{TourName}' created successfully with ID {TourId}", created.TourName, created.TourId);
            TempData["SuccessMessage"] = $"Tour '{created.TourName}' added successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour '{TourName}'", Input.TourName);
            ModelState.AddModelError(string.Empty, "An error occurred while creating the tour. Please try again.");
            return Page();
        }
    }
}
