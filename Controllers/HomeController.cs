using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FoundReserves.Models;
using FoundReserves.Data;

namespace FoundReserves.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Error = "Por favor completa todos los campos.";
            return View("Index");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.cedula == model.Cedula);

        // Por esta:
       if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.password))
        {
            ViewBag.Error = "Cedula o contraseña incorrectos.";
            return View("Index");
        }

        HttpContext.Session.SetString("UserId", user.iduser.ToString());
        HttpContext.Session.SetString("UserEmail", user.email);
        HttpContext.Session.SetString("UserRol", user.rol);
        HttpContext.Session.SetString("UserCedula", user.cedula);
        HttpContext.Session.SetString("UserName", user.name);
        HttpContext.Session.SetString("UserLastname", user.lastname);
        HttpContext.Session.SetString("UserPhone", user.phone);

        if (user.rol == "Admin")
    {
        return RedirectToAction("Dashboard");
    }

        if (user.rol == "Customer")
    {
        return RedirectToAction("DashboardCustomer");
    }

        return RedirectToAction("Index");
    }

//este es para Amin
    public IActionResult Dashboard()
{
    var email = HttpContext.Session.GetString("UserEmail");
    var rol = HttpContext.Session.GetString("UserRol");

    if (email == null || rol != "Admin")
        return RedirectToAction("Index");

    ViewBag.Email = email;
    ViewBag.Rol = rol;
    ViewBag.Cedula = HttpContext.Session.GetString("UserCedula");
    ViewBag.Name = HttpContext.Session.GetString("UserName");
    ViewBag.Lastname = HttpContext.Session.GetString("UserLastname");
    ViewBag.Phone = HttpContext.Session.GetString("UserPhone");
    

    return View();
}

//Este para Clientes
public IActionResult DashboardCustomer()
{
    var email = HttpContext.Session.GetString("UserEmail");
    var rol = HttpContext.Session.GetString("UserRol");

    if (email == null || rol != "Customer")
        return RedirectToAction("Index");

    ViewBag.Email = email;
    ViewBag.Rol = rol;
    ViewBag.Cedula = HttpContext.Session.GetString("UserCedula");
    ViewBag.Name = HttpContext.Session.GetString("UserName");
    ViewBag.Lastname = HttpContext.Session.GetString("UserLastname");
    ViewBag.Phone = HttpContext.Session.GetString("UserPhone");

    return View("~/Views/Customer/DashboardCustomer.cshtml");
}

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}