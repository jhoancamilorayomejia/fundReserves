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

        // POST: api/reservations/create
        [HttpPost("create")]
        public async Task<ActionResult> CreateReservation([FromBody] Reserve reserve)
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol))
                return Unauthorized(new { message = "Debes iniciar sesión" });

            if (reserve == null)
                return BadRequest(new { message = "Datos inválidos" });

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
    }
}