using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;
using FoundReserves.Models;

namespace FoundReserves.Controllers
{
    [Route("api/seasons")]
    [ApiController]
    public class SeasonsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SeasonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/seasons/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var rol = HttpContext.Session.GetString("UserRol");

            if (string.IsNullOrEmpty(rol))
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión"
                });
            }

            var seasons = await _context.Seasons
                .OrderBy(s => s.dateStart)
                .Select(s => new
                {
                    s.idSeason,
                    s.dateStart,
                    s.dateFinish,
                    s.type
                })
                .ToListAsync();

            return Ok(seasons);
        }

        // POST: /api/seasons/create
        [HttpPost("create")]
        public async Task<IActionResult> Create(
            [FromBody] Season season)
        {
            var rol = HttpContext.Session.GetString("UserRol");

            if (string.IsNullOrEmpty(rol))
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión"
                });
            }

            // Solo Admin puede crear temporadas
            if (rol != "Admin")
            {
                return StatusCode(403, new
                {
                    message = "No autorizado"
                });
            }

            // Validaciones
            if (season.dateStart == default ||
                season.dateFinish == default)
            {
                return BadRequest(new
                {
                    message = "Las fechas son obligatorias"
                });
            }

            if (season.dateFinish < season.dateStart)
            {
                return BadRequest(new
                {
                    message = "La fecha final no puede ser menor a la inicial"
                });
            }

            if (string.IsNullOrWhiteSpace(season.type))
            {
                return BadRequest(new
                {
                    message = "El tipo de temporada es obligatorio"
                });
            }

            var newSeason = new Season
            {
                dateStart = season.dateStart,
                dateFinish = season.dateFinish,
                type = season.type
            };

            _context.Seasons.Add(newSeason);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Temporada registrada correctamente",
                season = new
                {
                    newSeason.idSeason,
                    newSeason.dateStart,
                    newSeason.dateFinish,
                    newSeason.type
                }
            });
        }

        // DELETE: /api/seasons/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSeason(int id)
        {
            var rol = HttpContext.Session.GetString("UserRol");

            if (string.IsNullOrEmpty(rol))
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión"
                });
            }

            // Solo Admin puede eliminar temporadas
            if (rol != "Admin")
            {
                return StatusCode(403, new
                {
                    message = "No autorizado"
                });
            }

            var season = await _context.Seasons.FindAsync(id);

            if (season == null)
            {
                return NotFound(new
                {
                    message = "Temporada no encontrada"
                });
            }

            _context.Seasons.Remove(season);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Temporada eliminada correctamente"
            });
        }
    }
}