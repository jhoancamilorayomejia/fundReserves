using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;

namespace FoundReserves.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/users/all
        // Devuelve todos los usuarios (sin password).
        // Solo accesible si hay sesión activa con rol Admin.
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var rol = HttpContext.Session.GetString("UserRol");
            if (string.IsNullOrEmpty(rol) || rol != "Admin")
                return Unauthorized(new { message = "Acceso no autorizado" });

            var users = await _context.Users
                .OrderByDescending(u => u.createdAt)
                .Select(u => new
{
    u.iduser,
    u.cedula,
    u.name,
    u.lastname,
    u.phone,
    u.email,
    u.rol,
    u.createdAt,
    // ✅ Nuevos
    u.fechaNacimiento,
    u.departamento,
    u.municipio,
    u.barrio,
    u.direccion,
    u.preguntaSecreta,
    u.respuestaSecreta,
    u.autorizaCorreo,
    u.autorizaCelular
})
                .ToListAsync();

            return Ok(users);
        }

        // DELETE /api/users/{id}
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var rol = HttpContext.Session.GetString("UserRol");
    if (string.IsNullOrEmpty(rol) || rol != "Admin")
        return Unauthorized(new { message = "Acceso no autorizado" });

    var user = await _context.Users.FindAsync(id);
    if (user == null)
        return NotFound(new { message = "Usuario no encontrado" });

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Usuario eliminado correctamente" });
}

    
    // POST /api/users/createAdmin
[HttpPost("createAdmin")]
public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDto dto)
{
    var rol = HttpContext.Session.GetString("UserRol");
    if (string.IsNullOrEmpty(rol) || rol != "Admin")
        return Unauthorized(new { message = "Acceso no autorizado" });

    if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        return BadRequest(new { message = "Correo y contraseña son obligatorios" });

    var existe = await _context.Users.AnyAsync(u => u.email == dto.Email);
    if (existe)
        return Conflict(new { message = "Ya existe un usuario con ese correo" });

    var nuevoAdmin = new FoundReserves.Models.User
    {
        cedula    = dto.Cedula,
        name      = dto.Name,
        lastname  = dto.Lastname,
        phone     = dto.Phone,
        email     = dto.Email,
        password  = dto.Password,   // hashea si tu app lo requiere
        rol       = "Admin",        // siempre Admin
        createdAt = DateTime.Now
    };

    _context.Users.Add(nuevoAdmin);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Administrador creado correctamente", id = nuevoAdmin.iduser });
}

// DTO para crear admin
public class CreateAdminDto
{
    public string Cedula   { get; set; } = "";
    public string Name     { get; set; } = "";
    public string Lastname { get; set; } = "";
    public string Phone    { get; set; } = "";
    public string Email    { get; set; } = "";
    public string Password { get; set; } = "";
}
   

    // PUT /api/users/update/{id}
[HttpPut("update/{id}")]
public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
{
    var rol = HttpContext.Session.GetString("UserRol");
    if (string.IsNullOrEmpty(rol) || rol != "Admin")
        return Unauthorized(new { message = "Acceso no autorizado" });

    var user = await _context.Users.FindAsync(id);
    if (user == null)
        return NotFound(new { message = "Usuario no encontrado" });

    if (!string.IsNullOrWhiteSpace(dto.Cedula))   user.cedula   = dto.Cedula;
    if (!string.IsNullOrWhiteSpace(dto.Name))     user.name     = dto.Name;
    if (!string.IsNullOrWhiteSpace(dto.Lastname)) user.lastname = dto.Lastname;
    if (!string.IsNullOrWhiteSpace(dto.Phone))    user.phone    = dto.Phone;
    if (!string.IsNullOrWhiteSpace(dto.Email))    user.email    = dto.Email;
    if (!string.IsNullOrWhiteSpace(dto.Password)) user.password = dto.Password;

    await _context.SaveChangesAsync();
    return Ok(new { message = "Usuario actualizado correctamente" });
}

public class UpdateUserDto
{
    public string? Cedula   { get; set; }
    public string? Name     { get; set; }
    public string? Lastname { get; set; }
    public string? Phone    { get; set; }
    public string? Email    { get; set; }
    public string? Password { get; set; }
}

    }

}

