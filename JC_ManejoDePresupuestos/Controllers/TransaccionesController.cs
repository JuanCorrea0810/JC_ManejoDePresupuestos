using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using ManejoDePresupuestos.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Controllers
{
    [Authorize]
    public class TransaccionesController:Controller
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IGetUserInfo getUserInfo;

        public TransaccionesController(IRepositorioTransacciones repositorioTransacciones,IRepositorioCuentas repositorioCuentas,IRepositorioCategorias repositorioCategorias, IGetUserInfo getUserInfo)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.getUserInfo = getUserInfo;
        }
        [HttpGet]
        public async Task<ActionResult> Index() 
        {
            var UsuarioId = await getUserInfo.GetId();
            var Transacciones = await repositorioTransacciones.ObtenerListado(UsuarioId);
            return View(Transacciones);
        }

        [HttpGet]
        public async Task<ActionResult> Crear() 
        {
            var UsuarioId = await getUserInfo.GetId();
            var modelo = await PrepararModelo(UsuarioId);
            return View(modelo);
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(TransaccionCreacionViewModel transaccionViewModel)
        {
            var UsuarioId = await getUserInfo.GetId();
            if (!ModelState.IsValid)
            {
                transaccionViewModel.Cuentas = await ObtenerCuentas(UsuarioId);
                transaccionViewModel.Categorias = await ObtenerCategorias(UsuarioId, transaccionViewModel.TipoOperacionId);
                return View(transaccionViewModel);
            }
            var Cuenta = await repositorioCuentas.ObtenerPorId(transaccionViewModel.CuentaId,UsuarioId);
            if (Cuenta is null)
            {
                return View("ErrorGenerico");
            }
            var Categoria = await repositorioCategorias.ObtenerPorId(transaccionViewModel.CategoriaId, UsuarioId);
            if (Categoria is null)
            {
                return View("ErrorGenerico");
            }
            //No importa como haya pasado el valor el usuario si con negativo o positivo, lo único que va a importar es si es un gasto o un ingreso
            transaccionViewModel.Monto = Math.Abs(transaccionViewModel.Monto);
            if (transaccionViewModel.TipoOperacionId == TipoOperacionViewModel.Gasto)
            {
                transaccionViewModel.Monto *= -1;
            }
            await repositorioTransacciones.Crear(transaccionViewModel, UsuarioId);
            return RedirectToAction("Index");

        }

        [HttpGet]
        public async Task<ActionResult> Editar(int Id) 
        
        {
            var UsuarioId = await getUserInfo.GetId();
            var Transaccion = await repositorioTransacciones.ObtenerPorId(Id,UsuarioId);
            if (Transaccion is null)
            {
                return View("ErrorGenerico");
            }
            var modelo = repositorioTransacciones.MapearAModeloDeActualizacion(Transaccion);
            modelo.Categorias = await ObtenerCategorias(UsuarioId,modelo.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(UsuarioId);
            return View(modelo);
              
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(ActualizarTransaccionViewModel transaccionViewModel) 
        {
            var UsuarioId = await getUserInfo.GetId();
            if (!ModelState.IsValid)
            {
                transaccionViewModel.Categorias = await ObtenerCategorias(UsuarioId, transaccionViewModel.TipoOperacionId);
                transaccionViewModel.Cuentas = await ObtenerCuentas(UsuarioId);
                return View(transaccionViewModel);
            }
            var Transaccion = await repositorioTransacciones.ObtenerPorId(transaccionViewModel.Id,UsuarioId);
            if (Transaccion is null)
            {
                return View("ErrorGenerico");
            }
            var Cuenta = await repositorioCuentas.ObtenerPorId(transaccionViewModel.CuentaId,UsuarioId);
            if (Cuenta is null)
            {
                return View("ErrorGenerico");
            }
            var Categoria = await repositorioCategorias.ObtenerPorId(transaccionViewModel.CategoriaId,UsuarioId);
            if (Categoria is null)
            {
                return View("ErrorGenerico");
            }
            //No importa como haya pasado el valor el usuario si con negativo o positivo, lo único que va a importar es si es un gasto o un ingreso
            transaccionViewModel.Monto = Math.Abs(transaccionViewModel.Monto);
            if (transaccionViewModel.TipoOperacionId == TipoOperacionViewModel.Gasto)
            {
                transaccionViewModel.Monto *= -1;
            }
            await repositorioTransacciones.Actualizar(transaccionViewModel,UsuarioId);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> Borrar(int Id)
        {
            var UsuarioId = await getUserInfo.GetId();
            var Transaccion = await repositorioTransacciones.ObtenerPorId(Id, UsuarioId);
            if (Transaccion is null)
            {
                return View("ErrorGenerico");
            }
            return View(Transaccion);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrarTransaccion(int Id)

        {
            var UsuarioId = await getUserInfo.GetId();
            var Transaccion = await repositorioTransacciones.ObtenerPorId(Id, UsuarioId);
            if (Transaccion is null)
            {
                return View("ErrorGenerico");
            }
            await repositorioTransacciones.Borrar(Id,UsuarioId);
            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> ObtenerCategoriasPorTipoOperacion([FromBody] TipoOperacionViewModel TipoOperacionId) 
        {
            var UsuarioId = await getUserInfo.GetId();
            var Categorias = await ObtenerCategorias(UsuarioId, TipoOperacionId);
            if (!Categorias.Any())
            {
                ViewBag.Categorias = "No hay catagorías registradas";
            }
            else
            {
                ViewBag.Categorias = null;
            }
            return Ok(Categorias);
        }
        private async Task<TransaccionCreacionViewModel> PrepararModelo(string UsuarioId) 
        {
            var modelo = new TransaccionCreacionViewModel();
            modelo.Cuentas = await ObtenerCuentas(UsuarioId);
            modelo.Categorias = await ObtenerCategorias(UsuarioId,modelo.TipoOperacionId);
            if (!modelo.Cuentas.Any())
            {
                ViewBag.Cuentas = "No hay cuentas registradas";
            }
            return modelo;
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(string UsuarioId) 
        {
            var Cuentas = await repositorioCuentas.ObtenerListado(UsuarioId);
            return Cuentas.Select(x => new SelectListItem(x.Nombre,x.Id.ToString()));
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(string UsuarioId,TipoOperacionViewModel TipoOperacionId)
        {
            var categorias = await repositorioCategorias.ObtenerListado(UsuarioId,TipoOperacionId);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
