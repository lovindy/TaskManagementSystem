using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public interface IUserRepository
{
    Task<Guid> CreateUserAsync(User user, string password);
    Task<User> GetUserByIdAsync(Guid userId);
    Task<User> GetUserByUsernameAsync(string username);
    Task<bool> ValidateUserCredentialsAsync(string username, string password);
    Task UpdateUserAsync(User user);
    Task ChangePasswordAsync(Guid userId, string newPassword);
    Task DeactivateUserAsync(Guid userId);
    Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
    Task<bool> UsernameExistsAsync(string username);
}