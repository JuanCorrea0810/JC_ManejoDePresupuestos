using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class CuentaViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:50)]
        public string  Nombre { get; set; }
        [StringLength(maximumLength: 1000)]
        public string  Descripcion { get; set; }
        public decimal Balance { get; set; }
        [Display(Name ="Tipo de Cuenta")]
        [Required]
        public int TipoCuentasId { get; set; }
    }
}
