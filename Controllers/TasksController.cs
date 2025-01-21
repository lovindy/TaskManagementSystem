using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskRepository _taskRepository;

    public TasksController(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTask(TaskItem task)
    {
        task.CreatedBy = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var taskId = await _taskRepository.CreateTaskAsync(task);
        return Ok(taskId);
    }

    [HttpGet("list/{listId}")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasksByList(Guid listId)
    {
        var tasks = await _taskRepository.GetTasksByListAsync(listId);
        return Ok(tasks);
    }

    // [HttpPut("{taskId}/position")]
    // public async Task<ActionResult> UpdateTaskPosition(
    //     Guid taskId,
    //     [FromBody] UpdateTaskPositionRequest request)
    // {
    //     await _taskRepository.UpdateTaskPositionAsync(
    //         taskId, request.ListId, request.NewPosition);
    //     return Ok();
    // }

    [HttpPost("{taskId}/assign/{userId}")]
    public async Task<ActionResult> AssignTask(Guid taskId, Guid userId)
    {
        await _taskRepository.AssignTaskAsync(taskId, userId);
        return Ok();
    }
}
