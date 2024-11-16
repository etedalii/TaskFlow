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
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public NotificationsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Create a new notification
        [HttpPost]
        public async Task<IActionResult> CreateNotification([FromBody] NotificationDto notificationDto)
        {
            if (notificationDto == null)
                return BadRequest("Notification data is null");

            // Map NotificationDto to Notification entity
            var notification = _mapper.Map<Notification>(notificationDto);
            notification.CreatedAt = DateTime.UtcNow;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Map created Notification entity back to NotificationDto
            var createdNotificationDto = _mapper.Map<NotificationDto>(notification);

            return CreatedAtAction(nameof(GetNotificationById), new { id = createdNotificationDto.Id }, createdNotificationDto);
        }

        // Read all notifications for a specific user
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotificationsByUserId(string userId)
        {
            var notifications = await _context.Notifications
                                              .Where(n => n.UserId == userId)
                                              .ToListAsync();

            // Map list of Notification entities to list of NotificationDto
            var notificationDtos = _mapper.Map<IEnumerable<NotificationDto>>(notifications);

            return Ok(notificationDtos);
        }

        // Read a single notification by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationDto>> GetNotificationById(int id)
        {
            var notification = await _context.Notifications
                                             .FirstOrDefaultAsync(n => n.Id == id);

            if (notification == null)
                return NotFound();  // 404 Not Found if notification doesn't exist

            // Map Notification entity to NotificationDto
            var notificationDto = _mapper.Map<NotificationDto>(notification);

            return Ok(notificationDto);
        }

        // Mark a notification as read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            _context.Entry(notification).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Update a notification
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] NotificationDto notificationDto)
        {
            if (id != notificationDto.Id)
                return BadRequest("Notification ID mismatch");

            // Map NotificationDto to Notification entity
            var notification = _mapper.Map<Notification>(notificationDto);
            _context.Entry(notification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationExists(id))
                    return NotFound();
                else
                    return Conflict("The resource you attempted to modify has been changed by another process. Please refresh and try again.");
            }

            return NoContent();
        }

        // Delete a notification
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
                return NotFound();

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Helper method to check if a notification exists
        private bool NotificationExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}