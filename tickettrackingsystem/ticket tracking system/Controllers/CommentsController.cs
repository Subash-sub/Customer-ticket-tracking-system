using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ticket_tracking_system.Data;
using ticket_tracking_system.Models;
using ticket_tracking_system.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ticket_tracking_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CommentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddComment(AddCommentDto addCommentDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var comment = new Comment
            {
                Text = addCommentDto.Text,
                TicketId = addCommentDto.TicketId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }

        [HttpGet("view/{ticketId}")]
        [Authorize]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            var comments = await _context.Comments
                .Where(c => c.TicketId == ticketId)
                .ToListAsync();

            return Ok(comments);
        }
    }
}

