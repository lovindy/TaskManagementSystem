namespace TaskManagementSystem.DTOs.Boards;

public class BoardResponseDto
{
    public Guid BoardId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public required ICollection<BoardMemberResponseDto> Members { get; set; }
}
