using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;

namespace FoundReserves.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/users/all
        // Devuelve todos los usuarios (sin password).
        // Solo accesible si hay sesión activa con rol Admin.
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol) || rol != "Admin")
                return Unauthorized(new { message = "Acceso no autorizado" });

            var users = await _context.Users
                .OrderByDescending(u => u.createdAt)
                .Select(u => new
                {
                    u.iduser,
                    u.cedula,
                    u.name,
                    u.lastname,
                    u.phone,
                    u.email,
                    u.rol,
                    u.createdAt
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}