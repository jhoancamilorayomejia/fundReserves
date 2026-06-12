using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Models;
using FoundReserves.Data;
using FoundReserves.Services;
using FoundReserves.ViewModels;

namespace FoundReserves.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            IEmailService emailService)
        {
            _logger       = logger;
            _context      = context;
            _emailService = emailService;
        }

        // ══════════════════════════════════════
        // LOGIN
        // ══════════════════════════════════════
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]  //seguridad web
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Por favor completa todos los campos.";
                return View("Index");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.cedula == model.Cedula);
            
            // Verifica credenciales con BCrypt
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.password))
            {
                ViewBag.Error = "Cédula o contraseña incorrectos.";
                return View("Index");
            }
            // Guarda datos en sesión
            HttpContext.Session.SetInt32("IdUser",    user.iduser);
            HttpContext.Session.SetString("UserEmail",    user.email);
            HttpContext.Session.SetString("UserRol",      user.rol);
            HttpContext.Session.SetString("UserCedula",   user.cedula);
            HttpContext.Session.SetString("UserName",     user.name);
            HttpContext.Session.SetString("UserLastname", user.lastname);
            HttpContext.Session.SetString("UserPhone",    user.phone);

            if (user.rol == "Admin")    return RedirectToAction("Dashboard");
            if (user.rol == "Customer") return RedirectToAction("DashboardCustomer");

            return RedirectToAction("Index");
        }

        // ══════════════════════════════════════
        // Dashboard Admin or customer
        // ══════════════════════════════════════
        public IActionResult Dashboard()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var rol   = HttpContext.Session.GetString("UserRol");

            if (email == null || rol != "Admin")
                return RedirectToAction("Index");

            ViewBag.Email    = email;
            ViewBag.Rol      = rol;
            ViewBag.Cedula   = HttpContext.Session.GetString("UserCedula");
            ViewBag.Name     = HttpContext.Session.GetString("UserName");
            ViewBag.Lastname = HttpContext.Session.GetString("UserLastname");
            ViewBag.Phone    = HttpContext.Session.GetString("UserPhone");

            return View();
        }

        public IActionResult DashboardCustomer()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var rol   = HttpContext.Session.GetString("UserRol");
        //Esta sesión es la que todos los controladores API verifican antes de ejecutar cualquier
            if (email == null || rol != "Customer")
                return RedirectToAction("Index");

            ViewBag.Email    = email;
            ViewBag.Rol      = rol;
            ViewBag.IdUser   = HttpContext.Session.GetInt32("IdUser");
            ViewBag.Cedula   = HttpContext.Session.GetString("UserCedula");
            ViewBag.Name     = HttpContext.Session.GetString("UserName");
            ViewBag.Lastname = HttpContext.Session.GetString("UserLastname");
            ViewBag.Phone    = HttpContext.Session.GetString("UserPhone");

            return View("~/Views/Customer/DashboardCustomer.cshtml");
        }

        public IActionResult ReservesCustomer(int? idSede)
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var rol   = HttpContext.Session.GetString("UserRol");

            if (email == null || rol != "Customer")
                return RedirectToAction("Index");

            ViewBag.Email    = email;
            ViewBag.Rol      = rol;
            ViewBag.IdUser   = HttpContext.Session.GetInt32("IdUser");
            ViewBag.Cedula   = HttpContext.Session.GetString("UserCedula");
            ViewBag.Name     = HttpContext.Session.GetString("UserName");
            ViewBag.Lastname = HttpContext.Session.GetString("UserLastname");
            ViewBag.Phone    = HttpContext.Session.GetString("UserPhone");
            ViewBag.IdSede   = idSede;

            return View("~/Views/Customer/reservesCustomer.cshtml");
        }

        // ══════════════════════════════════════
        // RECUPERACIÓN DE CONTRASEÑA
        // ══════════════════════════════════════

        // GET: /Home/ForgotPassword
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        // POST: /Home/ForgotPassword   procesar el email
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.email == email);   //verifica que el email este en la BD
 
            // Siempre redirigimos para no revelar si el correo existe
            if (user != null)  //si el correo no existe aun asi procesa
            {
                var token = Guid.NewGuid().ToString();

                user.ResetToken           = token;
                user.ResetTokenExpiration = DateTime.Now.AddHours(1);  //genera el token para una hora
                await _context.SaveChangesAsync(); // se guarda en BD

                var resetLink = Url.Action(
                    "ChangePassword", "Home",   //aqui muestra el paso siguiente
                    new { token, email },
                    Request.Scheme
                );
                //aqui construyo el enlace para que se muestre en el correo
                var subject = "Recuperación de contraseña — FoundReserves";
                var body    = $@"
                    <div style='font-family:sans-serif;max-width:480px;margin:auto;
                                padding:24px;border:1px solid #e5e7eb;border-radius:8px'>
                        <h2 style='color:#635bff;margin-top:0'>Recuperación de contraseña</h2>
                        <p>Hola <strong>{user.name} {user.lastname}</strong>,</p>
                        <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta.</p>
                        <p>Haz clic en el siguiente botón (válido por <strong>1 hora</strong>):</p>
                        <a href='{resetLink}' style='
                            display:inline-block;padding:12px 28px;
                            background:#635bff;color:#fff;border-radius:6px;
                            text-decoration:none;font-weight:600;font-size:15px;
                            margin:12px 0;'>
                            Restablecer contraseña
                        </a>
                        <p style='margin-top:20px;color:#6b7280;font-size:12px;'>
                            Si no solicitaste esto, ignora este correo. Tu contraseña no cambiará.
                        </p>
                        <hr style='border:none;border-top:1px solid #e5e7eb;margin:20px 0'/>
                        <p style='color:#9ca3af;font-size:11px;margin:0'>FoundReserves</p>
                    </div>";

                await _emailService.SendEmailAsync(email, subject, body);
            }

            return RedirectToAction("EmailSent");
        }

        // GET: /Home/ChangePassword?token=...&email=...
        [HttpGet]
        public IActionResult ChangePassword(string token, string email)
        {
            var model = new ChangePasswordViewModel
            {
                Token = token,
                Email = email
            };
            return View(model);
        }

        // POST: /Home/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users    //validando el token BD
                .FirstOrDefaultAsync(u =>
                    u.email                == model.Email &&
                    u.ResetToken           == model.Token &&
                    u.ResetTokenExpiration >  DateTime.Now);

            if (user == null)
            {
                ModelState.AddModelError("", "El enlace es inválido o ha expirado.");
                return View(model);
            }

            user.password             = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            user.ResetToken           = null;   // ← token eliminado
            user.ResetTokenExpiration = null;   // ← expiración eliminada
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contraseña actualizada correctamente. Ya puedes iniciar sesión.";
            return RedirectToAction("Index");
        }

        // GET: /Home/EmailSent
        public IActionResult EmailSent() => View();

        // ══════════════════════════════════════
        // LOGOUT / OTROS
        // ══════════════════════════════════════
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}