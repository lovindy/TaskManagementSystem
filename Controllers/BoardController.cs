using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementSystem.DTOs.Boards;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;

namespace TaskManagementSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BoardController : ControllerBase
{
    private readonly IBoardRepository _boardRepository;
    private readonly ILogger<BoardController> _logger;

    public BoardController(IBoardRepository boardRepository, ILogger<BoardController> logger)
    {
        _boardRepository = boardRepository;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<BoardResponseDto>> CreateBoard(CreateBoardDto createDto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            
            var board = new Board
            {
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedBy = userId
            };

            var boardId = await _boardRepository.CreateBoardAsync(board);
            var createdBoard = await _boardRepository.GetBoardByIdAsync(boardId);

            return Ok(MapToBoardResponse(createdBoard));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating board");
            return StatusCode(500, "Error creating board");
        }
    }

    [HttpGet("{boardId:guid}")]
    public async Task<ActionResult<BoardResponseDto>> GetBoard(Guid boardId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var board = await _boardRepository.GetBoardByIdAsync(boardId);

            if (board == null)
            {
                return NotFound("Board not found");
            }

            if (!board.Members.Any(m => m.UserId == userId))
            {
                return Forbid();
            }

            return Ok(MapToBoardResponse(board));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving board");
            return StatusCode(500, "Error retrieving board");
        }
    }

    [HttpGet("my-boards")]
    public async Task<ActionResult<IEnumerable<BoardResponseDto>>> GetUserBoards()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var boards = await _boardRepository.GetUserBoardsAsync(userId);
            
            return Ok(boards.Select(MapToBoardResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user boards");
            return StatusCode(500, "Error retrieving boards");
        }
    }

    [HttpPut("{boardId:guid}")]
    public async Task<ActionResult<BoardResponseDto>> UpdateBoard(Guid boardId, UpdateBoardDto updateDto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var board = await _boardRepository.GetBoardByIdAsync(boardId);

            if (board == null)
            {
                return NotFound("Board not found");
            }

            if (!await _boardRepository.IsBoardOwnerAsync(boardId, userId))
            {
                return Forbid();
            }

            board.Title = updateDto.Title;
            board.Description = updateDto.Description;

            await _boardRepository.UpdateBoardAsync(board);
            var updatedBoard = await _boardRepository.GetBoardByIdAsync(boardId);

            return Ok(MapToBoardResponse(updatedBoard));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating board");
            return StatusCode(500, "Error updating board");
        }
    }

    [HttpDelete("{boardId:guid}")]
    public async Task<ActionResult> DeleteBoard(Guid boardId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            
            if (!await _boardRepository.IsBoardOwnerAsync(boardId, userId))
            {
                return Forbid();
            }

            await _boardRepository.DeleteBoardAsync(boardId);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting board");
            return StatusCode(500, "Error deleting board");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<BoardResponseDto>>> SearchBoards([FromQuery] string searchTerm)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var boards = await _boardRepository.SearchBoardsAsync(searchTerm, userId);
            
            return Ok(boards.Select(MapToBoardResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching boards");
            return StatusCode(500, "Error searching boards");
        }
    }

    private static BoardResponseDto MapToBoardResponse(Board board)
    {
        return new BoardResponseDto
        {
            BoardId = board.BoardId,
            Title = board.Title,
            Description = board.Description,
            CreatedBy = board.CreatedBy,
            CreatedAt = board.CreatedAt,
            UpdatedAt = board.UpdatedAt,
            Members = board.Members?.Select(m => new BoardMemberResponseDto
            {
                UserId = m.UserId,
                Username = m.User?.Username,
                Role = m.Role,
                JoinedAt = m.JoinedAt
            }).ToList()
        };
    }
}