using System.ComponentModel.DataAnnotations;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.DataTransferObjects
{
    public class TaskItemRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [EnumDataType(typeof(TaskItemStatus))]
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Todo;

        public DateTime? DueDate { get; set; }
    }
}
