namespace ManejoDePresupuestos.Models
{
    public class ReporteSemanaViewModel
    {
        public decimal Ingresos => TransaccionesPorSemana.Sum(x => x.Ingresos);
        public decimal Gastos => TransaccionesPorSemana.Sum(x => x.Gastos);
        public decimal Total => Ingresos - Math.Abs(Gastos);
        public DateTime Fecha { get; set; }
        public IEnumerable<TransaccionesSemanalesViewModel> TransaccionesPorSemana { get; set; }


    }
}
