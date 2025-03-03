using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ticket_tracking_system.Data;
using ticket_tracking_system.Models;
using ticket_tracking_system.Services;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ticket_tracking_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminTicketsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public AdminTicketsController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPut("assign/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTicket(int id, [FromBody] int supportId)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            ticket.AssignedSupportId = supportId;
            ticket.Status = "Assigned";
            await _context.SaveChangesAsync();

            // Notify IT Support
            var support = await _context.Users.FindAsync(supportId);
            if (support != null)
                await _emailService.SendEmailAsync(support.Email, "Ticket Assigned", $"Ticket ID: {ticket.Id} has been assigned to you.");

            // Notify Customer
            var customer = await _context.Users.FindAsync(ticket.CustomerId);
            if (customer != null)
                await _emailService.SendEmailAsync(customer.Email, "Ticket Assigned", $"Ticket ID: {ticket.Id} has been assigned to {support.Name}.");

            return Ok(ticket);
        }
    }
}

