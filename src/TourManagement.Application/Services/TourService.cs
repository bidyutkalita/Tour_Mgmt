using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Tour business operations.
/// </summary>
public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TourService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="TourService"/>.
    /// </summary>
    public TourService(ITourRepository tourRepository, IMapper mapper, ILogger<TourService> logger)
    {
        _tourRepository = tourRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all tours");
            return await _tourRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tours");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving tour with ID {TourId}", id);
            return await _tourRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Tour> CreateAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating tour with name {TourName}", tour.TourName);
            await _tourRepository.AddAsync(tour, cancellationToken);
            _logger.LogInformation("Tour created successfully with ID {TourId}", tour.TourId);
            return tour;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour with name {TourName}", tour.TourName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Tour> UpdateAsync(int id, Tour updatedTour, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating tour with ID {TourId}", id);
            var tour = await _tourRepository.GetByIdAsync(id, cancellationToken)
                ?? throw new NotFoundException(nameof(Tour), id);

            tour.TourName = updatedTour.TourName;
            tour.Place = updatedTour.Place;
            tour.Days = updatedTour.Days;
            tour.Price = updatedTour.Price;
            tour.Locations = updatedTour.Locations;
            tour.TourInfo = updatedTour.TourInfo;
            if (updatedTour.Pic != null)
            {
                tour.Pic = updatedTour.Pic;
            }

            await _tourRepository.UpdateAsync(tour, cancellationToken);
            _logger.LogInformation("Tour updated successfully with ID {TourId}", id);
            return tour;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with ID {TourId}", id);
            if (!await _tourRepository.ExistsAsync(id, cancellationToken))
            {
                throw new NotFoundException(nameof(Tour), id);
            }
            await _tourRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Tour deleted successfully with ID {TourId}", id);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", id);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching tours with term {SearchTerm}", searchTerm);
            return await _tourRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tours with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    // DTO-based helper methods for the Web layer
    /// <summary>Gets all tours as DTOs.</summary>
    public async Task<IEnumerable<TourDto>> GetAllDtoAsync(CancellationToken cancellationToken = default)
    {
        var tours = await GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TourDto>>(tours);
    }

    /// <summary>Gets a tour as DTO by ID.</summary>
    public async Task<TourDto?> GetDtoByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tour = await GetByIdAsync(id, cancellationToken);
        return tour == null ? null : _mapper.Map<TourDto>(tour);
    }

    /// <summary>Creates a tour from DTO.</summary>
    public async Task<TourDto> CreateFromDtoAsync(TourCreateDto dto, CancellationToken cancellationToken = default)
    {
        var tour = _mapper.Map<Tour>(dto);
        var created = await CreateAsync(tour, cancellationToken);
        return _mapper.Map<TourDto>(created);
    }

    /// <summary>Updates a tour from DTO.</summary>
    public async Task<TourDto> UpdateFromDtoAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var updatedTour = _mapper.Map<Tour>(dto);
        var updated = await UpdateAsync(id, updatedTour, cancellationToken);
        return _mapper.Map<TourDto>(updated);
    }

    /// <summary>Searches tours and returns DTOs.</summary>
    public async Task<IEnumerable<TourDto>> SearchDtoAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var tours = await SearchAsync(searchTerm, cancellationToken);
        return _mapper.Map<IEnumerable<TourDto>>(tours);
    }
}
