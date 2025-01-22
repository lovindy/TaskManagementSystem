using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;

namespace TaskManagementSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ListController : ControllerBase
{
    private readonly IListRepository _listRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly ILogger<ListController> _logger;

    public ListController(
        IListRepository listRepository,
        IBoardRepository boardRepository,
        ILogger<ListController> logger)
    {
        _listRepository = listRepository;
        _boardRepository = boardRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateList([FromBody] List list)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Verify board access
            if (!await _boardRepository.IsBoardOwnerAsync(list.BoardId, userId))
            {
                return Forbid();
            }

            var listId = await _listRepository.CreateListAsync(list);
            return Ok(listId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating list");
            return StatusCode(500, "Error creating list");
        }
    }

    [HttpGet("board/{boardId:guid}")]
    public async Task<ActionResult<IEnumerable<List>>> GetBoardLists(Guid boardId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Verify board access
            var board = await _boardRepository.GetBoardByIdAsync(boardId);
            if (board == null)
            {
                return NotFound("Board not found");
            }

            if (!board.Members.Any(m => m.UserId == userId))
            {
                return Forbid();
            }

            var lists = await _listRepository.GetBoardListsAsync(boardId);
            return Ok(lists);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving board lists");
            return StatusCode(500, "Error retrieving board lists");
        }
    }

    [HttpPut("{listId:guid}/position")]
    public async Task<ActionResult> UpdateListPosition(Guid listId, [FromBody] int newPosition)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            
            await _listRepository.UpdateListPositionAsync(listId, newPosition);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating list position");
            return StatusCode(500, "Error updating list position");
        }
    }

    [HttpPut("{listId:guid}/title")]
    public async Task<ActionResult> UpdateListTitle(Guid listId, [FromBody] string newTitle)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Similar note about board ownership check as in UpdateListPosition
            
            await _listRepository.UpdateListTitleAsync(listId, newTitle);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating list title");
            return StatusCode(500, "Error updating list title");
        }
    }

    [HttpDelete("{listId:guid}")]
    public async Task<ActionResult> DeleteList(Guid listId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // Similar note about board ownership check as in UpdateListPosition

            await _listRepository.DeleteListAsync(listId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting list");
            return StatusCode(500, "Error deleting list");
        }
    }
}