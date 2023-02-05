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
            var TiposCuentas = await ObtenerListadoTipoCuentas(UsuarioId);
            var modelo = PrepararModelo(TiposCuentas);
            return View(modelo);
        }
        [HttpPost]
        public async Task<ActionResult> Crear(CuentaViewModel cuentaViewModel) 
        {
            
            var UsuarioId = await getUserInfo.GetId();
           
            if (!ModelState.IsValid)
            {
                var TiposCuentas = await repositorioTipoCuentas.ObtenerListado(UsuarioId);
                var modelo = new CuentaCreacionViewModel();
                if (TiposCuentas.Count() > 0)
                {
                    modelo.TiposCuentas = TiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
                    return View(modelo);

                }
                ViewBag.TiposCuentas = "No hay Tipos cuentas registrados";
                return View(modelo);
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
                var TiposCuentas = await ObtenerListadoTipoCuentas(UsuarioId);
                var modelo = PrepararModelo(TiposCuentas);
                return View(modelo);

            }
            await repositorioCuentas.Crear(cuentaViewModel,UsuarioId);
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
       
    }
}
