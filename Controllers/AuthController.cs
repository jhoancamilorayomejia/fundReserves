using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Data;
using FoundReserves.Models;

namespace FoundReserves.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginModel model)
{
    if (!ModelState.IsValid)
{
    return BadRequest(new
    {
        message = "Datos inválidos",
        errors = ModelState
    });
}

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.cedula == model.Cedula);

    if (user == null || user.password != model.Password)
        return Unauthorized(new { message = "Cédula o contraseña incorrectos" });

    return Ok(new
    {
        message = "Login exitoso",
        userId = user.iduser,
        cedula = user.cedula,
        name = user.name,
        lastname = user.lastname,
        phone = user.phone,
        email = user.email,
        rol = user.rol,
        createdAt = user.createdAt
    });
}

        [HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterModel model)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var exists = await _context.Users
        .AnyAsync(u => u.email == model.Email);

    if (exists)
        return Conflict(new { message = "El correo ya está registrado" });

    // ✅ Verifica también por cédula
    var existsCedula = await _context.Users
        .AnyAsync(u => u.cedula == model.Cedula);

    if (existsCedula)
        return Conflict(new { message = "La cédula ya está registrada" });

    // REGISTRO
    var user = new User
{
    cedula = model.Cedula,
    name = model.Name,
    lastname = model.Lastname,
    phone = model.Phone,
    email = model.Email,
    password = BCrypt.Net.BCrypt.HashPassword(model.Password), // ✅ Encripta
    rol = "Customer",
    createdAt = DateTime.Now
};

    // LOGIN en AuthController
    if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.password)) // ✅ Verifica
        return Unauthorized(new { message = "Cédula o contraseña incorrectos" });

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Usuario creado correctamente" });
}
    }
}