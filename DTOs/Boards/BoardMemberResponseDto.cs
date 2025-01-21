namespace TaskManagementSystem.DTOs.Boards;

public class BoardMemberResponseDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public DateTime JoinedAt { get; set; }
}