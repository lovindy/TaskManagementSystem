using TaskManagementSystem.DTOs.Tasks;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories
{
    public interface ITaskRepository
    {
        Task<Guid> CreateTaskAsync(TaskItem task);
        Task<IEnumerable<TaskItem>> GetTasksByListAsync(Guid listId);
        Task UpdateTaskPositionAsync(Guid taskId, Guid listId, int newPosition);
        Task AssignTaskAsync(Guid taskId, Guid userId);
        Task UpdateTaskAsync(Guid taskId, UpdateTaskDto updateDto);
        Task<TaskItem?> GetTaskByIdAsync(Guid taskId);
    }
}