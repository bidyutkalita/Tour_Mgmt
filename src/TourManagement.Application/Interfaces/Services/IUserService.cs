using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces.Services;

/// <summary>
/// Service interface for UserInfo business operations.
/// </summary>
public interface IUserService
{
    /// <summary>Gets all active users.</summary>
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a user by email.</summary>
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Registers a new user.</summary>
    Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user.</summary>
    Task<UserDto?> UpdateAsync(string email, UserUpdateDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user by email.</summary>
    Task<bool> DeleteAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Validates user login credentials.</summary>
    Task<UserDto?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Searches users by name or email.</summary>
    Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
