namespace TaskManagementSystem.Models
{
    public class List
    {
        public Guid ListId { get; set; }
        public Guid BoardId { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
