using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.Data;
using TaskTracker.Api.DataTransferObjects;
using TaskTracker.Api.Models;

namespace TaskTracker.Api.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly ITaskTrackerRepository _repository;

        public ProjectsController(ITaskTrackerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectResponse>>> GetAll()
        {
            var projects = await _repository.GetProjectsAsync();
            return Ok(projects.Select(p => ProjectResponse.FromModel(p)).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectResponse>> GetById(int id)
        {
            var project = await _repository.GetProjectAsync(id);
            if (project is null)
            {
                return NotFound();
            }

            return Ok(ProjectResponse.FromModel(project));
        }

        [HttpPost]
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

        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
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
