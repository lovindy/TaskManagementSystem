using System.Data;
using Dapper;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbContext _context;
    private readonly IConfiguration _configuration;

    public UserRepository(IDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<Guid> CreateUserAsync(User user, string password)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@Username", user.Username);
        parameters.Add("@PasswordHash", HashPassword(password));
        parameters.Add("@UserId", dbType: DbType.Guid, direction: ParameterDirection.Output);

        await connection.ExecuteAsync(
            "sp_CreateUser",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return parameters.Get<Guid>("@UserId");
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<User>(
            "sp_GetUserById",
            new { UserId = userId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        using var connection = _context.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<User>(
            "sp_GetUserByUsername",
            new { Username = username },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
    {
        using var connection = _context.CreateConnection();

        var user = await connection.QuerySingleOrDefaultAsync<User>(
            "sp_GetUserByUsername",
            new { Username = username },
            commandType: CommandType.StoredProcedure
        );

        if (user == null) return false;

        return VerifyPassword(password, user.PasswordHash);
    }

    public async Task UpdateUserAsync(User user)
    {
        using var connection = _context.CreateConnection();

        await connection.ExecuteAsync(
            "sp_UpdateUser",
            new
            {
                UserId = user.UserId,
                Username = user.Username,
                IsActive = user.IsActive
            },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<User>(
            "sp_SearchUsers",
            new { SearchTerm = $"%{searchTerm}%" },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        using var connection = _context.CreateConnection();

        var count = await connection.ExecuteScalarAsync<int>(
            "sp_CheckUsernameExists",
            new { Username = username },
            commandType: CommandType.StoredProcedure
        );

        return count > 0;
    }

    private string HashPassword(string password)
    {
        // Using BCrypt for password hashing
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}