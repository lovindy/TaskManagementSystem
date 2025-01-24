namespace TaskManagementSystem.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public required string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
