﻿using System.Data;
using Dapper;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public class ListRepository : IListRepository
{
    private readonly IDbContext _context;

    public ListRepository(IDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateListAsync(List list)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            DECLARE @Position INT;
            SELECT @Position = ISNULL(MAX(Position), 0) + 1 
            FROM Lists 
            WHERE BoardId = @BoardId;

            INSERT INTO Lists (BoardId, Title, Position)
            OUTPUT INSERTED.ListId
            VALUES (@BoardId, @Title, @Position)";

        return await connection.ExecuteScalarAsync<Guid>(sql, list);
    }

    public async Task<IEnumerable<List>> GetBoardListsAsync(Guid boardId)
    {
        using var connection = _context.CreateConnection();
        const string storedProcedure = "usp_GetBoardLists";
        return await connection.QueryAsync<List>(storedProcedure, new { BoardId = boardId }, commandType: CommandType.StoredProcedure);
    }
    
    public async Task UpdateListPositionAsync(Guid listId, int newPosition)
    {
        using var connection = _context.CreateConnection();
        const string storedProcedure = "usp_UpdateListPosition";

        await connection.ExecuteAsync(
            storedProcedure,
            new { ListId = listId, NewPosition = newPosition },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }
    
    public async Task UpdateListTitleAsync(Guid listId, string newTitle)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"
            UPDATE Lists 
            SET Title = @Title, UpdatedAt = GETDATE()
            WHERE ListId = @ListId";

        await connection.ExecuteAsync(sql, new { ListId = listId, Title = newTitle });
    }

    public async Task DeleteListAsync(Guid listId)
    {
        using var connection = _context.CreateConnection();
        const string sql = @"DELETE FROM Lists WHERE ListId = @ListId";
        await connection.ExecuteAsync(sql, new { ListId = listId });
    }
}