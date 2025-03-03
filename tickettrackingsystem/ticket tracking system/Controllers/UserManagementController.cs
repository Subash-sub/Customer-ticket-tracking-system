//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using ticket_tracking_system.Data;
//using ticket_tracking_system.Models;
//using ticket_tracking_system.DTOs;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;
//using System.Linq;

//namespace ticket_tracking_system.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class UserManagementController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public UserManagementController(AppDbContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> GetUsers()
//        {
//            var users = await _context.Users.ToListAsync();
//            return Ok(users);
//        }

//        [HttpPost("AddUser")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> CreateUser(RegisterDto dto)
//        {
//            if (_context.Users.Any(u => u.Email == dto.Email))
//                return BadRequest("User already exists.");

//            var user = new User
//            {
//                Name = dto.Name,
//                Email = dto.Email,
//                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
//                Role = dto.Role
//            };

//            _context.Users.Add(user);
//            await _context.SaveChangesAsync();

//            return Ok(user);
//        }

//        [HttpPut("UpdateUser/{id}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> UpdateUser(int id, RegisterDto dto)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null)
//                return NotFound();

//            user.Name = dto.Name;
//            user.Email = dto.Email;
//            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
//            user.Role = dto.Role;

//            await _context.SaveChangesAsync();

//            return Ok(user);
//        }

//        [HttpDelete("DeleteUser/{id}")]
//        [Authorize(Roles = "Admin")]
//        public async Task<IActionResult> DeleteUser(int id)
//        {
//            var user = await _context.Users.FindAsync(id);
//            if (user == null)
//                return NotFound();

//            _context.Users.Remove(user);
//            await _context.SaveChangesAsync();

//            return NoContent();
//        }
//    }
//}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class UserManagementController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserManagementController> _logger;

        public UserManagementController(AppDbContext context, ILogger<UserManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting users.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("AddUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(RegisterDto dto)
        {
            try
            {
                if (_context.Users.Any(u => u.Email == dto.Email))
                    return BadRequest("User already exists.");

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = dto.Role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a user.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("UpdateUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, RegisterDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound();

                user.Name = dto.Name;
                user.Email = dto.Email;
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                user.Role = dto.Role;

                await _context.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating a user.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("DeleteUser/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound();

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting a user.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
