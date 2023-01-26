using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Utilidades.Administracion_DTOs
{
    public class ForgetPassword
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [EmailAddress(ErrorMessage ="El Email no es válido")]
        public string Email { get; set; }
    }
}
