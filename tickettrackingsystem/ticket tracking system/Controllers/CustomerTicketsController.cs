using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ticket_tracking_system.Data;
using ticket_tracking_system.Models;
using ticket_tracking_system.Services;
using ticket_tracking_system.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ticket_tracking_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerTicketsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public CustomerTicketsController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("CreateTicket")]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);
            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                Priority = dto.Priority,
                Status = "Open",
                CustomerId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            // Notify Customer
            var customer = await _context.Users.FindAsync(userId);
            if (customer != null)
                await _emailService.SendEmailAsync(customer.Email, "Ticket Created", $"Your ticket (ID: {ticket.Id}) has been created successfully.");

            // Notify Admin
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
            if (admin != null)
                await _emailService.SendEmailAsync(admin.Email, "New Ticket Created", $"Ticket ID: {ticket.Id} created by {customer.Name}.");

            return Ok(ticket);
        }

        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("ViewTickets/{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);
            var ticket = await _context.Tickets
                .Where(t => t.Id == id && (t.CustomerId == userId || User.IsInRole("Admin")))
                .FirstOrDefaultAsync();

            if (ticket == null)
                return NotFound();

            var ticketDto = new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status,
                Priority = ticket.Priority,
                CreatedAt = ticket.CreatedAt,
                CustomerId = ticket.CustomerId
            };

            return Ok(ticketDto);
        }

        [Authorize(Roles = "Customer,Admin")]
        [HttpPut("UpdateTicket/{id}")]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] TicketDto ticketDto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);
            var ticket = await _context.Tickets
                .Where(t => t.Id == id && (t.CustomerId == userId || User.IsInRole("Admin")))
                .FirstOrDefaultAsync();

            if (ticket == null)
                return NotFound();

            ticket.Title = ticketDto.Title;
            ticket.Description = ticketDto.Description;
            ticket.Priority = ticketDto.Priority;
            ticket.Status = ticketDto.Status;

            await _context.SaveChangesAsync();

            return Ok(ticket);
        }

        [Authorize(Roles = "Customer,Admin")]
        [HttpDelete("DeleteTicket/{id}")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdClaim);
            var ticket = await _context.Tickets
                .Where(t => t.Id == id && (t.CustomerId == userId || User.IsInRole("Admin")))
                .FirstOrDefaultAsync();

            if (ticket == null)
                return NotFound();

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

