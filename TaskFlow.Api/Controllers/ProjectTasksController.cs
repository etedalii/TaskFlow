using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.Models.Dtos;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ProjectTasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectTasksController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Create a new task
        [HttpPost]
        public async Task<IActionResult> CreateProjectTask([FromBody] ProjectTaskDto projectTaskDto)
        {
            if (projectTaskDto == null)
                return BadRequest("Task data is null");

            // Map ProjectTaskDto to ProjectTask entity
            var projectTask = _mapper.Map<ProjectTask>(projectTaskDto);

            _context.ProjectTasks.Add(projectTask);
            await _context.SaveChangesAsync();

            // Map created ProjectTask entity back to ProjectTaskDto
            var createdProjectTaskDto = _mapper.Map<ProjectTaskDto>(projectTask);

            return CreatedAtAction(nameof(GetProjectTaskById), new { id = createdProjectTaskDto.Id }, createdProjectTaskDto);
        }

        // Read all tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectTaskDto>>> GetProjectTasks()
        {
            var tasks = await _context.ProjectTasks.ToListAsync();

            // Map list of ProjectTask entities to list of ProjectTaskDto
            var taskDtos = _mapper.Map<IEnumerable<ProjectTaskDto>>(tasks);

            return Ok(taskDtos);  // 200 OK with task list
        }

        // Read a single task by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectTaskDto>> GetProjectTaskById(int id)
        {
            var projectTask = await _context.ProjectTasks
                                            .FirstOrDefaultAsync(pt => pt.Id == id);

            if (projectTask == null)
                return NotFound();  // 404 Not Found if task doesn't exist

            // Map ProjectTask entity to ProjectTaskDto
            var projectTaskDto = _mapper.Map<ProjectTaskDto>(projectTask);

            return Ok(projectTaskDto);  // 200 OK with task data
        }

        // Update a task
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProjectTask(int id, [FromBody] ProjectTaskDto projectTaskDto)
        {
            if (id != projectTaskDto.Id)  // Ensure route ID matches body ID
                return BadRequest("Task ID mismatch");

            // Map ProjectTaskDto to ProjectTask entity
            var projectTask = _mapper.Map<ProjectTask>(projectTaskDto);
            _context.Entry(projectTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectTaskExists(id))
                    return NotFound();  // 404 Not Found if task doesn't exist
                else
                    return Conflict("The resource you attempted to modify has been changed by another process. Please refresh and try again.");
            }

            return NoContent();  // 204 No Content indicates a successful update
        }

        // Delete a task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectTask(int id)
        {
            var projectTask = await _context.ProjectTasks.FindAsync(id);
            if (projectTask == null)
                return NotFound();  // 404 Not Found if task doesn't exist

            _context.ProjectTasks.Remove(projectTask);
            await _context.SaveChangesAsync();
            return NoContent();  // 204 No Content indicates successful deletion
        }

        // Helper method to check if a task exists
        private bool ProjectTaskExists(int id)
        {
            return _context.ProjectTasks.Any(e => e.Id == id);
        }
    }
}