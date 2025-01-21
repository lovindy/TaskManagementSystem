using Dapper;
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
        using var transaction = connection.BeginTransaction();

        try
        {
            // Insert the board
            const string createBoardSql = @"
                INSERT INTO Boards (Title, Description, CreatedBy)
                OUTPUT INSERTED.BoardId
                VALUES (@Title, @Description, @CreatedBy)";

            var boardId = await connection.ExecuteScalarAsync<Guid>(
                createBoardSql,
                board,
                transaction
            );

            // Add creator as board owner
            const string addOwnerSql = @"
                INSERT INTO BoardMembers (BoardId, UserId, Role)
                VALUES (@BoardId, @UserId, 'Owner')";

            await connection.ExecuteAsync(
                addOwnerSql,
                new { BoardId = boardId, UserId = board.CreatedBy },
                transaction
            );

            transaction.Commit();
            return boardId;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task<Board> GetBoardByIdAsync(Guid boardId)
    {
        using var connection = _context.CreateConnection();
        
        // Get board details
        const string boardSql = @"
            SELECT *
            FROM Boards
            WHERE BoardId = @BoardId";

        var board = await connection.QuerySingleOrDefaultAsync<Board>(
            boardSql,
            new { BoardId = boardId }
        );

        if (board != null)
        {
            // Get board members
            const string membersSql = @"
                SELECT bm.*, u.Username, u.IsActive
                FROM BoardMembers bm
                JOIN Users u ON bm.UserId = u.UserId
                WHERE bm.BoardId = @BoardId";

            var members = await connection.QueryAsync<BoardMember, User, BoardMember>(
                membersSql,
                (member, user) => {
                    member.User = user;
                    return member;
                },
                new { BoardId = boardId },
                splitOn: "Username"
            );

            board.Members = members.ToList();
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
                (member, user) => {
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
    
}