using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class TaskItem
    {
        public Guid TaskId { get; set; }
        public Guid ListId { get; set; }
        public required string Title { get; set; } 
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int Position { get; set; }
        [Required] public TaskPriority Priority { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}