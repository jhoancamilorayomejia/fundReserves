using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;

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
    // ✅ Agrega esta validación igual que en SedesController
    var rol = HttpContext.Session.GetString("UserRol");
    if (string.IsNullOrEmpty(rol))
        return Unauthorized(new { message = "Debes iniciar sesión" });

    var accommodations = await _context.Accommodations
        .Include(a => a.Sede)
        .Select(a => new
        {
            a.idAccommodation,
            a.idsede,
            sede = a.Sede != null ? a.Sede.name : "—",  // ✅ Ya hace el JOIN
            a.name,
            a.number,
            a.maximumPerson,
            a.description,
            a.state
        })
        .ToListAsync();

    return Ok(accommodations);
}
    }
}