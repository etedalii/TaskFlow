using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using TaskFlow.Api.Data;
using TaskFlow.Api.Models;
using TaskFlow.Api.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CommentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CommentsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Create a new comment
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CommentDto commentDto)
        {
            if (commentDto == null)
                return BadRequest("Comment data is null");

            // Map CommentDto to Comment entity
            var comment = _mapper.Map<Comment>(commentDto);
            comment.CreatedAt = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // Map created Comment entity back to CommentDto
            var createdCommentDto = _mapper.Map<CommentDto>(comment);

            return CreatedAtAction(nameof(GetCommentById), new { id = createdCommentDto.Id }, createdCommentDto);
        }

        // Read all comments for a specific task
        [HttpGet("task/{taskId}")]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetCommentsByTaskId(int taskId)
        {
            var comments = await _context.Comments
                                         .Where(c => c.TaskId == taskId)
                                         .ToListAsync();

            // Map list of Comment entities to list of CommentDto
            var commentDtos = _mapper.Map<IEnumerable<CommentDto>>(comments);

            return Ok(commentDtos);
        }

        // Read a single comment by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDto>> GetCommentById(int id)
        {
            var comment = await _context.Comments
                                        .FirstOrDefaultAsync(c => c.Id == id);

            if (comment == null)
                return NotFound();  // 404 Not Found if comment doesn't exist

            // Map Comment entity to CommentDto
            var commentDto = _mapper.Map<CommentDto>(comment);

            return Ok(commentDto);
        }

        // Update a comment
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(int id, [FromBody] CommentDto commentDto)
        {
            if (id != commentDto.Id)
                return BadRequest("Comment ID mismatch");

            // Map CommentDto to Comment entity
            var comment = _mapper.Map<Comment>(commentDto);
            _context.Entry(comment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
                    return NotFound();  // 404 Not Found if comment doesn't exist
                else
                    return Conflict("The resource you attempted to modify has been changed by another process. Please refresh and try again.");
            }

            return NoContent();  // 204 No Content indicates successful update
        }

        // Delete a comment
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
                return NotFound();  // 404 Not Found if comment doesn't exist

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return NoContent();  // 204 No Content indicates successful deletion
        }

        // Helper method to check if a comment exists
        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}