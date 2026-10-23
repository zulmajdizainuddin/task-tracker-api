using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Data;
using TaskTracker.Api.DataTransferObjects;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers
{
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskTrackerRepository _repository;

        public TasksController(ITaskTrackerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("api/projects/{projectId:int}/tasks")]
        public async Task<ActionResult<List<TaskItemResponse>>> GetForProject(int projectId)
        {
            var tasks = await _repository.GetTasksForProjectAsync(projectId);
            if (tasks is null)
            {
                return NotFound();
            }

            return Ok(tasks.Select(t => TaskItemResponse.FromModel(t)).ToList());
        }

        [HttpPost("api/projects/{projectId:int}/tasks")]
        public async Task<ActionResult<TaskItemResponse>> Create(int projectId, TaskItemRequest request)
        {
            var task = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                DueDate = request.DueDate
            };

            var created = await _repository.AddTaskAsync(projectId, task);
            if (created is null)
            {
                return NotFound();
            }

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, TaskItemResponse.FromModel(created));
        }

        [HttpGet("api/tasks/{id:int}")]
        public async Task<ActionResult<TaskItemResponse>> GetById(int id)
        {
            var task = await _repository.GetTaskAsync(id);
            if (task is null)
            {
                return NotFound();
            }

            return Ok(TaskItemResponse.FromModel(task));
        }

        [HttpPut("api/tasks/{id:int}")]
        public async Task<ActionResult<TaskItemResponse>> Update(int id, TaskItemRequest request)
        {
            var changes = new TaskItem
            {
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                DueDate = request.DueDate
            };

            var updated = await _repository.UpdateTaskAsync(id, changes);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(TaskItemResponse.FromModel(updated));
        }

        [HttpDelete("api/tasks/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteTaskAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
