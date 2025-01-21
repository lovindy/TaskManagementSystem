using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public interface IBoardRepository
{
    Task<Guid> CreateBoardAsync(Board board);
    Task<Board> GetBoardByIdAsync(Guid boardId);
    Task<IEnumerable<Board>> GetUserBoardsAsync(Guid userId);
    Task UpdateBoardAsync(Board board);
    Task DeleteBoardAsync(Guid boardId);
    Task<bool> IsBoardOwnerAsync(Guid boardId, Guid userId);
    Task<IEnumerable<Board>> SearchBoardsAsync(string searchTerm, Guid userId);
}