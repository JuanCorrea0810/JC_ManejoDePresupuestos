using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class MostrarCuentaViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Balance { get; set; }
        public string Descripcion { get; set; }
        [Display(Name ="Tipo de Cuenta")]
        public string TipoCuenta { get; set; }
    }
}
