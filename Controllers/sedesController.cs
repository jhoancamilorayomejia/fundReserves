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

            if (rol != "Admin")
                return Forbid();

            if (string.IsNullOrWhiteSpace(model.name))
                return BadRequest(new { message = "El nombre es obligatorio" });

            if (string.IsNullOrWhiteSpace(model.city))
                return BadRequest(new { message = "La ciudad es obligatoria" });

            var sede = new Sede
            {
                name            = model.name,
                city            = model.city,
                region          = model.region,
                description     = model.description,
                maximumCapacity = model.maximumCapacity,
                type            = model.type,
                priceLaundry    = model.priceLaundry
            };

            _context.Sedes.Add(sede);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sede registrada correctamente", sede });
        }

        // PUT /api/sedes/update/{id}
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSedeDto dto)
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol) || rol != "Admin")
                return Unauthorized(new { message = "No autorizado" });

            var sede = await _context.Sedes.FindAsync(id);
            if (sede == null)
                return NotFound(new { message = "Sede no encontrada" });

            if (!string.IsNullOrWhiteSpace(dto.Name))        sede.name            = dto.Name;
            if (!string.IsNullOrWhiteSpace(dto.City))        sede.city            = dto.City;
            if (!string.IsNullOrWhiteSpace(dto.Region))      sede.region          = dto.Region;
            if (!string.IsNullOrWhiteSpace(dto.Description)) sede.description     = dto.Description;
            if (!string.IsNullOrWhiteSpace(dto.Type))        sede.type            = dto.Type;
            if (dto.MaximumCapacity.HasValue)                 sede.maximumCapacity = dto.MaximumCapacity.Value;
            if (dto.PriceLaundry.HasValue)                    sede.priceLaundry    = dto.PriceLaundry.Value;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Sede actualizada correctamente" });
        }

        // DELETE /api/sedes/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol) || rol != "Admin")
                return Unauthorized(new { message = "No autorizado" });

            var sede = await _context.Sedes.FindAsync(id);
            if (sede == null)
                return NotFound(new { message = "Sede no encontrada" });

            _context.Sedes.Remove(sede);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Sede eliminada correctamente" });
        }
    }

    public class UpdateSedeDto
    {
        public string?  Name            { get; set; }
        public string?  City            { get; set; }
        public string?  Region          { get; set; }
        public string?  Description     { get; set; }
        public string?  Type            { get; set; }
        public int?     MaximumCapacity { get; set; }
        public decimal? PriceLaundry    { get; set; }
    }
}