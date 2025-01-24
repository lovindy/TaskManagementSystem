namespace TaskManagementSystem.DTOs.Boards;

public class BoardMemberResponseDto
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public required string Role { get; set; }
    public DateTime JoinedAt { get; set; }
}