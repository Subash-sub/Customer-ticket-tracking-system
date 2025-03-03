using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ticket_tracking_system.Data;
using ticket_tracking_system.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ticket_tracking_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("metrics")]
        [Authorize(Roles = "Admin,Support")]
        public async Task<IActionResult> GetMetrics()
        {
            var totalTickets = await _context.Tickets.CountAsync();
            var openTickets = await _context.Tickets.CountAsync(t => t.Status == "Open");
            var assignedTickets = await _context.Tickets.CountAsync(t => t.Status == "Assigned");
            var resolvedTickets = await _context.Tickets.CountAsync(t => t.Status == "Resolved");
            var resolvedTicketsQuery = _context.Tickets
    .Where(t => t.Status == "Resolved");

            var averageResolutionTime = resolvedTicketsQuery.Any()
                ? await resolvedTicketsQuery.AverageAsync(t => EF.Functions.DateDiffDay(t.CreatedAt, t.ResolvedAt))
                : 0; // or null

            var metrics = new
            {
                TotalTickets = totalTickets,
                OpenTickets = openTickets,
                AssignedTickets = assignedTickets,
                ResolvedTickets = resolvedTickets,
                AverageResolutionTime = averageResolutionTime
            };

            return Ok(metrics);
        }
    }
}
