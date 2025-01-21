namespace TaskManagementSystem.Models;

public class BoardMember
{
    public Guid BoardId { get; set; }
    public Guid UserId { get; set; }
    public string Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public User User { get; set; }
}