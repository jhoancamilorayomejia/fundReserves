using Microsoft.AspNetCore.Mvc;
using FoundReserves.Data;

namespace FoundReserves.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase  // 👈 ControllerBase, no Controller
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "Conexión exitosa", timestamp = DateTime.Now });
        }
    }
}