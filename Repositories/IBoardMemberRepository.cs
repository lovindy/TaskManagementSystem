using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories;

public interface IBoardMemberRepository
{
    Task<IEnumerable<BoardMember>> GetBoardMembersAsync(Guid boardId);
    Task AddBoardMemberAsync(BoardMember member);
    Task UpdateBoardMemberRoleAsync(Guid boardId, Guid userId, string newRole);
    Task RemoveBoardMemberAsync(Guid boardId, Guid userId);
    Task<bool> IsBoardMemberAsync(Guid boardId, Guid userId);
    Task<bool> IsBoardAdminAsync(Guid boardId, Guid userId);
}