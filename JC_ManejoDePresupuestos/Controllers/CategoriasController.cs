using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using ManejoDePresupuestos.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManejoDePresupuestos.Controllers
{
    [Authorize]
    public class CategoriasController: Controller
    {
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IGetUserInfo getUserInfo;

        public CategoriasController(IRepositorioCategorias repositorioCategorias, IGetUserInfo getUserInfo)
        {
            this.repositorioCategorias = repositorioCategorias;
            this.getUserInfo = getUserInfo;
        }

        [HttpGet]
        public async Task<ActionResult>Index() 
        {
            var UsuarioId = await getUserInfo.GetId();
            var Categorias = await repositorioCategorias.ObtenerListado(UsuarioId);
            return View(Categorias);    
        }

        [HttpGet]
        public ActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(CategoríaViewModel categoríaViewModel)
        {
            
            if (!ModelState.IsValid)
            {
                return View(categoríaViewModel);
            }
            var UsuarioId = await getUserInfo.GetId();
            await repositorioCategorias.Crear(categoríaViewModel,UsuarioId);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> Editar(int Id) 
        {
            var UsuarioId = await getUserInfo.GetId();
            var Categoria = await repositorioCategorias.ObtenerPorId(Id,UsuarioId);
            if (Categoria is null)
            {
                return View("ErrorGenerico");
            }
            return View(Categoria);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(int Id,CategoríaViewModel categoríaViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(categoríaViewModel);
            }
            var UsuarioId = await getUserInfo.GetId();
           
            var Categoria = await repositorioCategorias.ObtenerPorId(Id, UsuarioId);
            if (Categoria is null)
            {
                return View("ErrorGenerico");
            }
            await repositorioCategorias.Actualizar(categoríaViewModel);
            return RedirectToAction("Index");

        }
        [HttpGet]
        public async Task<ActionResult> Borrar(int Id)
        {
            var UsuarioId = await getUserInfo.GetId();
            var Categoria = await repositorioCategorias.ObtenerPorId(Id, UsuarioId);
            if (Categoria is null)
            {
                return View("ErrorGenerico");
            }
            return View(Categoria);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrarCategoria(int Id)
        {
            var UsuarioId = await getUserInfo.GetId();

            var Categoria = await repositorioCategorias.ObtenerPorId(Id, UsuarioId);
            if (Categoria is null)
            {
                return View("ErrorGenerico");
            }
            await repositorioCategorias.Borrar(Id,UsuarioId);
            return RedirectToAction("Index");
        }

    }
}
