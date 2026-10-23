using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data
{
    public class InMemoryTaskTrackerRepository : ITaskTrackerRepository
    {
        private readonly List<Project> _projects;
        private readonly object _lock = new();
        private int _nextProjectId;
        private int _nextTaskId;

        public InMemoryTaskTrackerRepository()
        {
            _projects = SeedData.Projects();

            foreach (var task in SeedData.Tasks())
            {
                var project = _projects.First(p => p.Id == task.ProjectId);
                project.Tasks.Add(task);
            }

            _nextProjectId = _projects.Select(p => p.Id).DefaultIfEmpty(0).Max() + 1;
            _nextTaskId = AllTasks().Select(t => t.Id).DefaultIfEmpty(0).Max() + 1;
        }


        public Task<List<Project>> GetProjectsAsync()
        {
            lock (_lock)
            {
                return Task.FromResult(_projects.ToList());
            }
        }

        public Task<Project?> GetProjectAsync(int id)
        {
            lock (_lock)
            {
                return Task.FromResult(FindProject(id));
            }
        }

        public Task<Project> AddProjectAsync(Project project)
        {
            lock (_lock)
            {
                project.Id = _nextProjectId++;
                project.CreatedAt = DateTime.UtcNow;
                project.Tasks = new List<TaskItem>();
                _projects.Add(project);
                return Task.FromResult(project);
            }
        }

        public Task<Project?> UpdateProjectAsync(int id, Project updated)
        {
            lock (_lock)
            {
                var existing = FindProject(id);
                if (existing is null)
                {
                    return Task.FromResult<Project?>(null);
                }

                existing.Name = updated.Name;
                existing.Description = updated.Description;
                return Task.FromResult<Project?>(existing);
            }
        }

        public Task<bool> DeleteProjectAsync(int id)
        {
            lock (_lock)
            {
                var existing = FindProject(id);
                if (existing is null)
                {
                    return Task.FromResult(false);
                }

                _projects.Remove(existing);
                return Task.FromResult(true);
            }
        }

        public Task<List<TaskItem>?> GetTasksForProjectAsync(int projectId)
        {
            lock (_lock)
            {
                var project = FindProject(projectId);
                return Task.FromResult(project?.Tasks.ToList());
            }
        }

        public Task<TaskItem?> GetTaskAsync(int id)
        {
            lock (_lock)
            {
                return Task.FromResult(FindTask(id));
            }
        }

        public Task<TaskItem?> AddTaskAsync(int projectId, TaskItem task)
        {
            lock (_lock)
            {
                var project = FindProject(projectId);
                if (project is null)
                {
                    return Task.FromResult<TaskItem?>(null);
                }

                task.Id = _nextTaskId++;
                task.ProjectId = projectId;
                task.CreatedAt = DateTime.UtcNow;
                project.Tasks.Add(task);
                return Task.FromResult<TaskItem?>(task);
            }
        }

        public Task<TaskItem?> UpdateTaskAsync(int id, TaskItem updated)
        {
            lock (_lock)
            {
                var existing = FindTask(id);
                if (existing is null)
                {
                    return Task.FromResult<TaskItem?>(null);
                }

                existing.Title = updated.Title;
                existing.Description = updated.Description;
                existing.Status = updated.Status;
                existing.DueDate = updated.DueDate;
                return Task.FromResult<TaskItem?>(existing);
            }
        }

        public Task<bool> DeleteTaskAsync(int id)
        {
            lock (_lock)
            {
                var task = FindTask(id);
                if (task is null)
                {
                    return Task.FromResult(false);
                }

                var project = FindProject(task.ProjectId);
                project?.Tasks.Remove(task);
                return Task.FromResult(true);
            }
        }

        private Project? FindProject(int id)
        {
            return _projects.FirstOrDefault(p => p.Id == id);
        }

        private TaskItem? FindTask(int id)
        {
            return AllTasks().FirstOrDefault(t => t.Id == id);
        }

        private IEnumerable<TaskItem> AllTasks()
        {
            return _projects.SelectMany(p => p.Tasks);
        }
    }
}
