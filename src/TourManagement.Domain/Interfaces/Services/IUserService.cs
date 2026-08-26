using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for UserInfo business operations (domain-level).
/// </summary>
public interface IUserService
{
    /// <summary>Gets all users asynchronously.</summary>
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Gets a user by email asynchronously.</summary>
    Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Creates a new user asynchronously.</summary>
    Task<UserInfo> CreateAsync(UserInfo user, string plainPassword, CancellationToken cancellationToken = default);

    /// <summary>Updates an existing user asynchronously.</summary>
    Task<UserInfo> UpdateAsync(string email, UserInfo updatedUser, string? newPassword, CancellationToken cancellationToken = default);

    /// <summary>Deletes a user by email asynchronously.</summary>
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>Validates user login credentials asynchronously.</summary>
    Task<UserInfo?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Searches users by name or email asynchronously.</summary>
    Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
