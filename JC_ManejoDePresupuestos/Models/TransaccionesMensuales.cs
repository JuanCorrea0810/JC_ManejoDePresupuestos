using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManejoDePresupuestos.Models
{
    public class TransaccionesMensuales
    {
        private int mes;
        public int Mes  
        {
            get { return mes; }
            set { mes = value; }
        }
        public decimal Monto { get; set; }
        public TipoOperacionViewModel TipoOperacionId { get; set; }
        [NotMapped]
        public decimal IngresosMensuales { get; set; }
        [NotMapped]
        public decimal GastosMensuales { get; set; }
        [NotMapped]
        public decimal TotalMensual => IngresosMensuales - Math.Abs(GastosMensuales);
        [NotMapped]
        public DateTime Fecha => new DateTime(DateTime.Today.Year,mes,1);
    }
}
