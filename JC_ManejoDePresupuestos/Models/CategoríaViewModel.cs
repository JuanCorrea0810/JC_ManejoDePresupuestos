using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class CategoríaViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [StringLength(maximumLength:50,ErrorMessage ="El campo {0} no puede tener más de {1} caracteres")]

        public string Nombre { get; set; }
        [Display(Name = "Tipo Operación")]
        public TipoOperacionViewModel TipoOperacionId { get; set; }
    }
}
