﻿using AutoMapper;
using ManejoDePresupuestos.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace ManejoDePresupuestos.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(ActualizarTransaccionViewModel transaccionViewModel, string UsuarioId);
        Task Borrar(int Id, string UsuarioId);
        Task Crear(TransaccionCreacionViewModel transaccionViewModel, string UsuarioId);
        ActualizarTransaccionViewModel MapearAModeloDeActualizacion(TransaccionCreacionViewModel transaccion);
        Task<IEnumerable<TransaccionCreacionViewModel>> ObtenerListado(string UsuarioId, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<TransaccionCreacionViewModel>> ObtenerPorCuentas(TransaccionesPorCuenta viewModel, string UsuarioId);
        Task<TransaccionCreacionViewModel> ObtenerPorId(int Id, string UsuarioId);
        Task<IEnumerable<TransaccionesMensuales>> ObtenerTransaccionesPorMes(string UsuarioId, int Year);
        Task<IEnumerable<TransaccionesSemanalesViewModel>> ObtenerTransaccionesPorSemana(string UsuarioId, DateTime FechaInicio, DateTime FechaFin);
    }
    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public RepositorioTransacciones(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }
        public async Task<IEnumerable<TransaccionCreacionViewModel>> ObtenerListado(string UsuarioId,DateTime fechaInicio, DateTime fechaFin)
        {
            var Transacciones = await context.Transacciones.Include(x => x.Categoria)
                                                           .Include(x => x.Cuenta)
                                                           .Where(x => x.UsuarioId == UsuarioId && (x.FechaTransaccion >= fechaInicio && x.FechaTransaccion <= fechaFin))
                                                           .OrderByDescending(x=> x.FechaTransaccion)
                                                           .ToListAsync();
            return mapper.Map<List<TransaccionCreacionViewModel>>(Transacciones);
        }

        public async Task Crear(TransaccionCreacionViewModel transaccionViewModel, string UsuarioId)
        {
            var parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@UsuarioId", UsuarioId),
                 new SqlParameter("@FechaTransaccion", transaccionViewModel.FechaTransaccion),
                 new SqlParameter("@Monto", transaccionViewModel.Monto),
                 new SqlParameter("@CategoriaId", transaccionViewModel.CategoriaId),
                 new SqlParameter("@CuentaId", transaccionViewModel.CuentaId),
                 new SqlParameter("@Nota", transaccionViewModel.Nota)
            };
            
            await context.Database.ExecuteSqlRawAsync("EXEC CrearTransaccion @UsuarioId, @FechaTransaccion, @Monto, @CategoriaId, @CuentaId, @Nota", parameters);
        }

        public async Task Actualizar(ActualizarTransaccionViewModel transaccionViewModel, string UsuarioId)
        {
            var parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@Id", transaccionViewModel.Id),
                 new SqlParameter("@UsuarioId", UsuarioId),
                 new SqlParameter("@FechaTransaccion", transaccionViewModel.FechaTransaccion),
                 new SqlParameter("@Monto", transaccionViewModel.Monto),
                 new SqlParameter("@MontoAnterior", transaccionViewModel.MontoAnterior),
                 new SqlParameter("@CuentaId", transaccionViewModel.CuentaId),
                 new SqlParameter("@CuentaAnteriorId", transaccionViewModel.CuentaAnteriorId),
                 new SqlParameter("@CategoriaId", transaccionViewModel.CategoriaId), 
                 new SqlParameter("@Nota", transaccionViewModel.Nota)
            };

            await context.Database.ExecuteSqlRawAsync("EXEC ActualizarTransaccion @Id,@UsuarioId,@FechaTransaccion,@Monto,@MontoAnterior,@CuentaId,@CuentaAnteriorId,@CategoriaId,@Nota", parameters);
        }

        public async Task<TransaccionCreacionViewModel> ObtenerPorId(int Id, string UsuarioId) 
        {
            var Transaccion = await context.Transacciones.Include(x=> x.Categoria).FirstOrDefaultAsync(x=> x.Id == Id && x.UsuarioId == UsuarioId);
            return mapper.Map<TransaccionCreacionViewModel>(Transaccion);
        }

        public async Task Borrar(int Id,string UsuarioId) 
        {
            var Transaccion = await context.Transacciones.FirstOrDefaultAsync(x=> x.Id == Id && x.UsuarioId == UsuarioId);
            context.Remove(Transaccion);
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<TransaccionCreacionViewModel>> ObtenerPorCuentas(TransaccionesPorCuenta viewModel,string UsuarioId) 
        {
            var Transacciones = await context.Transacciones.Include(x => x.Categoria)
                                                           .Include(x => x.Cuenta)
                                                           .Where(x => x.UsuarioId == UsuarioId && x.CuentaId == viewModel.CuentaId && (x.FechaTransaccion >= viewModel.FechaInicio && x.FechaTransaccion <= viewModel.FechaFin)).ToListAsync();
            return mapper.Map<List<TransaccionCreacionViewModel>>(Transacciones);

        }

        public ActualizarTransaccionViewModel MapearAModeloDeActualizacion(TransaccionCreacionViewModel transaccion) 
        {
            return mapper.Map<ActualizarTransaccionViewModel>(transaccion);
        }
        public async Task<IEnumerable<TransaccionesSemanalesViewModel>> ObtenerTransaccionesPorSemana(string UsuarioId, DateTime FechaInicio, DateTime FechaFin)
        {

            var parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@UsuarioId", UsuarioId),
                 new SqlParameter("@FechaInicio", FechaInicio),
                 new SqlParameter("@FechaFin", FechaFin)
            };
            var resultado = await context.TransaccionesPorSemana.FromSqlRaw("EXEC ReporteSemanal @UsuarioId,@FechaInicio,@FechaFin", parameters.ToArray()).
                ToListAsync();
            return resultado;
        }
        public async Task<IEnumerable<TransaccionesMensuales>> ObtenerTransaccionesPorMes(string UsuarioId, int Year) 
        {
            var parameters = new List<SqlParameter>()
            {
                 new SqlParameter("@UsuarioId", UsuarioId),
                 new SqlParameter("@Year", Year),
            };

            var resultado = await context.TransaccionesPorMes.FromSqlRaw("EXEC ReporteMensual @UsuarioId,@Year", parameters.ToArray()).ToListAsync();
            return resultado;
        }

    }
}
