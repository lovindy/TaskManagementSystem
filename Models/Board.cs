namespace TaskManagementSystem.Models;

public class Board
{
    public Guid BoardId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<BoardMember>? Members { get; set; }
}