using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for UserInfo business operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    /// <summary>Initializes a new instance of UserService.</summary>
    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            return user is null ? null : _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user with email {Email}", dto.Email);
            var exists = await _userRepository.ExistsAsync(dto.Email, cancellationToken);
            if (exists)
                throw new DuplicateEntityException("User", dto.Email);

            var user = _mapper.Map<UserInfo>(dto);
            // Hash password before storing
            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var created = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User created successfully with email {Email}", created.Email);
            return _mapper.Map<UserDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email {Email}", dto.Email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserDto?> UpdateAsync(string email, UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with email {Email}", email);
            var existing = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existing is null)
            {
                _logger.LogWarning("User with email {Email} not found for update", email);
                return null;
            }
            _mapper.Map(dto, existing);
            var updated = await _userRepository.UpdateAsync(existing, cancellationToken);
            _logger.LogInformation("User with email {Email} updated successfully", email);
            return _mapper.Map<UserDto>(updated);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with email {Email}", email);
            var result = await _userRepository.DeleteAsync(email, cancellationToken);
            if (result)
                _logger.LogInformation("User with email {Email} deleted successfully", email);
            else
                _logger.LogWarning("User with email {Email} not found for deletion", email);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserDto?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user is null || !user.IsActive)
                return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isValid)
                return null;

            _logger.LogInformation("Login validated successfully for email {Email}", email);
            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            var users = await _userRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}
