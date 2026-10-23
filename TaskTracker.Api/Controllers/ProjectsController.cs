using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Data;
using TaskTracker.Api.DataTransferObjects;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers
{
    /// <summary>
    /// Create, read, update and delete projects.
    /// </summary>
    [ApiController]
    [Route("api/projects")]
    [Produces("application/json")]
    public class ProjectsController : ControllerBase
    {
        private readonly ITaskTrackerRepository _repository;

        public ProjectsController(ITaskTrackerRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Lists all projects, each with its tasks.
        /// </summary>
        /// <response code="200">All projects.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProjectResponse>>> GetAll()
        {
            var projects = await _repository.GetProjectsAsync();
            return Ok(projects.Select(p => ProjectResponse.FromModel(p)).ToList());
        }

        /// <summary>
        /// Gets one project by its id.
        /// </summary>
        /// <param name="id">The project id.</param>
        /// <response code="200">The project.</response>
        /// <response code="404">No project has this id.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectResponse>> GetById(int id)
        {
            var project = await _repository.GetProjectAsync(id);
            if (project is null)
            {
                return NotFound();
            }

            return Ok(ProjectResponse.FromModel(project));
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <remarks>The server sets the id and creation time.</remarks>
        /// <param name="request">The project's name and optional description.</param>
        /// <response code="201">The project was created. The Location header points to it.</response>
        /// <response code="400">The name is missing or a field is too long.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProjectResponse>> Create(ProjectRequest request)
        {
            var project = new Project
            {
                Name = request.Name,
                Description = request.Description
            };

            var created = await _repository.AddProjectAsync(project);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ProjectResponse.FromModel(created));
        }

        /// <summary>
        /// Replaces a project's name and description.
        /// </summary>
        /// <remarks>A field left out of the body is cleared, because PUT replaces the whole project.</remarks>
        /// <param name="id">The project id.</param>
        /// <param name="request">The new name and description.</param>
        /// <response code="200">The updated project.</response>
        /// <response code="400">The name is missing or a field is too long.</response>
        /// <response code="404">No project has this id.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProjectResponse>> Update(int id, ProjectRequest request)
        {
            var changes = new Project
            {
                Name = request.Name,
                Description = request.Description
            };

            var updated = await _repository.UpdateProjectAsync(id, changes);
            if (updated is null)
            {
                return NotFound();
            }

            return Ok(ProjectResponse.FromModel(updated));
        }

        /// <summary>
        /// Deletes a project and all of its tasks.
        /// </summary>
        /// <param name="id">The project id.</param>
        /// <response code="204">The project and its tasks were deleted.</response>
        /// <response code="404">No project has this id.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.DeleteProjectAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}