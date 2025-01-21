namespace TaskManagementSystem.Models
{
    public class TaskItem
    {
        public Guid TaskId { get; set; }
        public Guid ListId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; }
        public int Position { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<TaskAssignee> Assignees { get; set; }
    }
}
