using System.Data;
using Dapper;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public class BoardMemberRepository : IBoardMemberRepository
{
    private readonly IDbContext _context;

    public BoardMemberRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BoardMember>> GetBoardMembersAsync(Guid boardId)
    {
        using var connection = _context.CreateConnection();
        
        var result = await connection.QueryAsync<BoardMember, User, BoardMember>(
            "sp_GetBoardMembers",
            (member, user) => {
                member.User = user;
                return member;
            },
            new { BoardId = boardId },
            splitOn: "Username",
            commandType: CommandType.StoredProcedure
        );

        return result;
    }

    public async Task AddBoardMemberAsync(BoardMember member)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            INSERT INTO BoardMembers (BoardId, UserId, Role)
            VALUES (@BoardId, @UserId, @Role)";

        await connection.ExecuteAsync(sql, member);
    }

    public async Task UpdateBoardMemberRoleAsync(Guid boardId, Guid userId, string newRole)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            UPDATE BoardMembers 
            SET Role = @Role
            WHERE BoardId = @BoardId AND UserId = @UserId";

        await connection.ExecuteAsync(sql, new { BoardId = boardId, UserId = userId, Role = newRole });
    }

    public async Task RemoveBoardMemberAsync(Guid boardId, Guid userId)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            DELETE FROM BoardMembers 
            WHERE BoardId = @BoardId AND UserId = @UserId";

        await connection.ExecuteAsync(sql, new { BoardId = boardId, UserId = userId });
    }
}