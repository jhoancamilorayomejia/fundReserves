using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;
using FoundReserves.Models;

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

        // GET /api/sedes/all
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

        // POST /api/sedes/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Sede model)
        {
            var rol = HttpContext.Session.GetString("UserRol");

            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "Debes iniciar sesión" });

            // Solo administradores
            if (rol != "Admin")
                return Forbid();

            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(model.name))
                return BadRequest(new { message = "El nombre es obligatorio" });

            if (string.IsNullOrWhiteSpace(model.city))
                return BadRequest(new { message = "La ciudad es obligatoria" });

            var sede = new Sede
            {
                name = model.name,
                city = model.city,
                region = model.region,
                description = model.description,
                maximumCapacity = model.maximumCapacity,
                type = model.type,
                priceLaundry = model.priceLaundry
            };

            _context.Sedes.Add(sede);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Sede registrada correctamente",
                sede
            });
        }
    }
}