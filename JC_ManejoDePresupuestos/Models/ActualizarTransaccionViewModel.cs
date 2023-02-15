using System.Reflection.Metadata.Ecma335;

namespace ManejoDePresupuestos.Models
{
    public class ActualizarTransaccionViewModel : TransaccionCreacionViewModel
    {
        public int CuentaAnteriorId { get; set; }
        public decimal MontoAnterior { get; set; }
    }
}
