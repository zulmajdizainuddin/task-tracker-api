namespace TaskTracker.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

        public DateTime? DueDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
