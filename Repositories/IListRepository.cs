using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public interface IListRepository
{
    Task<Guid> CreateListAsync(List list);
    Task<List?> GetListAsync(Guid listId);
    Task<IEnumerable<List>> GetBoardListsAsync(Guid boardId);
    Task UpdateListPositionAsync(Guid listId, int newPosition);
    Task UpdateListTitleAsync(Guid listId, string newTitle);
    Task DeleteListAsync(Guid listId);
}