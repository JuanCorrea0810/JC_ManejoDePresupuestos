using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class LoginAuth
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [EmailAddress(ErrorMessage = "Se debe ingresar una dirección de correo válida")]
        public string Email { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
        [Display(Name = "Mantener sesión iniciada")]
        public bool RememberMe { get; set; }
    }
}
