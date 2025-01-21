using Dapper;
using System.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDbContext _context;

        public TaskRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateTaskAsync(TaskItem task)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ListId", task.ListId);
            parameters.Add("@Title", task.Title);
            parameters.Add("@Description", task.Description);
            parameters.Add("@DueDate", task.DueDate);
            parameters.Add("@Priority", task.Priority);
            parameters.Add("@CreatedBy", task.CreatedBy);

            var taskId = await connection.ExecuteScalarAsync<Guid>(
                "sp_CreateTask",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return taskId;
        }

        public Task<IEnumerable<TaskItem>> GetTasksByListAsync(Guid listId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTaskPositionAsync(Guid taskId, Guid listId, int newPosition)
        {
            throw new NotImplementedException();
        }

        public Task AssignTaskAsync(Guid taskId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
