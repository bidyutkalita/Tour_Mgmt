using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for UserInfo business operations.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="UserService"/>.
    /// </summary>
    public UserService(IUserRepository userRepository, IMapper mapper, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            return await _userRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email {Email}", email);
            return await _userRepository.GetByEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo> CreateAsync(UserInfo user, string plainPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating user with email {Email}", user.Email);

            if (await _userRepository.ExistsAsync(user.Email, cancellationToken))
            {
                throw new DuplicateEntityException(nameof(UserInfo), user.Email);
            }

            // Hash password before storing
            user.Password = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User created successfully with email {Email}", user.Email);
            return user;
        }
        catch (DuplicateEntityException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with email {Email}", user.Email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo> UpdateAsync(string email, UserInfo updatedUser, string? newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with email {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken)
                ?? throw new NotFoundException(nameof(UserInfo), email);

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.Gender = updatedUser.Gender;
            user.Dob = updatedUser.Dob;
            user.Street = updatedUser.Street;
            user.City = updatedUser.City;
            user.State = updatedUser.State;

            if (!string.IsNullOrEmpty(newPassword))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            await _userRepository.UpdateAsync(user, cancellationToken);
            _logger.LogInformation("User updated successfully with email {Email}", email);
            return user;
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with email {Email}", email);
            if (!await _userRepository.ExistsAsync(email, cancellationToken))
            {
                throw new NotFoundException(nameof(UserInfo), email);
            }
            await _userRepository.DeleteAsync(email, cancellationToken);
            _logger.LogInformation("User deleted successfully with email {Email}", email);
        }
        catch (NotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<UserInfo?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                return null;
            }

            bool isValid;
            try
            {
                isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            }
            catch
            {
                // Fallback for plain-text passwords (legacy data)
                isValid = user.Password == password;
            }

            return isValid ? user : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email {Email}", email);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term {SearchTerm}", searchTerm);
            return await _userRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term {SearchTerm}", searchTerm);
            throw;
        }
    }

    // DTO-based helper methods for the Web layer
    /// <summary>Gets all users as DTOs.</summary>
    public async Task<IEnumerable<UserDto>> GetAllDtoAsync(CancellationToken cancellationToken = default)
    {
        var users = await GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    /// <summary>Gets a user as DTO by email.</summary>
    public async Task<UserDto?> GetDtoByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await GetByEmailAsync(email, cancellationToken);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    /// <summary>Creates a user from DTO.</summary>
    public async Task<UserDto> CreateFromDtoAsync(UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        var user = _mapper.Map<UserInfo>(dto);
        var created = await CreateAsync(user, dto.Password, cancellationToken);
        return _mapper.Map<UserDto>(created);
    }

    /// <summary>Updates a user from DTO.</summary>
    public async Task<UserDto> UpdateFromDtoAsync(string email, UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var updatedUser = _mapper.Map<UserInfo>(dto);
        var updated = await UpdateAsync(email, updatedUser, dto.Password, cancellationToken);
        return _mapper.Map<UserDto>(updated);
    }

    /// <summary>Validates login and returns DTO.</summary>
    public async Task<UserDto?> ValidateLoginDtoAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await ValidateLoginAsync(email, password, cancellationToken);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    /// <summary>Searches users and returns DTOs.</summary>
    public async Task<IEnumerable<UserDto>> SearchDtoAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var users = await SearchAsync(searchTerm, cancellationToken);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
}
