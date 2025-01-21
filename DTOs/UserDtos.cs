using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.DTOs;

public class RegisterUserDto
{
    [Required]
    [StringLength(255)]
    public string Username { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; }
}

public class LoginDto
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
}

public class UpdateUserDto
{
    [Required]
    [StringLength(255)]
    public string Username { get; set; }
}

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; }
}

public class UserResponseDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LoginResponseDto
{
    public string Token { get; set; }
    public UserResponseDto User { get; set; }
}