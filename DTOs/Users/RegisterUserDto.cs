using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.Users;

public class RegisterUserDto
{
    [Required]
    [StringLength(255)]
    public required string Username { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }
}