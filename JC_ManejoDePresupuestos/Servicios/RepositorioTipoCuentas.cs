using AutoMapper;
using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Utilidades;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace ManejoDePresupuestos.Servicios
{
    
    public interface IRepositorioTipoCuentas
    {
        Task Actualizar(int Id, string Nombre, string UsuarioId);
        Task<int> Crear(TipoCuentaViewModel viewModel,string UsuarioId);
        Task Eliminar(int Id, string UsuarioId);
        Task<IEnumerable<MostrarTipoCuentaViewModel>> ObtenerListado(string UsuarioId);
        Task<MostrarTipoCuentaViewModel> ObtenerPorId(int Id, string UsuarioId);
        Task<bool> YaExisteNombre(string Nombre, string UsuarioId);
    }
    public class RepositorioTipoCuentas : IRepositorioTipoCuentas
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        

        public RepositorioTipoCuentas(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<bool> YaExisteNombre(string Nombre, string UsuarioId)
        {
            var Existe = await context.TipoCuentas.AnyAsync(x => x.UsuarioId == UsuarioId && x.Nombre == Nombre);
            return Existe;
        }
        public async Task<int> Crear(TipoCuentaViewModel viewModel, string UsuarioId)
        {
            var TipoCuentaDB = mapper.Map<TipoCuenta>(viewModel);
            TipoCuentaDB.UsuarioId = UsuarioId;
            await AsignarOrden(TipoCuentaDB,UsuarioId);
            context.Add(TipoCuentaDB);
            await context.SaveChangesAsync();
            return TipoCuentaDB.Id;
        }
        public async Task<IEnumerable<MostrarTipoCuentaViewModel>> ObtenerListado(string UsuarioId) 
        {
            var Listado = await context.TipoCuentas.Where(x=> x.UsuarioId == UsuarioId).
                OrderBy(x=> x.Orden).
                ToListAsync();
            
            var ListadoViewModel = mapper.Map<List<MostrarTipoCuentaViewModel>>(Listado);
            return ListadoViewModel;
        }
        public async Task Actualizar(int Id, string Nombre,string UsuarioId) 
        {
            var TipoCuenta = await context.TipoCuentas.FirstOrDefaultAsync(x=> x.Id == Id && x.UsuarioId == UsuarioId);
            TipoCuenta.Nombre = Nombre;
            context.Entry(TipoCuenta).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }
        public async Task<MostrarTipoCuentaViewModel> ObtenerPorId(int Id,string UsuarioId)
        {
            var TipoCuenta = await context.TipoCuentas.FirstOrDefaultAsync(x => x.Id == Id && x.UsuarioId == UsuarioId);
            return mapper.Map<MostrarTipoCuentaViewModel>(TipoCuenta);
        }
        public async Task Eliminar(int Id, string UsuarioId) 
        {
            var TipoCuenta = await context.TipoCuentas.FirstOrDefaultAsync(x => x.Id == Id && x.UsuarioId == UsuarioId);
            context.Remove(TipoCuenta);
            await context.SaveChangesAsync();
        }

        private async Task AsignarOrden(TipoCuenta tipoCuenta, string UsuarioId)
        {
            var TiposCuentas = await context.TipoCuentas.Where(x => x.UsuarioId == UsuarioId).ToListAsync();
            var TotalTiposCuentas = TiposCuentas.Count;
            for (int i = 0, j = 1; i < TotalTiposCuentas; i++, j++)
            {
                TiposCuentas[i].Orden = j;

            }
            tipoCuenta.Orden = TotalTiposCuentas + 1;
        }


        ////El siguiente método sirve por si queremos cambiar el modo en el que se guarda el orden de los registros
        ////En este caso el usuario por medio de los Ids definirá el orden
        ////Se puede utilizar AJAX con el fetch api para que manden los ids mientras que el usuario elija por medio de una interfaz arrastrable.
        ////Lo importante es que los datos que haga el usuario en el cliente persistirán en la base de datos y siempre al recargar la página se mostrará el orden que el usuario eligió
        ////Se debe hacer el método público para que se pueda llamar desde la FETCH API. lo siguiente es solo un ejemplo hard codeado.
        ////Los Ids los recibiremos como un arreglo de enteros y los utilizaremos para la actualización, la variable Ids de abajo está hard codeada y obviamente es solo un ejemplo, asi que no sirve.
        //private readonly int[] Ids = new int[6] { 14,1, 5,13,19,18 };
        //private async Task AsignarOrdenComoQuiereElUsuario(TipoCuenta tipoCuenta,string UsuarioId)
        //{

        //    for (int i = 0 , j = 1; i < Ids.Length; i++ )
        //    {
        //        var TipoCuenta = await context.TipoCuentas.FirstOrDefaultAsync(x => x.Id == Ids[i] && x.UsuarioId == UsuarioId);
        //        if (TipoCuenta is not null)
        //        {
        //            TipoCuenta.Orden = j;
        //            j++;
        //        }
        //    }
        //    tipoCuenta.Orden = Ids.Length + 1;
        //}

    }
}
