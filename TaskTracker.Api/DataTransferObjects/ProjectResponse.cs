using TaskTracker.Api.Models;

namespace TaskTracker.Api.DataTransferObjects
{
    public class ProjectResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TaskItemResponse> Tasks { get; set; } = new();

        public static ProjectResponse FromModel(Project project)
        {
            return new ProjectResponse
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedAt = project.CreatedAt,
                Tasks = project.Tasks.Select(t => TaskItemResponse.FromModel(t)).ToList()
            };
        }
    }
}
