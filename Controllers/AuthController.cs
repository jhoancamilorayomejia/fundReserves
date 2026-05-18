using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;
using FoundReserves.Models;

namespace FoundReserves.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email == model.Email);

            if (user == null)
                return Unauthorized(new { message = "Correo o contraseña incorrectos" });

            // Comparación directa sin hash
            if (user.password != model.Password)
                return Unauthorized(new { message = "Correo o contraseña incorrectos" });

            return Ok(new
            {
                message = "Login exitoso",
                userId = user.id,
                email = user.email,
                rol = user.rol
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exists = await _context.Users
                .AnyAsync(u => u.email == model.Email);

            if (exists)
                return Conflict(new { message = "El correo ya está registrado" });

            var user = new User
            {
                email = model.Email,
                password = model.Password, // sin hash
                rol = model.Rol ?? "Client"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario creado", userId = user.id });
        }
    }
}