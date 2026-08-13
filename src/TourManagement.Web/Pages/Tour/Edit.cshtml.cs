using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Exceptions;
using TourManagement.Application.Interfaces;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tour;

/// <summary>
/// Page model for editing an existing tour.
/// </summary>
public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public TourFormViewModel TourForm { get; set; } = new();

    public int TourId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/User/Login");

        try
        {
            var dto = await _tourService.GetByIdAsync(id, cancellationToken);
            if (dto is null)
                return NotFound();

            TourId = id;
            TourForm = new TourFormViewModel
            {
                TourName = dto.TourName,
                Place = dto.Place,
                Days = dto.Days,
                Price = dto.Price,
                Locations = dto.Locations,
                TourInfo = dto.TourInfo,
                Pic = dto.Pic
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit, ID {TourId}", id);
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
            return RedirectToPage("/User/Login");

        TourId = id;

        if (!ModelState.IsValid)
            return Page();

        try
        {
            string? picFileName = TourForm.Pic;

            if (TourForm.PicFile != null && TourForm.PicFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                Directory.CreateDirectory(uploadsFolder);
                picFileName = Guid.NewGuid().ToString() + Path.GetExtension(TourForm.PicFile.FileName);
                var filePath = Path.Combine(uploadsFolder, picFileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await TourForm.PicFile.CopyToAsync(stream, cancellationToken);
            }

            var dto = new TourUpdateDto
            {
                TourName = TourForm.TourName,
                Place = TourForm.Place,
                Days = TourForm.Days,
                Price = TourForm.Price,
                Locations = TourForm.Locations,
                TourInfo = TourForm.TourInfo,
                Pic = picFileName
            };

            await _tourService.UpdateAsync(id, dto, cancellationToken);
            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("Details", new { id });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour.");
            return Page();
        }
    }
}
