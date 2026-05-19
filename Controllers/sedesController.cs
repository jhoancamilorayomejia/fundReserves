using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;

namespace FoundReserves.Controllers
{
    [Route("api/sedes")]
    [ApiController]
    public class SedesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SedesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/sedes/all — accesible para cualquier usuario autenticado
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "Debes iniciar sesión" });

            var sedes = await _context.Sedes
                .OrderBy(s => s.name)
                .Select(s => new
                {
                    s.idSede,
                    s.name,
                    s.city,
                    s.region,
                    s.description,
                    s.maximumCapacity,
                    s.type,
                    s.priceLaundry
                })
                .ToListAsync();

            return Ok(sedes);
        }
    }
}