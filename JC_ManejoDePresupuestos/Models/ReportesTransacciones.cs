namespace ManejoDePresupuestos.Models
{
    public class ReportesTransacciones
    {
        //Esta clase nos ayudará a mostrar al usuario un reporte de sus transacciones
        //Las transacciones que se tomarán en cuenta son las que estén dentro del rango de FechaInicio y FechaFin
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        //En este Enumerable nos llegará la información de lo que se gastó en cada día
        public IEnumerable<TransaccionesPorFecha> TransaccionesAgrupadas { get; set; }
        public decimal BalanceIngresos => TransaccionesAgrupadas.Sum(x=> x.BalanceDepositos);
        public decimal BalanceRetiros => TransaccionesAgrupadas.Sum(x => x.BalanceRetiros);
        public decimal Total => BalanceIngresos - Math.Abs(BalanceRetiros);

        //Estas son las transacciones agrupadas por fecha
        public class TransaccionesPorFecha 
        {
            //A qué fecha corresponde cada transacción Ej: 01/08/2022.
            public DateTime FechaTransaccion { get; set; }
            //Transacciones que se dieron en ese día específico
            public IEnumerable<TransaccionCreacionViewModel> Transacciones { get; set; }
            //Cuánto ingresó ese día en particular
            public decimal BalanceDepositos => Transacciones.Where(x=> x.TipoOperacionId == TipoOperacionViewModel.Ingreso).Sum(x=> x.Monto);
            //Cuánto gastó ese día en particular
            public decimal BalanceRetiros => Transacciones.Where(x => x.TipoOperacionId == TipoOperacionViewModel.Gasto).Sum(x => x.Monto);
            //Balance de ese día
            public decimal Total => BalanceDepositos - Math.Abs(BalanceRetiros);
        }
    }
}
