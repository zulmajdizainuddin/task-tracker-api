using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data
{
    public interface ITaskTrackerRepository
    {
        Task<List<Project>> GetProjectsAsync();
        Task<Project?> GetProjectAsync(int id);
        Task<Project> AddProjectAsync(Project project);
        Task<Project?> UpdateProjectAsync(int id, Project updated);
        Task<bool> DeleteProjectAsync(int id);

        // Tasks
        Task<List<TaskItem>?> GetTasksForProjectAsync(int projectId);
        Task<TaskItem?> GetTaskAsync(int id);
        Task<TaskItem?> AddTaskAsync(int projectId, TaskItem task);
        Task<TaskItem?> UpdateTaskAsync(int id, TaskItem updated);
        Task<bool> DeleteTaskAsync(int id);
    }
}
