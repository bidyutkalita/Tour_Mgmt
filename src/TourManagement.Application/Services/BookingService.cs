using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Booking business operations.
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="BookingService"/>.
    /// </summary>
    public BookingService(IBookingRepository bookingRepository, IMapper mapper, ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings");
            return await _bookingRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving booking with ID {BookingId}", id);
            return await _bookingRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for user {Email}", email);
            return await _bookingRepository.GetByUserEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Booking> CreateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating booking for user {Email}", booking.Email);
            booking.BookingDate = DateTime.UtcNow;
            await _bookingRepository.AddAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created successfully with ID {BookingId}", booking.BookingId);
            return booking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user {Email}", booking.Email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Booking> UpdateAsync(int id, Booking updatedBooking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID {BookingId}", id);
            var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Booking), id);

            booking.TourName = updatedBooking.TourName;
            booking.Place = updatedBooking.Place;
            booking.Email = updatedBooking.Email;
            booking.FirstName = updatedBooking.FirstName;

            await _bookingRepository.UpdateAsync(booking, cancellationToken);
            _logger.LogInformation("Booking updated successfully with ID {BookingId}", id);
            return booking;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID {BookingId}", id);
            if (!await _bookingRepository.ExistsAsync(id, cancellationToken))
            {
                throw new NotFoundException(nameof(Booking), id);
            }
            await _bookingRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Booking deleted successfully with ID {BookingId}", id);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {BookingId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching bookings with term {SearchTerm}", searchTerm);
            return await _bookingRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookings with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    // DTO-based helper methods for the Web layer
    /// <summary>Gets all bookings as DTOs.</summary>
    public async Task<IEnumerable<BookingDto>> GetAllDtoAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    /// <summary>Gets bookings for a user as DTOs.</summary>
    public async Task<IEnumerable<BookingDto>> GetByUserEmailDtoAsync(string email, CancellationToken cancellationToken = default)
    {
        var bookings = await GetByUserEmailAsync(email, cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    /// <summary>Creates a booking from DTO.</summary>
    public async Task<BookingDto> CreateFromDtoAsync(BookingCreateDto dto, CancellationToken cancellationToken = default)
    {
        var booking = _mapper.Map<Booking>(dto);
        var created = await CreateAsync(booking, cancellationToken);
        return _mapper.Map<BookingDto>(created);
    }

    /// <summary>Searches bookings and returns DTOs.</summary>
    public async Task<IEnumerable<BookingDto>> SearchDtoAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var bookings = await SearchAsync(searchTerm, cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }
}
