using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.Users;

public class UpdateUserDto
{
    [Required]
    [StringLength(255)]
    public string Username { get; set; }
}
