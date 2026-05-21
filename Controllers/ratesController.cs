using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;
using FoundReserves.Models;

namespace FoundReserves.Controllers
{
    [Route("api/rates")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/rates/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "Debes iniciar sesión" });

            var rates = await _context.Rates
                .Join(_context.Accommodations,
                    r => r.idAccommodation,
                    a => a.idAccommodation,
                    (r, a) => new { rate = r, accommodation = a })
                .Join(_context.Seasons,
                    ra => ra.rate.idSeason,
                    s => s.idSeason,
                    (ra, s) => new { ra.rate, ra.accommodation, season = s })
                // ← JOIN nuevo: trae el nombre de la sede
                .Join(_context.Sedes,
                    ras => ras.accommodation.idsede,
                    sede => sede.idSede,
                    (ras, sede) => new
                    {
                        ras.rate.idPrice,
                        ras.rate.idAccommodation,

                        // Ahora incluye [NombreSede] delante
                        accommodationName =
    "[" + (sede.name ?? "") + " — " + (sede.city ?? "") + "] " +
    (ras.accommodation.name ?? "") +
    (ras.accommodation.number != null
        ? " — N° " + ras.accommodation.number
        : ""),

                        ras.rate.idSeason,
                        seasonName = ras.season.name,
                        seasonType = ras.season.type,

                        ras.rate.minimumPerson,
                        ras.rate.maximumPerson,
                        ras.rate.priceNight,
                        ras.rate.pricePersonAdditional
                    })
                .OrderBy(r => r.idPrice)
                .ToListAsync();

            return Ok(rates);
        }

        // POST: api/rates/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] Rate rate)
        {
            var rol = HttpContext.Session.GetString("UserRol");

            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "Debes iniciar sesión" });

            if (rol != "Admin")
                return StatusCode(403, new { message = "No autorizado" });

            if (rate.priceNight < 0 || rate.pricePersonAdditional < 0)
                return BadRequest(new { message = "Los precios no pueden ser negativos" });

            if (rate.minimumPerson > rate.maximumPerson)
                return BadRequest(new { message = "El mínimo de personas no puede ser mayor al máximo" });

            var accommodationExists = await _context.Accommodations
                .AnyAsync(a => a.idAccommodation == rate.idAccommodation);

            if (!accommodationExists)
                return BadRequest(new { message = "El alojamiento no existe" });

            var seasonExists = await _context.Seasons
                .AnyAsync(s => s.idSeason == rate.idSeason);

            if (!seasonExists)
                return BadRequest(new { message = "La temporada no existe" });

            var newRate = new Rate
            {
                idAccommodation       = rate.idAccommodation,
                idSeason              = rate.idSeason,
                minimumPerson         = rate.minimumPerson,
                maximumPerson         = rate.maximumPerson,
                priceNight            = rate.priceNight,
                pricePersonAdditional = rate.pricePersonAdditional
            };

            _context.Rates.Add(newRate);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tarifa registrada correctamente",
                rate = newRate
            });
        }

        // DELETE: api/rates/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rol = HttpContext.Session.GetString("UserRol");

            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "Debes iniciar sesión" });

            if (rol != "Admin")
                return StatusCode(403, new { message = "No autorizado" });

            var rate = await _context.Rates.FindAsync(id);

            if (rate == null)
                return NotFound(new { message = "Tarifa no encontrada" });

            _context.Rates.Remove(rate);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tarifa eliminada correctamente" });
        }

        // GET: api/rates/byAccommodation/{idAccommodation}
[HttpGet("byAccommodation/{idAccommodation}")]
public async Task<IActionResult> GetByAccommodation(int idAccommodation)
{
    var rol = HttpContext.Session.GetString("UserRol");
    if (string.IsNullOrEmpty(rol))
        return Unauthorized(new { message = "Debes iniciar sesión" });

    var rates = await _context.Rates
        .Where(r => r.idAccommodation == idAccommodation)
        .Join(_context.Seasons,
            r => r.idSeason,
            s => s.idSeason,
            (r, s) => new
            {
                r.idPrice,
                r.idSeason,
                seasonName            = s.name,
                seasonType            = s.type,
                r.minimumPerson,
                r.maximumPerson,
                r.priceNight,
                r.pricePersonAdditional
            })
        .OrderBy(r => r.idSeason)
        .ToListAsync();

    if (!rates.Any())
        return NotFound(new { message = "No hay tarifas para este alojamiento" });

    return Ok(rates);
}
    }
}