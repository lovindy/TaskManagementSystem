using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.Boards;

public class AddBoardMemberDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    [RegularExpression("^(Admin|Member|Viewer)$", ErrorMessage = "Role must be either 'Admin', 'Member', or 'Viewer'")]
    public string Role { get; set; } = "Member";
}