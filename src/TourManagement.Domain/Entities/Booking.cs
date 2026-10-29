namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour booking made by a user.
/// </summary>
public class Booking
{
    /// <summary>Gets or sets the unique booking identifier.</summary>
    public int BookingId { get; set; }

    /// <summary>Gets or sets the name of the booked tour.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the place/destination of the booked tour.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the email of the user who made the booking.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the first name of the user who made the booking.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the date the booking was created.</summary>
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;

    /// <summary>Navigation property: the user who made this booking.</summary>
    public UserInfo? User { get; set; }
}
