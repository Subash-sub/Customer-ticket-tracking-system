//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using System.Security.Claims;
//using ticket_tracking_system.Data;
//using ticket_tracking_system.Models;
//using ticket_tracking_system.DTOs;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using System.IO;

//namespace ticket_tracking_system.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AttachmentsController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public AttachmentsController(AppDbContext context)
//        {
//            _context = context;
//        }
//        [ApiExplorerSettings(IgnoreApi = true)]
//        [HttpPost]
//        [Authorize]
//        public async Task<IActionResult> AddAttachment([FromForm] IFormFile file, [FromForm] int ticketId)
//        {
//            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

//            if (file == null || file.Length == 0)
//                return BadRequest("No file uploaded.");

//            var filePath = Path.Combine("uploads", file.FileName);

//            using (var stream = new FileStream(filePath, FileMode.Create))
//            {
//                await file.CopyToAsync(stream);
//            }

//            var attachment = new Attachment
//            {
//                FileName = file.FileName,
//                FilePath = filePath,
//                TicketId = ticketId,
//                UserId = userId,
//                CreatedAt = DateTime.UtcNow
//            };

//            _context.Attachments.Add(attachment);
//            await _context.SaveChangesAsync();

//            return Ok(attachment);
//        }

//        [HttpGet("{ticketId}")]
//        [Authorize]
//        public async Task<IActionResult> GetAttachments(int ticketId)
//        {
//            var attachments = await _context.Attachments
//                .Where(a => a.TicketId == ticketId)
//                .ToListAsync();

//            return Ok(attachments);
//        }
//    }
//}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ticket_tracking_system.Data;
using ticket_tracking_system.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ticket_tracking_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttachmentsController(AppDbContext context)
        {
            _context = context;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAttachment([FromForm] IFormFile file, [FromForm] int ticketId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var filePath = Path.Combine("uploads", file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var isScreenshot = file.ContentType.StartsWith("image/");

            var attachment = new Attachment
            {
                FileName = file.FileName,
                FilePath = filePath,
                TicketId = ticketId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsScreenshot = isScreenshot
            };

            _context.Attachments.Add(attachment);
            await _context.SaveChangesAsync();

            return Ok(attachment);
        }

        [HttpGet("{ticketId}")]
        [Authorize]
        public async Task<IActionResult> GetAttachments(int ticketId)
        {
            var attachments = await _context.Attachments
                .Where(a => a.TicketId == ticketId && a.IsScreenshot)
                .ToListAsync();

            return Ok(attachments);
        }
    }
}
