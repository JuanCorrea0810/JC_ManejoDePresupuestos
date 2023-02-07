using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using ManejoDePresupuestos.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace ManejoDePresupuestos.Controllers
{
    [Authorize]
    public class CuentasController : Controller
    {
        private readonly IRepositorioTipoCuentas repositorioTipoCuentas;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IGetUserInfo getUserInfo;
        

        public CuentasController(IRepositorioTipoCuentas repositorioTipoCuentas, 
            IRepositorioCuentas repositorioCuentas, 
            IGetUserInfo getUserInfo)
        {
            this.repositorioTipoCuentas = repositorioTipoCuentas;
            this.repositorioCuentas = repositorioCuentas;
            this.getUserInfo = getUserInfo;
        }
        [HttpGet]
        public async Task<ActionResult> Index() 
        {
            var UsuarioId = await getUserInfo.GetId();
            var Cuentas = await repositorioCuentas.ObtenerListadoConNombre(UsuarioId);
            var modelo = Cuentas.
                GroupBy(x=> x.TipoCuenta).
                Select(grupo=> new IndiceCuentasViewModel 
                {
                    TipoCuenta= grupo.Key,
                    Cuentas = grupo.AsEnumerable(),
                }).ToList();    
            return View(modelo);
        }
        [HttpGet]
        public async Task<ActionResult> Crear()  
        {
            var UsuarioId = await getUserInfo.GetId();
            return await MandarModelo(UsuarioId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(CuentaViewModel cuentaViewModel) 
        {
            
            var UsuarioId = await getUserInfo.GetId();
           
            if (!ModelState.IsValid)
            {
                return await MandarModelo(UsuarioId);

            }

            var TipoCuenta = await repositorioTipoCuentas.ObtenerPorId(cuentaViewModel.TipoCuentasId,UsuarioId);
            if (TipoCuenta is null)
            {
                return View("ErrorGenerico");
            }
            var ExisteNombre = await repositorioCuentas.YaExisteNombre(cuentaViewModel.Nombre,UsuarioId,cuentaViewModel.TipoCuentasId);
            if (ExisteNombre)
            {
                ModelState.AddModelError(nameof(cuentaViewModel.Nombre), "No se puede repetir nombre para cuentas que comparte el mismo tipo de cuentas");
                return await MandarModelo(UsuarioId);

            }
            await repositorioCuentas.Crear(cuentaViewModel,UsuarioId);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<ActionResult> Editar(int Id) 
        {
            var UsuarioId = await getUserInfo.GetId();
            var cuenta = await repositorioCuentas.ObtenerPorId(Id,UsuarioId);
            if (cuenta is null)
            {
                return View("ErrorGenerico");
            }
            var modelo = repositorioCuentas.MapearCuenta(cuenta);
            var tiposCuentas = await ObtenerListadoTipoCuentas(UsuarioId);
            modelo.TiposCuentas = tiposCuentas.Select(x=> new SelectListItem(x.Nombre,x.Id.ToString()));         
            return View(modelo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(CuentaViewModel cuentaViewModel)
        {
            var UsuarioId = await getUserInfo.GetId();
            if (!ModelState.IsValid)
            {
                return await MandarModelo(UsuarioId);
            }
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaViewModel.Id, UsuarioId);
            if (cuenta is null)
            {
                return View("ErrorGenerico");
            }
            var ExisteTipoCuentaId = await repositorioTipoCuentas.ObtenerPorId(cuentaViewModel.TipoCuentasId, UsuarioId);
            if (ExisteTipoCuentaId is null)
            {
                return View("ErrorGenerico");
            }
            var ExisteNombre = await repositorioCuentas.YaExisteNombre(cuentaViewModel.Nombre, UsuarioId, cuentaViewModel.TipoCuentasId, cuentaViewModel.Id);
            if (ExisteNombre)
            {
                ModelState.AddModelError(nameof(cuentaViewModel.Nombre), "El tipo de cuenta que elegiste ya tiene otra cuenta registrada con este nombre");
                return await MandarModelo(UsuarioId);
            }
            await repositorioCuentas.Actualizar(cuentaViewModel);
            return RedirectToAction("Index");


        }
        [HttpGet]
        public async Task<ActionResult> Borrar(int Id)
        {
            var UsuarioId = await getUserInfo.GetId();
            var Cuenta = await repositorioCuentas.ObtenerPorId(Id,UsuarioId);
            if (Cuenta is null)
            {
                return View("ErrorGenerico");
            }
            return View(Cuenta);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrarCuenta(int Id) 
        {
            var UsuarioId = await getUserInfo.GetId();
            var Cuenta = await repositorioCuentas.ObtenerPorId(Id, UsuarioId);
            if (Cuenta is null)
            {
                return View("ErrorGenerico");
            }
            await repositorioCuentas.Borrar(Id, UsuarioId);
            return RedirectToAction("Index");
        }
        private async Task<IEnumerable<MostrarTipoCuentaViewModel>> ObtenerListadoTipoCuentas(string UsuarioId) 
        {
            return await repositorioTipoCuentas.ObtenerListado(UsuarioId);
        }
        private CuentaCreacionViewModel PrepararModelo(IEnumerable<MostrarTipoCuentaViewModel> tiposCuentas) 
        {
            var modelo = new CuentaCreacionViewModel();
            if (tiposCuentas.Count() > 0) 
            {
                modelo.TiposCuentas = tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
                return modelo;
            }
            ViewBag.TiposCuentas = "No hay Tipos cuentas registrados";
            return modelo;
        }
        private async Task<ActionResult> MandarModelo(string UsuarioId) 
        {
            var TiposCuentas = await repositorioTipoCuentas.ObtenerListado(UsuarioId);
            var modelo = PrepararModelo(TiposCuentas);
            return View(modelo);
        }
        

    }
}
