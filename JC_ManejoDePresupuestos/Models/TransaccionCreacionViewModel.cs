using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class TransaccionCreacionViewModel : TransaccionViewModel
    {
        public IEnumerable<SelectListItem> Cuentas { get; set; }
        public IEnumerable<SelectListItem> Categorias { get; set; }
        [Display(Name = "Tipo Operación")]
        public TipoOperacionViewModel TipoOperacionId { get; set; } = TipoOperacionViewModel.Ingreso;

    }
}
