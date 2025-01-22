using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class TaskItem
    {
        public Guid TaskId { get; set; }
        public Guid ListId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        [Required]
        public TaskPriority Priority { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}