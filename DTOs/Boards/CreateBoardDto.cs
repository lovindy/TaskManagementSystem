namespace TaskManagementSystem.DTOs.Boards;

public class CreateBoardDto
{
    public required string Title { get; set; }
    public string? Description { get; set; } 
}