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
            (ra, s) => new
{
    ra.rate.idPrice,
    ra.rate.idAccommodation,
    accommodationName = (ra.accommodation.name ?? "") + 
                        (ra.accommodation.number != null ? " — N° " + ra.accommodation.number : ""),
    ra.rate.idSeason,
    seasonType = s.type + " (" + s.dateStart.ToString("dd/MM/yyyy") + " - " + s.dateFinish.ToString("dd/MM/yyyy") + ")",
    ra.rate.minimumPerson,
    ra.rate.maximumPerson,
    ra.rate.priceNight,
    ra.rate.pricePersonAdditional
})
        .OrderBy(r => r.idPrice)
        .ToListAsync();

    return Ok(rates);
}

        // POST: api/rates/create
[HttpPost("create")]
public async Task<IActionResult> Create(
    [FromBody] Rate rate)
{
    var rol = HttpContext.Session.GetString("UserRol");

    if (string.IsNullOrEmpty(rol))
    {
        return Unauthorized(new
        {
            message = "Debes iniciar sesión"
        });
    }

    if (rol != "Admin")
    {
        return StatusCode(403, new
        {
            message = "No autorizado"
        });
    }

    // Validar precios
    if (rate.priceNight < 0 || rate.pricePersonAdditional < 0)
    {
        return BadRequest(new
        {
            message = "Los precios no pueden ser negativos"
        });
    }

    // Validar personas
    if (rate.minimumPerson > rate.maximumPerson)
    {
        return BadRequest(new
        {
            message = "El mínimo de personas no puede ser mayor al máximo"
        });
    }

    var accommodationExists = await _context.Accommodations
        .AnyAsync(a => a.idAccommodation == rate.idAccommodation);

    if (!accommodationExists)
    {
        return BadRequest(new
        {
            message = "El alojamiento no existe"
        });
    }

    var seasonExists = await _context.Seasons
        .AnyAsync(s => s.idSeason == rate.idSeason);

    if (!seasonExists)
    {
        return BadRequest(new
        {
            message = "La temporada no existe"
        });
    }

    var newRate = new Rate
    {
        idAccommodation = rate.idAccommodation,
        idSeason = rate.idSeason,
        minimumPerson = rate.minimumPerson,
        maximumPerson = rate.maximumPerson,
        priceNight = rate.priceNight,
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
            {
                return Unauthorized(new
                {
                    message = "Debes iniciar sesión"
                });
            }

            if (rol != "Admin")
            {
                return StatusCode(403, new
                {
                    message = "No autorizado"
                });
            }

            var rate = await _context.Rates.FindAsync(id);

            if (rate == null)
            {
                return NotFound(new
                {
                    message = "Tarifa no encontrada"
                });
            }

            _context.Rates.Remove(rate);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tarifa eliminada correctamente"
            });
        }
    }
}