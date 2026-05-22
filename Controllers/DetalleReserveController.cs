using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;
using FoundReserves.Models;

namespace FoundReserves.Controllers
{
    [ApiController]
    [Route("api/detalleReserve")]
    public class DetalleReserveController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DetalleReserveController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ════════════════════════════════════════
        // CREAR DETALLE RESERVA
        // POST: /api/detalleReserve/create
        // ════════════════════════════════════════
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] DetalleReserveDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var detalle = new DetalleReserve
            {
                IdUser       = dto.IdUser,
                IdSede       = dto.IdSede,
                DateStart    = DateTime.Parse(dto.DateStart),
                DateFinish   = DateTime.Parse(dto.DateFinish),
                NumberPerson = dto.NumberPerson,
                NumberRooms  = dto.NumberRooms,
                TotalCal     = dto.TotalCal,

                // Inicialmente NULL
                PaymentProof = null
            };

            _db.DetalleReserves.Add(detalle);

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Detalle de reserva creado.",
                id = detalle.IdDetalleReserve
            });
        }

        // ════════════════════════════════════════
        // SUBIR PDF
        // POST: /api/detalleReserve/uploadProof/1
        // ════════════════════════════════════════
        [HttpPost("uploadProof/{id}")]
        public async Task<IActionResult> UploadProof(
            int id,
            IFormFile file
        )
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    message = "Archivo inválido"
                });
            }

            // Buscar detalle reserva
            var detalle = await _db.DetalleReserves
                .FirstOrDefaultAsync(d =>
                    d.IdDetalleReserve == id);

            if (detalle == null)
            {
                return NotFound(new
                {
                    message = "Reserva no encontrada"
                });
            }

            // Validar extensión
            var extension = Path
                .GetExtension(file.FileName)
                .ToLower();

            if (extension != ".pdf")
            {
                return BadRequest(new
                {
                    message = "Solo se permiten archivos PDF"
                });
            }

            // Ruta carpeta
            var folder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "comprobantes"
            );

            // Crear carpeta si no existe
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            // Nombre único
            var fileName = $"{Guid.NewGuid()}.pdf";

            // Ruta completa
            var path = Path.Combine(folder, fileName);

            // Guardar archivo
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Guardar ruta en DB
            detalle.PaymentProof =
                $"/uploads/comprobantes/{fileName}";

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "PDF subido correctamente",
                file = detalle.PaymentProof
            });
        }

        // ════════════════════════════════════════
        // OBTENER POR USUARIO
        // GET: /api/detalleReserve/byUser/1
        // ════════════════════════════════════════
        [HttpGet("byUser/{idUser}")]
        public async Task<IActionResult> GetByUser(int idUser)
        {
            var lista = await _db.DetalleReserves
                .Where(d => d.IdUser == idUser)
                .OrderByDescending(d => d.IdDetalleReserve)
                .ToListAsync();

            return Ok(lista);
        }

        // ════════════════════════════════════════
        // OBTENER TODOS
        // GET: /api/detalleReserve/all
        // ════════════════════════════════════════
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var lista = await _db.DetalleReserves
                .OrderByDescending(d => d.IdDetalleReserve)
                .ToListAsync();

            return Ok(lista);
        }
    }

    // ════════════════════════════════════════
    // DTO
    // ════════════════════════════════════════
    public class DetalleReserveDto
    {
        public int IdUser { get; set; }

        public int IdSede { get; set; }

        public string DateStart { get; set; } = "";

        public string DateFinish { get; set; } = "";

        public int NumberPerson { get; set; }

        public int NumberRooms { get; set; }

        public decimal TotalCal { get; set; }
    }
}