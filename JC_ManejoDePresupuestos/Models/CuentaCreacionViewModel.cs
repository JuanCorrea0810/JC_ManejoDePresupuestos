using Microsoft.AspNetCore.Mvc.Rendering;

namespace ManejoDePresupuestos.Models
{
    public class CuentaCreacionViewModel: CuentaViewModel
    {
        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
