using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeShare.API.Helpers;
using RecipeShare.Application.DTOs;
using RecipeShare.Infrastructure.Data;

namespace RecipeShare.API.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtHelper _jwtHelper;
        private readonly AppDbContext _context;

        public AuthController(JwtHelper jwtHelper, AppDbContext context)
        {
            _jwtHelper = jwtHelper;
            _context = context;
        }

        [HttpPost("api/auth/login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid username or password");
            }

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var token = _jwtHelper.GenerateToken(user.Username, roles);

            return Ok(new
            {
                token,
                username = user.Username,
                roles
            });
        }
    }
}