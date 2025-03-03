using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ticket_tracking_system.Data;
using ticket_tracking_system.Models;
using ticket_tracking_system.Services;
using ticket_tracking_system.DTOs;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace ticket_tracking_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthenticationController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
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

            var registeredUser = new
            {
                user.Id,
                user.Name,
                user.Email,
                user.Role
            };

            return Ok(registeredUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .Where(u => u.Email == dto.Email)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.PasswordHash,
                    u.Role
                })
                .FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return BadRequest("Invalid credentials.");

            var token = _jwtService.GenerateToken(new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            });

            return Ok(new { token, user.Role });
        }
    }
}

