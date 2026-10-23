using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Data;
using TaskTracker.Api.DataTransferObjects;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers
{
    /// <summary>
    /// Create, read, update and delete tasks inside projects.
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskTrackerRepository _repository;

        public TasksController(ITaskTrackerRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Lists the tasks of one project.
        /// </summary>
        /// <param name="projectId">The project id.</param>
        /// <response code="200">The project's tasks (an empty list if it has none).</response>
        /// <response code="404">No project has this id.</response>
        [HttpGet("api/projects/{projectId:int}/tasks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<TaskItemResponse>>> GetForProject(int projectId)
        {
            var tasks = await _repository.GetTasksForProjectAsync(projectId);
            if (tasks is null)
            {
                return NotFound();
            }

            return Ok(tasks.Select(t => TaskItemResponse.FromModel(t)).ToList());
        }

        /// <summary>
        /// Creates a task inside a project.
        /// </summary>
        /// <remarks>Status is Todo, InProgress or Done, and defaults to Todo. The server sets the id and creation time.</remarks>
        /// <param name="projectId">The project the task belongs to.</param>
        /// <param name="request">The task's title, description, status and due date.</param>
        /// <response code="201">The task was created. The Location header points to it.</response>
        /// <response code="400">The title is missing, a field is too long, or the status is not valid.</response>
        /// <response code="404">No project has this id.</response>
        [HttpPost("api/projects/{projectId:int}/tasks")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Gets one task by its id.
        /// </summary>
        /// <param name="id">The task id.</param>
        /// <response code="200">The task.</response>
        /// <response code="404">No task has this id.</response>
        [HttpGet("api/tasks/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TaskItemResponse>> GetById(int id)
        {
            var task = await _repository.GetTaskAsync(id);
            if (task is null)
            {
                return NotFound();
            }

            return Ok(TaskItemResponse.FromModel(task));
        }

        /// <summary>
        /// Replaces a task's title, description, status and due date.
        /// </summary>
        /// <remarks>A field left out of the body is cleared (status goes back to Todo), because PUT replaces the whole task. A task cannot move to another project.</remarks>
        /// <param name="id">The task id.</param>
        /// <param name="request">The new title, description, status and due date.</param>
        /// <response code="200">The updated task.</response>
        /// <response code="400">The title is missing, a field is too long, or the status is not valid.</response>
        /// <response code="404">No task has this id.</response>
        [HttpPut("api/tasks/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Deletes a task.
        /// </summary>
        /// <param name="id">The task id.</param>
        /// <response code="204">The task was deleted.</response>
        /// <response code="404">No task has this id.</response>
        [HttpDelete("api/tasks/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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