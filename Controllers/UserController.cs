using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenService _tokenService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserRepository userRepository,
        ITokenService tokenService,
        ILogger<UserController> logger)
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserResponseDto>> Register(RegisterUserDto registerDto)
    {
        try
        {
            if (await _userRepository.UsernameExistsAsync(registerDto.Username))
            {
                return BadRequest("Username is already taken");
            }

            var user = new User
            {
                Username = registerDto.Username,
                IsActive = true
            };

            var userId = await _userRepository.CreateUserAsync(user, registerDto.Password);
            var createdUser = await _userRepository.GetUserByIdAsync(userId);

            return Ok(MapToUserResponse(createdUser));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return StatusCode(500, "Error registering user");
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
    {
        try
        {
            var isValid = await _userRepository.ValidateUserCredentialsAsync(
                loginDto.Username, 
                loginDto.Password
            );

            if (!isValid)
            {
                return Unauthorized("Invalid username or password");
            }

            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

            if (!user.IsActive)
            {
                return Unauthorized("Account is deactivated");
            }

            var token = _tokenService.GenerateToken(user);

            return Ok(new LoginResponseDto
            {
                Token = token,
                User = MapToUserResponse(user)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, "Error during login");
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponseDto>> GetCurrentUser()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(MapToUserResponse(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return StatusCode(500, "Error retrieving user information");
        }
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<ActionResult<UserResponseDto>> UpdateUser(UpdateUserDto updateDto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            if (updateDto.Username != user.Username && 
                await _userRepository.UsernameExistsAsync(updateDto.Username))
            {
                return BadRequest("Username is already taken");
            }

            user.Username = updateDto.Username;
            await _userRepository.UpdateUserAsync(user);

            return Ok(MapToUserResponse(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return StatusCode(500, "Error updating user information");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> SearchUsers([FromQuery] string searchTerm)
    {
        try
        {
            var users = await _userRepository.SearchUsersAsync(searchTerm);
            return Ok(users.Select(MapToUserResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users");
            return StatusCode(500, "Error searching users");
        }
    }

    private static UserResponseDto MapToUserResponse(User user)
    {
        return new UserResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}
