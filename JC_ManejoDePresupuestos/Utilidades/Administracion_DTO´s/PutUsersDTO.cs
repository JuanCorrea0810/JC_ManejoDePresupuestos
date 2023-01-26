
using ManejoDePresupuestos.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Utilidades.Administracion_DTOs
{
    public class PutUsersDTO : IValidatableObject
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [Display(Name = "Documento de Identidad")]
        public string Dni { get; set; }
        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [Display(Name ="Primer Nombre")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string FirstName { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [Display(Name ="Segundo Nombre")]
        public string SecondName { get; set; }

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [Display(Name ="Primer Apellido")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string FirstSurName { get; set; } 

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [Display(Name ="Segundo Apellido")]
        public string SecondSurName { get; set; } 


        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [CountryValidation]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name ="País")]
        public string Country { get; set; } 

        [StringLength(250, ErrorMessage = "El campo {0}, no puede tener más de {1} caracteres")]
        [Display(Name = "Dirección")]

        public string Address { get; set; }
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Range(1, 130, ErrorMessage = "Se debe ingresar un valor entre {1} y {2}")]
        [Display(Name = "Edad")]
        public int Age { get; set; }
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Número de celular no válido")]
        [Display(Name = "Número de Celular")]
        public  string PhoneNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SecondName is not null)
            {
                SecondName = SecondName.ToUpper();
            }
            if (SecondSurName is not null)
            {
                SecondSurName = SecondSurName.ToUpper();
            }
            FirstName = FirstName.ToUpper();
            FirstSurName = FirstSurName.ToUpper();           
            Country = Country.ToUpper();
            yield return ValidationResult.Success;
        }
    }
}
