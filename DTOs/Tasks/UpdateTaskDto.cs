namespace TaskManagementSystem.DTOs.Tasks
{
    public class UpdateTaskDto
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; }
    }
}