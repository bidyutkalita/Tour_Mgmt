using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for editing an existing tour.
/// </summary>
public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    /// <summary>Gets or sets the input view model bound from the form.</summary>
    [BindProperty]
    public TourEditViewModel Input { get; set; } = new();

    /// <summary>Initializes a new instance of EditModel.</summary>
    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>Handles GET requests for the edit tour page.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (tour is null)
                return NotFound();

            Input = new TourEditViewModel
            {
                TourId = tour.TourId,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                ExistingPic = tour.Pic,
                IsActive = tour.IsActive
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit with ID {TourId}", id);
            return RedirectToPage("Index");
        }
    }

    /// <summary>Handles POST requests to update an existing tour.</summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            string? picFileName = Input.ExistingPic;
            if (Input.PicFile != null && Input.PicFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);
                picFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Input.PicFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, picFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await Input.PicFile.CopyToAsync(stream, cancellationToken);
            }

            var dto = new TourUpdateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                Pic = picFileName,
                IsActive = Input.IsActive
            };

            var updated = await _tourService.UpdateAsync(Input.TourId, dto, cancellationToken);
            if (updated is null)
                return NotFound();

            TempData["SuccessMessage"] = $"Tour '{updated.TourName}' updated successfully!";
            return RedirectToPage("Details", new { id = Input.TourId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", Input.TourId);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour. Please try again.");
            return Page();
        }
    }
}
