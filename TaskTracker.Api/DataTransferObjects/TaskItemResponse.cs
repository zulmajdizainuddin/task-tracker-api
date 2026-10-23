using TaskTracker.Api.Models;

namespace TaskTracker.Api.DataTransferObjects
{
    public class TaskItemResponse
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskItemStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public static TaskItemResponse FromModel(TaskItem task)
        {
            return new TaskItemResponse
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt
            };
        }
    }
}
