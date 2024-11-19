using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.Models.Dtos;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProjectController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Create a new project
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDto projectDto)
        {
            if (projectDto == null)
                return BadRequest("Project data is null");

            // Map ProjectDto to Project entity
            var project = _mapper.Map<Project>(projectDto);

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Map created Project entity back to ProjectDto
            var createdProjectDto = _mapper.Map<ProjectDto>(project);

            // Returns 201 Created status with the URI for the newly created project
            return CreatedAtAction(nameof(GetProjectById), new { id = createdProjectDto.Id }, createdProjectDto);
        }

        // Read all projects
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects()
        {
            var projects = await _context.Projects.Include(u => u.Owner).ToListAsync();

            // Map list of Project entities to list of ProjectDto
            var projectDtos = _mapper.Map<IEnumerable<ProjectDto>>(projects);

            return Ok(new {data= projectDtos});  // 200 OK with project list
        }

        // Read a single project by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            var project = await _context.Projects.Include(u => u.Owner).FirstOrDefaultAsync(p => p.Id == id);
            if (project == null) 
                return NotFound();  // 404 Not Found if project doesn't exist
            
            // Map Project entity to ProjectDto
            var projectDto = _mapper.Map<ProjectDto>(project);

            return Ok(projectDto);  // 200 OK with project data
        }

        // Update a project
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] ProjectDto projectDto)
        {
            if (id != projectDto.Id)  // Ensure route ID matches body ID
                return BadRequest("Project ID mismatch");

            // Map ProjectDto to Project entity
            var project = _mapper.Map<Project>(projectDto);
            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                    return NotFound();  // 404 Not Found if project doesn't exist
                else
                    return Conflict("The resource you attempted to modify has been changed by another process. Please refresh and try again.");
            }

            return NoContent();  // 204 No Content indicates a successful update without returning the updated resource
        }

        // Delete a project
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
                return NotFound();  // 404 Not Found if project doesn't exist

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();  // 204 No Content indicates successful deletion
        }

        // Helper method to check if a project exists
        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}