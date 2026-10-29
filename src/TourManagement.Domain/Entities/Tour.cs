namespace TourManagement.Domain.Entities;

/// <summary>
/// Represents a tour package offered by the system.
/// </summary>
public class Tour
{
    /// <summary>Gets or sets the unique tour identifier.</summary>
    public int TourId { get; set; }

    /// <summary>Gets or sets the name of the tour.</summary>
    public string TourName { get; set; } = string.Empty;

    /// <summary>Gets or sets the main place/destination of the tour.</summary>
    public string Place { get; set; } = string.Empty;

    /// <summary>Gets or sets the number of days for the tour.</summary>
    public int Days { get; set; }

    /// <summary>Gets or sets the price of the tour.</summary>
    public decimal Price { get; set; }

    /// <summary>Gets or sets the locations covered in the tour.</summary>
    public string Locations { get; set; } = string.Empty;

    /// <summary>Gets or sets detailed information about the tour.</summary>
    public string TourInfo { get; set; } = string.Empty;

    /// <summary>Gets or sets the filename of the tour picture.</summary>
    public string? Pic { get; set; }

    /// <summary>Navigation property: bookings for this tour.</summary>
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
