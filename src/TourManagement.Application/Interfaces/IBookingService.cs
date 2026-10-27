using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

/// <summary>
/// Service interface for Booking business operations.
/// </summary>
public interface IBookingService
{
    /// <summary>Gets all bookings.</summary>
    Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a booking by its identifier.</summary>
    Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Gets all bookings for a specific user.</summary>
    Task<IEnumerable<BookingDto>> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Creates a new booking.</summary>
    Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a booking by its identifier.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
