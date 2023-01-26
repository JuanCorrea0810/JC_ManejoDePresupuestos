using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Utilidades.Administracion_DTOs
{
    public class ChangePassword
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [Display(Name ="Contraseña Actual")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 50, MinimumLength = 8, ErrorMessage = "La Contraseña debe ser entre {2} y {1} caracteres")]
        [Display(Name = "Contraseña Nueva")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }
    }
}
