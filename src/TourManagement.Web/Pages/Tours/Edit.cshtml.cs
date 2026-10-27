using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Exceptions;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for editing a tour.
/// </summary>
public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    /// <summary>Initializes a new instance of EditModel.</summary>
    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>Gets or sets the edit input model.</summary>
    [BindProperty]
    public TourEditViewModel Input { get; set; } = new();

    /// <summary>Gets or sets the error message.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>Handles GET requests.</summary>
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (tour == null)
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
                Pic = tour.Pic
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit: {TourId}", id);
            return RedirectToPage("/Tours/Manage");
        }
    }

    /// <summary>Handles POST requests for updating a tour.</summary>
    public async Task<IActionResult> OnPostAsync(IFormFile? tourImage, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("AdminEmail") == null)
            return RedirectToPage("/Admin/Login");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            string? picFileName = Input.Pic;

            // Handle file upload
            if (tourImage != null && tourImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);
                picFileName = $"{Guid.NewGuid()}_{Path.GetFileName(tourImage.FileName)}";
                var filePath = Path.Combine(uploadsFolder, picFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await tourImage.CopyToAsync(stream, cancellationToken);
            }

            var dto = new TourUpdateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                Pic = picFileName
            };

            await _tourService.UpdateAsync(Input.TourId, dto, cancellationToken);
            _logger.LogInformation("Tour {TourId} updated successfully", Input.TourId);
            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("/Tours/Manage");
        }
        catch (NotFoundException)
        {
            ErrorMessage = "Tour not found.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour {TourId}", Input.TourId);
            ErrorMessage = "An error occurred while updating the tour.";
            return Page();
        }
    }
}
