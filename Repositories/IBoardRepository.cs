using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public interface IBoardRepository
{
    Task<Guid> CreateBoardAsync(Board board);
    Task<Board> GetBoardByIdAsync(Guid boardId);
    Task<IEnumerable<Board>> GetUserBoardsAsync(Guid userId);
}