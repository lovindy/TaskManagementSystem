using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.DTOs.Tasks;

public class CreateTaskDto
{
    [Required] 
    public Guid ListId { get; set; }

    [Required] [StringLength(200)]
    public required string Title { get; set; } 

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [Required] 
    public TaskPriority Priority { get; set; }

    public Guid? AssignedTo { get; set; }
}