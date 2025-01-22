using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementSystem.DTOs.Tasks;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;

namespace TaskManagementSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TaskController : ControllerBase
{
    private readonly ITaskRepository _taskRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IListRepository _listRepository;
    private readonly ILogger<TaskController> _logger;

    public TaskController(
        ITaskRepository taskRepository,
        IBoardRepository boardRepository,
        IListRepository listRepository,
        ILogger<TaskController> logger)
    {
        _taskRepository = taskRepository;
        _boardRepository = boardRepository;
        _listRepository = listRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var task = new TaskItem
            {
                ListId = createTaskDto.ListId,
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                Priority = createTaskDto.Priority,
                CreatedBy = userId
            };

            var taskId = await _taskRepository.CreateTaskAsync(task);
            return Ok(taskId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(500, "Error creating task");
        }
    }

    [HttpGet("list/{listId:guid}")]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasksByList(Guid listId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var tasks = await _taskRepository.GetTasksByListAsync(listId);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks");
            return StatusCode(500, "Error retrieving tasks");
        }
    }

    [HttpPut("{taskId:guid}/position")]
    public async Task<ActionResult> UpdateTaskPosition(
        Guid taskId,
        [FromQuery] Guid listId,
        [FromBody] UpdatePositionRequest newPosition)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            await _taskRepository.UpdateTaskPositionAsync(taskId, listId, newPosition.Position);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task position");
            return StatusCode(500, "Error updating task position");
        }
    }

    [HttpPut("{taskId:guid}/assign")]
    public async Task<ActionResult> AssignTask(Guid taskId, [FromBody] Guid assigneeId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            await _taskRepository.AssignTaskAsync(taskId, assigneeId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning task");
            return StatusCode(500, "Error assigning task");
        }
    }
}