namespace TaskManagementSystem.DTOs.BoardMembers;

public class AddBoardMemberDto
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = "Member";
}