using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs.Users;

public class RegisterUserDto
{
    [Required]
    [StringLength(255)]
    public string Username { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
}