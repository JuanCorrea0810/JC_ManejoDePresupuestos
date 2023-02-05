using AutoMapper;
using ManejoDePresupuestos.Models;
using Microsoft.EntityFrameworkCore;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioCuentas
    {
        Task<int> Crear(CuentaViewModel cuentaViewModel, string UsuarioId);
        Task<IEnumerable<CuentaViewModel>> ObtenerListado(string UsuarioId);
        Task<IEnumerable<MostrarCuentaViewModel>> ObtenerListadoConNombre(string UsuarioId);
        Task<CuentaViewModel> ObtenerPorId(int Id, string UsuarioId);
        Task<bool> YaExisteNombre(string Nombre, string UsuarioId, int TipoCuentasId);
    }
    public class RepositorioCuentas: IRepositorioCuentas
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public RepositorioCuentas(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<CuentaViewModel>> ObtenerListado(string UsuarioId) 
        {
            var Listado = await context.Cuentas.Where(x=> x.UsuarioId == UsuarioId).ToListAsync();
            return mapper.Map<List<CuentaViewModel>>(Listado);
        }
        public async Task<int> Crear(CuentaViewModel cuentaViewModel, string UsuarioId) 
        {
            var CuentaDB = mapper.Map<Cuenta>(cuentaViewModel);
            CuentaDB.UsuarioId= UsuarioId;
            context.Add(CuentaDB);
            await context.SaveChangesAsync();
            return CuentaDB.Id;
        }
        public async Task<IEnumerable<MostrarCuentaViewModel>> ObtenerListadoConNombre(string UsuarioId) 
        {
            var Listado = await context.Cuentas.Where(x=> x.UsuarioId == UsuarioId).Include(x=> x.TipoCuentas).ToListAsync();
            return mapper.Map<List<MostrarCuentaViewModel>>(Listado);
        }

        public async Task<bool> YaExisteNombre(string Nombre, string UsuarioId, int TipoCuentasId) 
        {
            var ExisteNombre = await context.Cuentas.AnyAsync(x=> x.Nombre == Nombre && x.UsuarioId == UsuarioId && x.TipoCuentasId == TipoCuentasId);
            return ExisteNombre;
        }
        public async Task<CuentaViewModel> ObtenerPorId(int Id, string UsuarioId) 
        {
            var Cuenta = await context.Cuentas.FirstOrDefaultAsync(x=> x.Id == Id && x.UsuarioId == UsuarioId);
            return mapper.Map<CuentaViewModel>(Cuenta);
        }
    }
}
