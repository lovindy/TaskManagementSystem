using System.Data;
using Dapper;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public class BoardRepository : IBoardRepository
{
    private readonly IDbContext _context;

    public BoardRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateBoardAsync(Board board)
    {
        using var connection = _context.CreateConnection();
        connection.Open(); // Use SqlConnection's OpenAsync

        using var transaction = connection.BeginTransaction(); // Use SqlTransaction

        try
        {
            const string createBoardSql = @"
        INSERT INTO Boards (Title, Description, CreatedBy)
        OUTPUT INSERTED.BoardId
        VALUES (@Title, @Description, @CreatedBy)";

            var boardId = await connection.ExecuteScalarAsync<Guid>(
                createBoardSql,
                board,
                transaction
            );

            const string addOwnerSql = @"
        INSERT INTO BoardMembers (BoardId, UserId, Role)
        VALUES (@BoardId, @UserId, 'Admin')";

            await connection.ExecuteAsync(
                addOwnerSql,
                new { BoardId = boardId, UserId = board.CreatedBy },
                transaction
            );

            transaction.Commit(); // Use synchronous Commit
            return boardId;
        }
        catch
        {
            transaction.Rollback(); // Use synchronous Rollback
            throw;
        }
    }


    public async Task<Board> GetBoardByIdAsync(Guid boardId)
    {
        using var connection = _context.CreateConnection();

        using var multi = await connection.QueryMultipleAsync(
            "sp_GetBoardById",
            new { BoardId = boardId },
            commandType: CommandType.StoredProcedure
        );

        var board = await multi.ReadFirstOrDefaultAsync<Board>();
        if (board != null)
        {
            var memberDictionary = new Dictionary<Guid, BoardMember>();

            var members = await connection.QueryAsync<BoardMember, User, BoardMember>(
                "SELECT bm.BoardId, bm.UserId, bm.Role, bm.JoinedAt, u.UserId, u.Username, u.PasswordHash, u.IsActive, u.CreatedAt, u.UpdatedAt " +
                "FROM BoardMembers bm JOIN Users u ON bm.UserId = u.UserId WHERE bm.BoardId = @BoardId",
                (member, user) =>
                {
                    if (!memberDictionary.TryGetValue(member.UserId, out var boardMember))
                    {
                        boardMember = member;
                        boardMember.User = user;
                        memberDictionary[member.UserId] = boardMember;
                    }

                    return boardMember;
                },
                new { BoardId = boardId },
                splitOn: "UserId"
            );

            board.Members = memberDictionary.Values.ToList();
        }

        return board;
    }

    public async Task<IEnumerable<Board>> GetUserBoardsAsync(Guid userId)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT b.*
            FROM Boards b
            JOIN BoardMembers bm ON b.BoardId = bm.BoardId
            WHERE bm.UserId = @UserId
            ORDER BY b.UpdatedAt DESC";

        var boards = await connection.QueryAsync<Board>(sql, new { UserId = userId });

        // Load members for each board
        foreach (var board in boards)
        {
            const string membersSql = @"
                SELECT bm.*, u.Username, u.IsActive
                FROM BoardMembers bm
                JOIN Users u ON bm.UserId = u.UserId
                WHERE bm.BoardId = @BoardId";

            var members = await connection.QueryAsync<BoardMember, User, BoardMember>(
                membersSql,
                (member, user) =>
                {
                    member.User = user;
                    return member;
                },
                new { BoardId = board.BoardId },
                splitOn: "Username"
            );

            board.Members = members.ToList();
        }

        return boards;
    }

    public async Task UpdateBoardAsync(Board board)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            UPDATE Boards 
            SET Title = @Title,
                Description = @Description,
                UpdatedAt = GETDATE()
            WHERE BoardId = @BoardId";

        await connection.ExecuteAsync(sql, board);
    }

    public async Task DeleteBoardAsync(Guid boardId)
    {
        using var connection = _context.CreateConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            // Delete all tasks in all lists
            const string deleteTasksSql = @"
                DELETE t
                FROM Tasks t
                JOIN Lists l ON t.ListId = l.ListId
                WHERE l.BoardId = @BoardId";

            await connection.ExecuteAsync(deleteTasksSql, new { BoardId = boardId }, transaction);

            // Delete all lists
            const string deleteListsSql = @"
                DELETE FROM Lists
                WHERE BoardId = @BoardId";

            await connection.ExecuteAsync(deleteListsSql, new { BoardId = boardId }, transaction);

            // Delete all board members
            const string deleteMembersSql = @"
                DELETE FROM BoardMembers
                WHERE BoardId = @BoardId";

            await connection.ExecuteAsync(deleteMembersSql, new { BoardId = boardId }, transaction);

            // Delete the board
            const string deleteBoardSql = @"
                DELETE FROM Boards
                WHERE BoardId = @BoardId";

            await connection.ExecuteAsync(deleteBoardSql, new { BoardId = boardId }, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<bool> IsBoardOwnerAsync(Guid boardId, Guid userId)
    {
        using var connection = _context.CreateConnection();

        var count = await connection.ExecuteScalarAsync<int>(
            "sp_IsBoardOwner",
            new { BoardId = boardId, UserId = userId },
            commandType: CommandType.StoredProcedure
        );

        return count > 0;
    }

    public async Task<IEnumerable<Board>> SearchBoardsAsync(string searchTerm, Guid userId)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<Board>(
            "sp_SearchBoards",
            new
            {
                UserId = userId,
                SearchTerm = searchTerm
            },
            commandType: CommandType.StoredProcedure
        );
    }
}