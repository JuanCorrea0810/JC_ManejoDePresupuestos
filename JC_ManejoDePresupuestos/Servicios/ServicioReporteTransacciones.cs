using AutoMapper;
using ManejoDePresupuestos.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace ManejoDePresupuestos.Servicios
{
    public interface IServicioReporteTransacciones
    {
        Task<IEnumerable<TransaccionesSemanalesViewModel>> ObtenerTransaccionesPorSemana(int month, int year, string UsuarioId, dynamic ViewBag);
        Task<ReportesTransacciones> ReportePorCuentas(int month, int year, int Id, string UsuarioId, dynamic ViewBag);
        Task<ReportesTransacciones> ReportesTransacciones(int month, int year,string UsuarioId, dynamic ViewBag);
    }
    public class ServicioReporteTransacciones: IServicioReporteTransacciones
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IMapper mapper;
        private readonly HttpContext httpContext;

        public ServicioReporteTransacciones(IRepositorioTransacciones repositorioTransacciones, 
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            )
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
            this.httpContext = httpContextAccessor.HttpContext;
        }
        public async Task<ReportesTransacciones> ReportePorCuentas(int month,int year, int Id, string UsuarioId,dynamic ViewBag) 
        {
            var (FechaInicio, FechaFin) = GenerarFechas(month,year);
            var transaccionesPorCuenta = new TransaccionesPorCuenta()
            {
                CuentaId = Id,
                FechaInicio = FechaInicio,
                FechaFin = FechaFin
            };
            var transacciones = await repositorioTransacciones.ObtenerPorCuentas(transaccionesPorCuenta, UsuarioId);
            var modelo = GenerarModelo(transacciones,FechaInicio,FechaFin);
            GenerarViewBag(ViewBag, FechaInicio);
            return modelo;
        }
        public async Task<ReportesTransacciones> ReportesTransacciones(int month,int year,string UsuarioId,dynamic ViewBag) 
        {
            var (FechaInicio, FechaFin) = GenerarFechas(month, year);
            var transacciones = await repositorioTransacciones.ObtenerListado(UsuarioId,FechaInicio,FechaFin);
            var modelo = GenerarModelo(transacciones, FechaInicio, FechaFin);
            GenerarViewBag(ViewBag, FechaInicio);
            return modelo;
        }
        public async Task<IEnumerable<TransaccionesSemanalesViewModel>> ObtenerTransaccionesPorSemana(int month, int year, string UsuarioId, dynamic ViewBag) 
        {
            var (FechaInicio, FechaFin) = GenerarFechas(month, year);
            GenerarViewBag(ViewBag, FechaInicio);
            var Transacciones = await repositorioTransacciones.ObtenerTransaccionesPorSemana(UsuarioId, FechaInicio, FechaFin);
            return Transacciones;
        }
        

        private (DateTime FechaInicio, DateTime FechaFin) GenerarFechas(int month,int year) 
        {
            DateTime FechaInicio;
            DateTime FechaFin;
            if (month <= 0 || month > 12 || year < 1900) //Si no se pasan valores correctos, entonces por defecto se asigna de fecha el mes actual y el año actual
            {
                var hoy = DateTime.Now;
                FechaInicio = new DateTime(hoy.Year, hoy.Month, 1);
            }
            else
            {
                FechaInicio = new DateTime(year, month, 1);
            }
            FechaFin = FechaInicio.AddMonths(1).AddDays(-1); //Fecha fin queda automáticamente como el último día del mes de FechaInicio.
            return (FechaInicio,FechaFin);
        }
        private void GenerarViewBag(dynamic ViewBag, DateTime FechaInicio) 
        {
            ViewBag.mesAnterior = FechaInicio.AddMonths(-1).Month;
            ViewBag.añoAnterior = FechaInicio.AddMonths(-1).Year;
            ViewBag.mesPosterior = FechaInicio.AddMonths(1).Month;
            ViewBag.añoPosterior = FechaInicio.AddMonths(1).Year;
            ViewBag.urlRetorno = httpContext.Request.Path + httpContext.Request.QueryString;
        }
        private ReportesTransacciones GenerarModelo(IEnumerable<TransaccionCreacionViewModel> transacciones, DateTime FechaInicio, DateTime FechaFin) 
        {
            var modelo = new ReportesTransacciones();

            var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion).
                                        GroupBy(x => x.FechaTransaccion).
                                        Select(grupo => new ReportesTransacciones.TransaccionesPorFecha
                                        {
                                            FechaTransaccion = grupo.Key,
                                            Transacciones = grupo.AsEnumerable()
                                        });
            modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            modelo.FechaInicio = FechaInicio;
            modelo.FechaFin = FechaFin;
            return modelo;
        }
    }
}
