using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Utilidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ManejoDePresupuestos.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<NewIdentityUser> signInManager;

        public HomeController(ILogger<HomeController> logger,SignInManager<NewIdentityUser> signInManager)
        {
            _logger = logger;
            this.signInManager = signInManager;
        }

        public IActionResult Index()
        {
            
            return (signInManager.IsSignedIn(HttpContext.User))? RedirectToAction("Index","Cuentas"): RedirectToAction("LogIn", "Registro");
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
}