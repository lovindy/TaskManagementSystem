namespace TaskManagementSystem.DTOs.Users;

public class UserResponseDto
{
    public Guid UserId { get; set; }
    public required string Username { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
