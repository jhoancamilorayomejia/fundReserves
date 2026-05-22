using Microsoft.AspNetCore.Mvc;
using FoundReserves.Data;
using FoundReserves.Models;

namespace FoundReserves.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
public async Task<ActionResult> CreateReservation([FromBody] Reserve reserve)
{
    var rol = HttpContext.Session.GetString("UserRol");
    if (string.IsNullOrEmpty(rol))
        return Unauthorized(new { message = "Debes iniciar sesión" });

    if (reserve == null)
        return BadRequest(new { message = "Datos inválidos" });

    // ── Verificar disponibilidad ──────────────────────────────────────
    var ocupado = _context.Reserves.Any(r =>
        r.IdAccommodation == reserve.IdAccommodation &&
        r.State           != "Cancelada"             &&
        r.DateStart       <  reserve.DateFinish      &&
        r.DateFinish      >  reserve.DateStart);

    if (ocupado)
        return Conflict(new {
            message = $"El alojamiento ya está reservado en ese rango de fechas."
        });

    reserve.State        = "Pendiente";
    reserve.DateCreation = DateTime.Now;

    _context.Reserves.Add(reserve);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Reserva creada exitosamente", data = reserve });
}

        // GET: api/reservations/my
        [HttpGet("my")]
        public ActionResult GetMyReservations()
        {
            var idUser = HttpContext.Session.GetInt32("IdUser");
            if (idUser == null)
                return Unauthorized(new { message = "Debes iniciar sesión" });

            var reservas = _context.Reserves
                .Where(r => r.IdUser == idUser.Value)
                .OrderByDescending(r => r.DateCreation)
                .ToList();

            return Ok(reservas);
        }

        // GET: api/reservations/byUserAndDate?idUser=6&createdAt=2026-05-22T12:37:34
        [HttpGet("byUserAndDate")]
public ActionResult GetByUserAndDate(
    [FromQuery] int idUser,
    [FromQuery] DateTime dateStart,
    [FromQuery] DateTime dateFinish)
{
    var reservas = _context.Reserves
        .Where(r => r.IdUser     == idUser     &&
                    r.DateStart  == dateStart   &&
                    r.DateFinish == dateFinish)
        .Join(_context.Accommodations,
              r => r.IdAccommodation,
              a => a.idAccommodation,
              (r, a) => new {
                  accommodationName        = a.name,
                  accommodationDescription = a.description,
                  state                    = r.State
              })
        .ToList();

    return Ok(reservas);
}

    // GET: api/reservations/available?idSede=1&dateStart=2026-06-01&dateFinish=2026-06-04
    // GET: api/reservations/available?idSede=1&dateStart=2026-06-01&dateFinish=2026-06-04&persons=3
[HttpGet("available")]
public ActionResult GetAvailable(
    [FromQuery] int idSede,
    [FromQuery] DateTime dateStart,
    [FromQuery] DateTime dateFinish,
    [FromQuery] int persons = 1)
{
    var ocupados = _context.Reserves
        .Where(r => r.DateStart  < dateFinish &&
                    r.DateFinish > dateStart  &&
                    r.State != "Cancelada")
        .Select(r => r.IdAccommodation)
        .Distinct()
        .ToList();

    var disponibles = _context.Accommodations
        .Where(a => a.idsede == idSede &&
                    !ocupados.Contains(a.idAccommodation) &&
                    a.maximumPerson >= persons)
        .Select(a => new {
            a.idAccommodation,
            a.name,
            a.number,
            a.maximumPerson,
            a.description,
            a.state,
            tarifas = _context.Rates
                .Where(r => r.idAccommodation == a.idAccommodation &&
                            r.minimumPerson   <= persons &&
                            r.maximumPerson   >= persons)
                .Join(_context.Seasons,
                      r => r.idSeason,
                      s => s.idSeason,
                      (r, s) => new {
                          r.idPrice,
                          seasonName   = s.name,
                          seasonType   = s.type,
                          r.priceNight,
                          r.pricePersonAdditional,
                          r.minimumPerson,
                          r.maximumPerson
                      })
                .ToList()
        })
        .ToList();

    return Ok(disponibles);
}


    }
}