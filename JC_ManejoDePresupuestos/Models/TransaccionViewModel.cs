using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class TransaccionViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Fecha Transacción")]
        public DateTime FechaTransaccion { get; set; } = DateTime.Parse(DateTime.Now.ToString("g"));
        [Required(ErrorMessage = "Debe proporcionar un Monto")]
        public decimal Monto { get; set; }
        [StringLength(maximumLength:1000,ErrorMessage ="El campo {0} debe tener un máximo de {1} caracteres")]
        [Required(ErrorMessage ="Debe proporcionar una descripción breve de donde proviene esta transacción")]
        public string Nota { get; set; }
        [Required(ErrorMessage = "Debe proporcionar una Categoría")]
        [Range(1,maximum:int.MaxValue,ErrorMessage ="Debe seleccionar una Categoría")]
        [Display(Name ="Categoría")]
        public int CategoriaId { get; set; }
        [Required(ErrorMessage = "Debe proporcionar una Cuenta")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Debe seleccionar una Cuenta")]
        [Display(Name = "Cuenta")]
        public int CuentaId { get; set; }

        public string Cuenta { get; set; }
        public string Categoria { get; set; }

    }
}
