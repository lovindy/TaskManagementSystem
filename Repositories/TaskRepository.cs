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
        
        public async Task<IEnumerable<TaskItem>> GetTasksByListAsync(Guid listId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ListId", listId);

            var tasks = await connection.QueryAsync<TaskItem>(
                "sp_GetTasksByList",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return tasks;
        }

        public async Task UpdateTaskPositionAsync(Guid taskId, Guid listId, int newPosition)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@TaskId", taskId);
            parameters.Add("@ListId", listId);
            parameters.Add("@NewPosition", newPosition);

            await connection.ExecuteAsync(
                "sp_UpdateTaskPosition",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task AssignTaskAsync(Guid taskId, Guid userId)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@TaskId", taskId);
            parameters.Add("@UserId", userId);

            await connection.ExecuteAsync(
                "sp_AssignTask",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
