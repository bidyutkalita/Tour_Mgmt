using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.ViewModels;

/// <summary>
/// ViewModel for displaying a booking.
/// </summary>
public class BookingViewModel
{
    public int BookingId { get; set; }

    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Display(Name = "City")]
    public string Place { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Name")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Booking Date")]
    public DateTime BookingDate { get; set; }
}

/// <summary>
/// ViewModel for creating a new booking.
/// </summary>
public class BookingCreateViewModel
{
    [Required(ErrorMessage = "Your name is required")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
    [Display(Name = "Your Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Your city is required")]
    [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
    [Display(Name = "Your City")]
    public string Place { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tour name is required")]
    [StringLength(100, ErrorMessage = "Tour name cannot exceed 100 characters")]
    [Display(Name = "Tour Name")]
    public string TourName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mobile number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    [Display(Name = "Mobile Number")]
    public string Email { get; set; } = string.Empty;

    public int? TourId { get; set; }
}
