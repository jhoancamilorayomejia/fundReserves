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
                .Join(_context.Accommodations,  //verifica que sean igual los id's
                    r => r.idAccommodation,     // ← ID de Rates
                    a => a.idAccommodation,     // ← ID de Accommodations (compara aquí)
                    (r, a) => new { rate = r, accommodation = a })
                .Join(_context.Seasons,
                    ra => ra.rate.idSeason,     // ← ID de Rates
                    s => s.idSeason,             // ← ID de Seasons
                    (ra, s) => new { ra.rate, ra.accommodation, season = s })
                // ← JOIN nuevo: trae el nombre de la sede
                .Join(_context.Sedes,
                    ras => ras.accommodation.idsede,  // ← ID de Accommodations
                    sede => sede.idSede,              // ← ID de Sedes
                    (ras, sede) => new
                    {
                        ras.rate.idPrice,
                        ras.rate.idAccommodation,

                        // Ahora incluye [NombreSede] delante
                        accommodationName =
    "[" + (sede.name ?? "") + " — " + (sede.city ?? "") + "] " +  //extrae name y city de sede
    (ras.accommodation.name ?? "") +                              //igual extrae name pero de alojamiento
    (ras.accommodation.number != null                             // extrae number de la tabla alojamiento
        ? " — N° " + ras.accommodation.number
        : ""),

                        ras.rate.idSeason,
                        seasonName = ras.season.name,   //aqui los datos que vienen de temporada
                        seasonType = ras.season.type,

                        ras.rate.minimumPerson,       //aqui ya las que son de la tabla rates
                        ras.rate.maximumPerson,
                        ras.rate.priceNight,
                        ras.rate.pricePersonAdditional
                    })
                .OrderBy(r => r.idPrice)
                .ToListAsync();
        //Asegura que el frontend siempre reciba las tarifas en el mismo orden, de menor a mayor ID
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

        // GET: para extrae solo las de un alojamiento en particular.
[HttpGet("byAccommodation/{idAccommodation}")]
public async Task<IActionResult> GetByAccommodation(int idAccommodation)
{
    var rol = HttpContext.Session.GetString("UserRol");
    if (string.IsNullOrEmpty(rol))
        return Unauthorized(new { message = "Debes iniciar sesión" });

    var rates = await _context.Rates   //primero filtra y luego hace join
        .Where(r => r.idAccommodation == idAccommodation) // ← filtra solo las tarifas de ese alojamiento
        .Join(_context.Seasons,                     // ← luego une con temporadas
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

    
    // PUT: api/rates/update/{id}
[HttpPut("update/{id}")]
public async Task<IActionResult> Update(int id, [FromBody] RateUpdateDto dto)
{
    var rol = HttpContext.Session.GetString("UserRol");
    if (string.IsNullOrEmpty(rol))
        return Unauthorized(new { message = "Debes iniciar sesión" });
    if (rol != "Admin")
        return StatusCode(403, new { message = "No autorizado" });

    var rate = await _context.Rates.FindAsync(id);
    if (rate == null)
        return NotFound(new { message = "Tarifa no encontrada" });

    if (dto.MinimumPerson > dto.MaximumPerson)
        return BadRequest(new { message = "El mínimo no puede ser mayor al máximo" });

    rate.minimumPerson         = dto.MinimumPerson;
    rate.maximumPerson         = dto.MaximumPerson;
    rate.priceNight            = dto.PriceNight;
    rate.pricePersonAdditional = dto.PricePersonAdditional;

    await _context.SaveChangesAsync();
    return Ok(new { message = "Tarifa actualizada correctamente" });
}

public class RateUpdateDto
{
    public int     MinimumPerson         { get; set; }
    public int     MaximumPerson         { get; set; }
    public decimal PriceNight            { get; set; }
    public decimal PricePersonAdditional { get; set; }
}

    }
}