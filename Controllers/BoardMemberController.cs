using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementSystem.DTOs.BoardMembers;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;

namespace TaskManagementSystem.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BoardMemberController : ControllerBase
{
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly ILogger<BoardMemberController> _logger;

    public BoardMemberController(
        IBoardMemberRepository boardMemberRepository,
        IBoardRepository boardRepository,
        ILogger<BoardMemberController> logger)
    {
        _boardMemberRepository = boardMemberRepository;
        _boardRepository = boardRepository;
        _logger = logger;
    }

    [HttpGet("{boardId:guid}/members")]
    public async Task<ActionResult<IEnumerable<BoardMember>>> GetBoardMembers(Guid boardId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
            // Check if user has access to the board (either as owner or member)
            if (!await _boardRepository.IsBoardOwnerAsync(boardId, userId) && 
                !await _boardMemberRepository.IsBoardMemberAsync(boardId, userId))
            {
                return Forbid();
            }

            var members = await _boardMemberRepository.GetBoardMembersAsync(boardId);
            return Ok(members);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving board members");
            return StatusCode(500, "Error retrieving board members");
        }
    }

    [HttpPost("{boardId:guid}/members")]
    public async Task<ActionResult> AddBoardMember(Guid boardId, [FromBody] AddBoardMemberDto request)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
    
            // Check if user is board owner or admin
            if (!await CanManageMembers(boardId, userId))
            {
                return Forbid();
            }

            var member = new BoardMember
            {
                BoardId = boardId,
                UserId = request.UserId,
                Role = request.Role
            };

            await _boardMemberRepository.AddBoardMemberAsync(member);
        
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding board member");
            return StatusCode(500, "Error adding board member");
        }
    }

    [HttpPut("{boardId:guid}/members/{memberId:guid}/role")]
    public async Task<ActionResult> UpdateMemberRole(Guid boardId, Guid memberId, [FromBody] string newRole)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            
            // Check if user is board owner
            if (!await _boardRepository.IsBoardOwnerAsync(boardId, userId))
            {
                return Forbid();
            }

            await _boardMemberRepository.UpdateBoardMemberRoleAsync(boardId, memberId, newRole);
            
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating board member role");
            return StatusCode(500, "Error updating board member role");
        }
    }

    [HttpDelete("{boardId:guid}/members/{memberId:guid}")]
    public async Task<ActionResult> RemoveBoardMember(Guid boardId, Guid memberId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            
            // Check if user is board owner
            if (!await _boardRepository.IsBoardOwnerAsync(boardId, userId))
            {
                return Forbid();
            }

            await _boardMemberRepository.RemoveBoardMemberAsync(boardId, memberId);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing board member");
            return StatusCode(500, "Error removing board member");
        }
    }
    
    private async Task<bool> CanManageMembers(Guid boardId, Guid userId)
    {
        return await _boardRepository.IsBoardOwnerAsync(boardId, userId) ||
               await _boardMemberRepository.IsBoardAdminAsync(boardId, userId);
    }
}