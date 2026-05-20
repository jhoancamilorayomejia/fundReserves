using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;
using FoundReserves.Models;

namespace FoundReserves.Controllers
{
    [Route("api/accommodation")]
    [ApiController]
    public class AccommodationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccommodationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/accommodation/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "Debes iniciar sesión" });

            var accommodations = await _context.Accommodations
                .Include(a => a.Sede)
                .Select(a => new
                {
                    a.idAccommodation,
                    a.idsede,
                    sede        = a.Sede != null ? a.Sede.name : "—",
                    a.name,
                    a.number,
                    a.maximumPerson,
                    a.description,
                    a.state
                })
                .ToListAsync();

            return Ok(accommodations);
        }

        // POST: /api/accommodation/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Accommodation accommodation)
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "Debes iniciar sesión" });

            _context.Accommodations.Add(accommodation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Alojamiento registrado correctamente" });
        }

        // DELETE: /api/accommodation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "Debes iniciar sesión" });

            var accommodation = await _context.Accommodations.FindAsync(id);
            if (accommodation == null)
                return NotFound(new { message = "Alojamiento no encontrado" });

            _context.Accommodations.Remove(accommodation);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Alojamiento eliminado correctamente" });
        }
    }
}