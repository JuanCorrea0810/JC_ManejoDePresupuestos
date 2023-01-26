using ManejoDePresupuestos.Utilidades.Countries;
using ManejoDePresupuestos.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Validaciones
{
    public class CountryValidationAttribute : ValidationAttribute
    {
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult("Se debe ingresar un país válido");
            }
            value = value.ToString().ToUpper();
            if (!Countries.ListCountries.Contains(value))
            {
                return new ValidationResult("El país no es válido", new string[] { nameof(value) });
            }
             return ValidationResult.Success;
        }
    }
}
