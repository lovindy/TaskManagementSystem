using TaskManagementSystem.Models;

namespace TaskManagementSystem.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
