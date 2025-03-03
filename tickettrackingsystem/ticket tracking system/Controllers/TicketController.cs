using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ticket_tracking_system.Data;
using ticket_tracking_system.Models;
using ticket_tracking_system.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace ticket_tracking_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TicketsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Support")]
        public async Task<IActionResult> SearchTickets([FromQuery] string status, [FromQuery] string priority, [FromQuery] int? assignedSupportId)
        {
            var query = _context.Tickets.AsQueryable();

            if (!string.IsNullOrEmpty(status))
                query = query.Where(t => t.Status == status);

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(t => t.Priority == priority);

            if (assignedSupportId.HasValue)
                query = query.Where(t => t.AssignedSupportId == assignedSupportId);

            var tickets = await query.ToListAsync();

            return Ok(tickets);
        }
    }
}  
