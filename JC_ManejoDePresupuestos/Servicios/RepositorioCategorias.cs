using AutoMapper;
using ManejoDePresupuestos.Models;
using Microsoft.EntityFrameworkCore;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(CategoríaViewModel categoríaViewModel);
        Task Borrar(int Id, string UsuarioId);
        Task Crear(CategoríaViewModel categoríaViewModel, string UsuarioId);
        Task<IEnumerable<CategoríaViewModel>> ObtenerListado(string UsuarioId);
        Task<CategoríaViewModel> ObtenerPorId(int Id, string UsuarioId);
    }
    public class RepositorioCategorias : IRepositorioCategorias
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public RepositorioCategorias(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task Crear(CategoríaViewModel categoríaViewModel, string UsuarioId) 
        {
            var Categoria = mapper.Map<Categoria>(categoríaViewModel);
            Categoria.UsuarioId = UsuarioId;
            context.Add(Categoria);
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<CategoríaViewModel>> ObtenerListado(string UsuarioId)
        {
            var Listado = await context.Categorias.Where(x => x.UsuarioId == UsuarioId).ToListAsync();
            return mapper.Map<List<CategoríaViewModel>>(Listado);
        }
        public async Task<CategoríaViewModel> ObtenerPorId(int Id, string UsuarioId)
        {
            var Categoria = await context.Categorias.FirstOrDefaultAsync(x => x.Id == Id && x.UsuarioId == UsuarioId);
            return mapper.Map<CategoríaViewModel>(Categoria);
        }
        public async Task Borrar(int Id, string UsuarioId)
        {
            var Categoria = await context.Categorias.FirstOrDefaultAsync(x => x.Id == Id && x.UsuarioId == UsuarioId);
            context.Remove(Categoria);
            await context.SaveChangesAsync();
        }

        public async Task Actualizar(CategoríaViewModel categoríaViewModel)
        {
            var Categoria = await context.Categorias.FirstOrDefaultAsync(x => x.Id == categoríaViewModel.Id);
            Categoria = mapper.Map(categoríaViewModel, Categoria);
            await context.SaveChangesAsync();
        }

    }
}
