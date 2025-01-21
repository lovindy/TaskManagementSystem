namespace TaskManagementSystem.Models
{
    public class TaskAssignee
    {
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public DateTime AssignedAt { get; set; }
        public User User { get; set; }
    }
}
