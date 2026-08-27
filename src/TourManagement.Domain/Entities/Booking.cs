namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour booking made by a user.
/// </summary>
public class Booking
{
    /// <summary>Gets or sets the unique identifier for the booking.</summary>
    public int BookingId { get; set; }

    /// <summary>Gets or sets the name of the tour booked.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the place/destination of the tour.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the email of the user who made the booking.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name of the person who booked.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the date the booking was created.</summary>
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    /// <summary>Gets or sets whether the booking is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>Gets or sets the optional foreign key to the Tour entity.</summary>
    public int? TourId { get; set; }

    /// <summary>Navigation property to the associated tour.</summary>
    public Tour? Tour { get; set; }
}
