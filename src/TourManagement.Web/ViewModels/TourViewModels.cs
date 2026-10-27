using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a tour.
/// </summary>
public class TourViewModel
{
    public int TourId { get; set; }

    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Display(Name = "Days")]
    public int Days { get; set; }

    [Display(Name = "Price")]
    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Price { get; set; }

    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [Display(Name = "Tour Info")]
    public string TourInfo { get; set; } = string.Empty;

    [Display(Name = "Picture")]
    public string? Pic { get; set; }

    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// ViewModel for creating a new tour.
/// </summary>
public class TourCreateViewModel
{
    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(100, ErrorMessage = "Tour name cannot exceed 100 characters")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [StringLength(100, ErrorMessage = "Place cannot exceed 100 characters")]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    [Display(Name = "Days")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be a positive value")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Locations are required")]
    [StringLength(200, ErrorMessage = "Locations cannot exceed 200 characters")]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour info is required")]
    [StringLength(500, ErrorMessage = "Tour info cannot exceed 500 characters")]
    [Display(Name = "Tour Info")]
    public string TourInfo { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for editing an existing tour.
/// </summary>
public class TourEditViewModel
{
    public int TourId { get; set; }

    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(100, ErrorMessage = "Tour name cannot exceed 100 characters")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Place is required")]
    [StringLength(100, ErrorMessage = "Place cannot exceed 100 characters")]
    [Display(Name = "Place")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Number of days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    [Display(Name = "Days")]
    public int Days { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 999999.99, ErrorMessage = "Price must be a positive value")]
    [Display(Name = "Price")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Locations are required")]
    [StringLength(200, ErrorMessage = "Locations cannot exceed 200 characters")]
    [Display(Name = "Locations")]
    public string Locations { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour info is required")]
    [StringLength(500, ErrorMessage = "Tour info cannot exceed 500 characters")]
    [Display(Name = "Tour Info")]
    public string TourInfo { get; set; } = string.Empty;

    public string? Pic { get; set; }
}
