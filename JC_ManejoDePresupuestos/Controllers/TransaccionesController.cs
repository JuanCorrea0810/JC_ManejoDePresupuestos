using AutoMapper.Configuration.Conventions;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using ManejoDePresupuestos.Models;
using ManejoDePresupuestos.Servicios;
using ManejoDePresupuestos.Utilidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace ManejoDePresupuestos.Controllers
{
    [Authorize]
    public class TransaccionesController : Controller
    {
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IRepositorioCategorias repositorioCategorias;
        private readonly IGetUserInfo getUserInfo;
        private readonly IServicioReporteTransacciones reporteTransacciones;

        public TransaccionesController(IRepositorioTransacciones repositorioTransacciones,
            IRepositorioCuentas repositorioCuentas,
            IRepositorioCategorias repositorioCategorias,
            IGetUserInfo getUserInfo,
            IServicioReporteTransacciones reporteTransacciones)
        {
            this.repositorioTransacciones = repositorioTransacciones;
            this.repositorioCuentas = repositorioCuentas;
            this.repositorioCategorias = repositorioCategorias;
            this.getUserInfo = getUserInfo;
            this.reporteTransacciones = reporteTransacciones;
        }
        [HttpGet]
        public async Task<ActionResult> Index(int month, int year)
        {
            var UsuarioId = await getUserInfo.GetId();
            var modelo = await reporteTransacciones.ReportesTransacciones(month, year, UsuarioId, ViewBag);
            return View(modelo);
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
            var Cuenta = await repositorioCuentas.ObtenerPorId(transaccionViewModel.CuentaId, UsuarioId);
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
        public async Task<ActionResult> Editar(int Id, string urlRetorno = null)

        {
            var UsuarioId = await getUserInfo.GetId();
            var Transaccion = await repositorioTransacciones.ObtenerPorId(Id, UsuarioId);
            if (Transaccion is null)
            {
                return View("ErrorGenerico");
            }
            var modelo = repositorioTransacciones.MapearAModeloDeActualizacion(Transaccion);
            modelo.Categorias = await ObtenerCategorias(UsuarioId, modelo.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(UsuarioId);
            ViewBag.urlRetorno = urlRetorno;
            return View(modelo);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(ActualizarTransaccionViewModel transaccionViewModel, string urlRetorno = null)
        {
            var UsuarioId = await getUserInfo.GetId();
            if (!ModelState.IsValid)
            {
                transaccionViewModel.Categorias = await ObtenerCategorias(UsuarioId, transaccionViewModel.TipoOperacionId);
                transaccionViewModel.Cuentas = await ObtenerCuentas(UsuarioId);
                return View(transaccionViewModel);
            }
            var Transaccion = await repositorioTransacciones.ObtenerPorId(transaccionViewModel.Id, UsuarioId);
            if (Transaccion is null)
            {
                return View("ErrorGenerico");
            }
            var Cuenta = await repositorioCuentas.ObtenerPorId(transaccionViewModel.CuentaId, UsuarioId);
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
            await repositorioTransacciones.Actualizar(transaccionViewModel, UsuarioId);
            if (urlRetorno is not null)
            {
                return LocalRedirect(urlRetorno);
            }
            return RedirectToAction("Index");

        }
        [HttpGet]
        public async Task<ActionResult> Borrar(int Id, string urlRetorno = null)
        {
            var UsuarioId = await getUserInfo.GetId();
            var Transaccion = await repositorioTransacciones.ObtenerPorId(Id, UsuarioId);
            if (Transaccion is null)
            {
                return View("ErrorGenerico");
            }
            if (urlRetorno is not null)
            {
                ViewBag.urlRetorno = urlRetorno;
            }
            return View(Transaccion);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrarTransaccion(int Id, string urlRetorno = null)

        {
            var UsuarioId = await getUserInfo.GetId();
            var Transaccion = await repositorioTransacciones.ObtenerPorId(Id, UsuarioId);
            if (Transaccion is null)
            {
                return View("ErrorGenerico");
            }
            await repositorioTransacciones.Borrar(Id, UsuarioId);
            if (urlRetorno is not null)
            {
                return LocalRedirect(urlRetorno);
            }
            return RedirectToAction("Index");

        }
        [HttpGet]
        public async Task<ActionResult> Diario(int month, int year)
        {
            var UsuarioId = await getUserInfo.GetId();
            ReportesTransacciones transacciones = await reporteTransacciones.ReportesTransacciones(month, year, UsuarioId, ViewBag);
            return View(transacciones);
        }
        [HttpGet]
        public async Task<ActionResult> Semanal(int month, int year)
        {
            var UsuarioId = await getUserInfo.GetId();
            IEnumerable<TransaccionesSemanalesViewModel> TransaccionesPorSemana =
                await reporteTransacciones.ObtenerTransaccionesPorSemana(month, year, UsuarioId, ViewBag);

            var Agrupadas = TransaccionesPorSemana.GroupBy(x => x.Semana).
                            Select(x => new TransaccionesSemanalesViewModel()
                            {
                                Semana = x.Key,
                                Ingresos = x.Where(x => x.TipoOperacionId == TipoOperacionViewModel.Ingreso).Select(x => x.Monto).FirstOrDefault(),
                                Gastos = x.Where(x => x.TipoOperacionId == TipoOperacionViewModel.Gasto).Select(x => x.Monto).FirstOrDefault(),


                            }).ToList();
            if (month == 0 || year == 0)
            {
                var hoy = DateTime.Today;
                month = hoy.Month;
                year = hoy.Year;
            }
            var Fecha = new DateTime(year, month, 1);
            var DiasDelMes = Enumerable.Range(1, Fecha.AddMonths(1).AddDays(-1).Day);
            var DiasDivididos = DiasDelMes.Chunk(7).ToList();

            for (int i = 0; i < DiasDivididos.Count; i++)
            {
                var semana = i + 1;
                var FechaInicio = new DateTime(year, month, DiasDivididos[i].First());
                var FechaFin = new DateTime(year, month, DiasDivididos[i].Last());
                var grupoSemana = Agrupadas.FirstOrDefault(x => x.Semana == semana);
                if (grupoSemana is null)
                {
                    Agrupadas.Add(new TransaccionesSemanalesViewModel()
                    {
                        Semana = semana,
                        FechaInicio = FechaInicio,
                        FechaFin = FechaFin
                    });
                }
                else
                {
                    grupoSemana.FechaInicio = FechaInicio;
                    grupoSemana.FechaFin = FechaFin;
                }
            }
            Agrupadas = Agrupadas.OrderByDescending(x => x.Semana).ToList();
            var modelo = new ReporteSemanaViewModel
            {
                TransaccionesPorSemana = Agrupadas,
                Fecha = Fecha
            };
            return View(modelo);
        }
        [HttpGet]
        public async Task<ActionResult> Mensual(int year)
        {
            var UsuarioId = await getUserInfo.GetId();
            IEnumerable<TransaccionesMensuales> Transacciones = await reporteTransacciones.ObtenerTransaccionesPorMes(year, UsuarioId, ViewBag);
            var TransaccionesAgrupadas = Transacciones.GroupBy(x => x.Mes).
                         Select(x => new TransaccionesMensuales()
                         {
                             Mes = x.Key,
                             IngresosMensuales = x.Where(x => x.TipoOperacionId == TipoOperacionViewModel.Ingreso).Select(x => x.Monto).FirstOrDefault(),
                             GastosMensuales = x.Where(x => x.TipoOperacionId == TipoOperacionViewModel.Gasto).Select(x => x.Monto).FirstOrDefault()

                         }).ToList();
            for (int i = 0; i < 12; i++)
            {
                int mes = i + 1;
                if (!TransaccionesAgrupadas.Any(x => x.Mes == mes))
                {
                    TransaccionesAgrupadas.Add(new TransaccionesMensuales()
                    {
                        Mes = mes
                    }
                    );
                }

            }
            TransaccionesAgrupadas = TransaccionesAgrupadas.OrderBy(x => x.Mes).ToList();

            var modelo = new ReporteTransaccionesPorMes();
            modelo.TransaccionesAgrupadas = TransaccionesAgrupadas;
            return View(modelo);
        }
        [HttpGet]
        public ActionResult Excel()
        {
           
            return View();
        }
        [HttpGet]
        public async Task<ActionResult> ExcelPorMes(int month, int year)
        {
            int Month;
            int Year;
            if (month == 0 || year == 0)
            {
                var hoy = DateTime.Today;
                Month = hoy.Month;
                Year = hoy.Year;
            }
            else
            {
                Month = month;
                Year = year;
            }
            var FechaInicio = new DateTime(Year, Month, 1);
            var FechaFin = FechaInicio.AddMonths(1).AddDays(-1);
            var UsuarioId = await getUserInfo.GetId();
            var transacciones = await repositorioTransacciones.ObtenerListado(UsuarioId, FechaInicio, FechaFin);
            var nombreArchivo = $"JC_ManejoDePresupuestos - {FechaInicio.ToString("MMM yyyy").ToUpper().Replace(".","-")}.xlsx";
            return GenerarExcel(nombreArchivo, transacciones);

        }
        [HttpGet]
        public async Task<ActionResult> ExcelAnual(int year)
        {
            int Year;
            if (year == 0)
            {
                var hoy = DateTime.Today;
                Year = hoy.Year;
            }
            else
            {
                Year = year;
            }
            var FechaInicio = new DateTime(Year, 1, 1);
            var FechaFin = FechaInicio.AddYears(1).AddDays(-1);
            var UsuarioId = await getUserInfo.GetId();
            var transacciones = await repositorioTransacciones.ObtenerListado(UsuarioId, FechaInicio, FechaFin);
            var nombreArchivo = $"JC_ManejoDePresupuestos - {FechaInicio.ToString("yyyy")}.xlsx";
            return GenerarExcel(nombreArchivo, transacciones);

        }
        [HttpGet]
        public async Task<ActionResult> ExportarExcelTodo()
        {

            var FechaInicio = DateTime.Today.AddYears(-100);
            var FechaFin = DateTime.Today.AddYears(1000);
            var UsuarioId = await getUserInfo.GetId();
            var transacciones = await repositorioTransacciones.ObtenerListado(UsuarioId, FechaInicio, FechaFin);
            var nombreArchivo = $"JC_ManejoDePresupuestos - {DateTime.Today.ToString("dd MMM yyyy").ToUpper().Replace(".","-")}.xlsx";
            return GenerarExcel(nombreArchivo, transacciones);

        }
        private FileResult GenerarExcel(string nombreArchivo, IEnumerable<TransaccionCreacionViewModel> transacciones)
        {
            DataTable dataTable = new DataTable("Transacciones");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("Fecha"),
                new DataColumn("Monto"),
                new DataColumn("Nota"),
                new DataColumn("Cuenta"),
                new DataColumn("Categoria"),
                new DataColumn("Ingreso / Gasto"),
            });
            foreach (var transaccion in transacciones)
            {
                dataTable.Rows.Add(transaccion.FechaTransaccion, transaccion.Monto, transaccion.Nota, transaccion.Cuenta, transaccion.Categoria, transaccion.TipoOperacionId);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.SaveAs(memoryStream);
                    return File(memoryStream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        nombreArchivo);
                }
            }
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
            modelo.Categorias = await ObtenerCategorias(UsuarioId, modelo.TipoOperacionId);
            if (!modelo.Cuentas.Any())
            {
                ViewBag.Cuentas = "No hay cuentas registradas";
            }
            return modelo;
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCuentas(string UsuarioId)
        {
            var Cuentas = await repositorioCuentas.ObtenerListado(UsuarioId);
            return Cuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(string UsuarioId, TipoOperacionViewModel TipoOperacionId)
        {
            var categorias = await repositorioCategorias.ObtenerListado(UsuarioId, TipoOperacionId);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
