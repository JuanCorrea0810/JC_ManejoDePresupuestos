using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using ManejoDePresupuestos.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ManejoDePresupuestos.Controllers
{
    [Authorize]
    public class TipoCuentasController : Controller
    {
        private readonly IRepositorioTipoCuentas repositorio;
        private readonly IGetUserInfo getUserInfo;

        public TipoCuentasController(IRepositorioTipoCuentas repositorio, IGetUserInfo getUserInfo)
        {
            this.repositorio = repositorio;
            this.getUserInfo = getUserInfo;
        }
        [HttpGet]
        public async Task<ActionResult<List<MostrarTipoCuentaViewModel>>> Index()
        {
            var UsuarioId = await getUserInfo.GetId();
            var Listado = await repositorio.ObtenerListado(UsuarioId);
            return View(Listado);
        }
        public ActionResult Crear()
        {
            return View();
        }
      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(TipoCuentaViewModel ViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(ViewModel);
            }
            var UsuarioId = await getUserInfo.GetId();
            var ExisteTipoCuenta = await repositorio.YaExisteNombre(ViewModel.Nombre, UsuarioId);
            if (ExisteTipoCuenta)
            {
                ModelState.AddModelError(nameof(ViewModel.Nombre), $"Ya has asignado el nombre: {ViewModel.Nombre} a un tipo de cuenta");
                return View(ViewModel);
            }
            await repositorio.Crear(ViewModel, UsuarioId);
            return RedirectToAction("Index");

        }
        [HttpGet]
        public async Task<ActionResult> Editar(int Id) 
        {
            var UsuarioId = await getUserInfo.GetId();
            var TipoCuenta = await repositorio.ObtenerPorId(Id,UsuarioId);
            if (TipoCuenta is null)
            {
                return View("ErrorGenerico");
            }
            return View(TipoCuenta);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(int Id,string Nombre)
        {
            var UsuarioId = await getUserInfo.GetId();
            var TipoCuenta = await repositorio.ObtenerPorId(Id, UsuarioId);
            if (TipoCuenta is null)
            {
                return View("ErrorGenerico");
            }
            await repositorio.Actualizar(Id,Nombre,UsuarioId);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> Borrar(int Id) 
        {
            var UsuarioId = await getUserInfo.GetId();
            var TipoCuenta = await repositorio.ObtenerPorId(Id, UsuarioId);
            if (TipoCuenta is null)
            {
                return View("ErrorGenerico");
            }
            return View(TipoCuenta);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrarTipoCuenta(int Id)
        {
            var UsuarioId = await getUserInfo.GetId();
            var TipoCuenta = await repositorio.ObtenerPorId(Id, UsuarioId);
            if (TipoCuenta is null)
            {
                return View("ErrorGenerico");
            }
            await repositorio.Eliminar(Id, UsuarioId);
            return RedirectToAction("Index");
            
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarExisteNombreJS(string nombre)
        {
            var UsuarioId = await getUserInfo.GetId();
            var ExisteTipoCuenta = await repositorio.YaExisteNombre(nombre, UsuarioId);
            if (ExisteTipoCuenta)
            {
                return Json($"Ya has asignado el nombre: {nombre} a un tipo de cuenta");
            }
            return Json(true);
            

        }
    }
}
