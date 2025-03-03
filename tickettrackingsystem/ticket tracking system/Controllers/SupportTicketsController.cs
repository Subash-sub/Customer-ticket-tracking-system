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
    public class SupportTicketsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public SupportTicketsController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPut("resolve/{id}")]
        [Authorize(Roles = "Support")]
        public async Task<IActionResult> ResolveTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            ticket.Status = "Resolved";
            ticket.ResolvedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Notify Customer
            var customer = await _context.Users.FindAsync(ticket.CustomerId);
            if (customer != null)
                await _emailService.SendEmailAsync(customer.Email, "Ticket Resolved", $"Ticket ID: {ticket.Id} has been resolved.");

            return Ok(ticket);
        }
    }
}

