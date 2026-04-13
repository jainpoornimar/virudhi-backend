using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HerbalMedicalCare.Data;
using HerbalMedicalCare.Models;
using HerbalMedicalCare.DTOs;

namespace HerbalMedicalCare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly HerbalCareDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(HerbalCareDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 🔐 REGISTER
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (existingUser != null)
            {
                return BadRequest(new { message = "User already exists" });
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 🔥 First user becomes Admin
            var role = _context.Users.Any() ? "User" : "Admin";

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = _context.Users.Any() ? "User" : "Admin"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User registered successfully"
            });
        }

        // 🔑 LOGIN (USER + ADMIN)
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null)
                return Unauthorized(new { message = "Invalid email" });

            bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!isValid)
                return Unauthorized(new { message = "Invalid password" });

            // 🔥 JWT SETTINGS
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256
                )
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                id = user.Id,
                name = user.Name,
                email = user.Email,
                role = user.Role
            });
        }

        // 🔐 TEST PROTECTED ROUTE
        [HttpGet("profile")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult GetProfile()
        {
            return Ok("Protected route 🔐");
        }
    }
}