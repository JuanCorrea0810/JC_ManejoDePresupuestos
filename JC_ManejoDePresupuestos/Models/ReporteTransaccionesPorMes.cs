namespace ManejoDePresupuestos.Models
{
    public class ReporteTransaccionesPorMes
    {
        public IEnumerable<TransaccionesMensuales> TransaccionesAgrupadas { get; set; }
        public decimal IngresosAnuales => TransaccionesAgrupadas.Sum(x => x.IngresosMensuales);
        public decimal GastosAnuales => TransaccionesAgrupadas.Sum(x => x.GastosMensuales);
        public decimal TotalAnual => IngresosAnuales - Math.Abs(GastosAnuales);

    }
}
