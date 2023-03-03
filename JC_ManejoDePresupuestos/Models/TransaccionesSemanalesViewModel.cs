using System.ComponentModel.DataAnnotations.Schema;

namespace ManejoDePresupuestos.Models
{
    [NotMapped]
    public class TransaccionesSemanalesViewModel 
    {
        public int Semana { get; set; }
        public decimal Monto { get; set; }
        public TipoOperacionViewModel TipoOperacionId { get; set; }
        [NotMapped]
        public decimal Ingresos { get; set; }
        [NotMapped]
        public decimal Gastos { get; set; }
        [NotMapped]
        public DateTime FechaInicio { get; set; }
        [NotMapped]
        public DateTime FechaFin { get; set; }


    }
}
