using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Data
{
    public class EfTaskTrackerRepository : ITaskTrackerRepository
    {
        private readonly TaskTrackerDbContext _db;

        public EfTaskTrackerRepository(TaskTrackerDbContext db)
        {
            _db = db;
        }

        // ---------- Projects ----------

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _db.Projects
                .AsNoTracking()
                .Include(p => p.Tasks.OrderBy(t => t.Id))
                .OrderBy(p => p.Id)
                .ToListAsync();
        }

        public async Task<Project?> GetProjectAsync(int id)
        {
            return await _db.Projects
                .AsNoTracking()
                .Include(p => p.Tasks.OrderBy(t => t.Id))
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Project> AddProjectAsync(Project project)
        {
            project.CreatedAt = DateTime.UtcNow;
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            return project;
        }

        public async Task<Project?> UpdateProjectAsync(int id, Project updated)
        {
            var existing = await _db.Projects
                .Include(p => p.Tasks.OrderBy(t => t.Id))
                .FirstOrDefaultAsync(p => p.Id == id);
            if (existing is null)
            {
                return null;
            }

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var existing = await _db.Projects.FindAsync(id);
            if (existing is null)
            {
                return false;
            }

            // The database deletes the project's tasks too (cascade delete).
            _db.Projects.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        // ---------- Tasks ----------

        public async Task<List<TaskItem>?> GetTasksForProjectAsync(int projectId)
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
            {
                return null;
            }

            return await _db.Tasks
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId)
                .OrderBy(t => t.Id)
                .ToListAsync();
        }

        public async Task<TaskItem?> GetTaskAsync(int id)
        {
            return await _db.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<TaskItem?> AddTaskAsync(int projectId, TaskItem task)
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);
            if (!projectExists)
            {
                return null;
            }

            task.ProjectId = projectId;
            task.CreatedAt = DateTime.UtcNow;
            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();
            return task;
        }

        public async Task<TaskItem?> UpdateTaskAsync(int id, TaskItem updated)
        {
            var existing = await _db.Tasks.FindAsync(id);
            if (existing is null)
            {
                return null;
            }

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.Status = updated.Status;
            existing.DueDate = updated.DueDate;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var existing = await _db.Tasks.FindAsync(id);
            if (existing is null)
            {
                return false;
            }

            _db.Tasks.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
