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

        // =========================================
        // POST: api/reservations/create
        // =========================================
        [HttpPost("create")]
public async Task<ActionResult> CreateReservation([FromBody] Reserve reserve)
{
    var rol = HttpContext.Session.GetString("UserRol");
    if (string.IsNullOrEmpty(rol))
        return Unauthorized(new { message = "Debes iniciar sesión" });

    if (reserve == null)
        return BadRequest(new { message = "Datos inválidos" });

    // State y DateCreation se asignan en el servidor, no dependen del cliente
    reserve.State        = "Pendiente";
    reserve.DateCreation = DateTime.Now;

    _context.Reserves.Add(reserve);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Reserva creada exitosamente", data = reserve });
}

    // =========================================
// GET: api/reservations/my
// Devuelve las reservas del usuario en sesión
// =========================================
// GET: api/reservations/my
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




    }
}