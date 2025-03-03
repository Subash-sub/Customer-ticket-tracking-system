using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ticket_tracking_system.Data;
using ticket_tracking_system.Models;
using ticket_tracking_system.Services;
using Microsoft.EntityFrameworkCore;

namespace ticket_tracking_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public EmailController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ticket.CustomerId = userId;
            ticket.Status = "Open";
            ticket.CreatedAt = DateTime.UtcNow;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Notify Customer
            var customer = await _context.Users.FindAsync(userId);
            if (customer != null)
                _emailService.SendEmail(customer.Email, "Ticket Created", $"Your ticket (ID: {ticket.Id}) has been created successfully.");

            // Notify Admin
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
            if (admin != null)
                _emailService.SendEmail(admin.Email, "New Ticket Created", $"Ticket ID: {ticket.Id} created by {customer.Name}.");

            return Ok(ticket);
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
                _emailService.SendEmail(support.Email, "Ticket Assigned", $"Ticket ID: {ticket.Id} has been assigned to you.");

            // Notify Customer
            var customer = await _context.Users.FindAsync(ticket.CustomerId);
            if (customer != null)
                _emailService.SendEmail(customer.Email, "Ticket Assigned", $"Ticket ID: {ticket.Id} has been assigned to {support.Name}.");

            return Ok(ticket);
        }

        [HttpPut("resolve/{id}")]
        [Authorize(Roles = "Support")]
        public async Task<IActionResult> ResolveTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound();

            ticket.Status = "Resolved";
            await _context.SaveChangesAsync();

            // Notify Customer
            var customer = await _context.Users.FindAsync(ticket.CustomerId);
            if (customer != null)
                _emailService.SendEmail(customer.Email, "Ticket Resolved", $"Ticket ID: {ticket.Id} has been resolved.");

            return Ok(ticket);
        }
    }
}
