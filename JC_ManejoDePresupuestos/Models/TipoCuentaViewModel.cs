using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class TipoCuentaViewModel
    {
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [Remote(action: "VerificarExisteNombreJS",controller:"TipoCuentas")]
        public string Nombre { get; set; }
    }
}
