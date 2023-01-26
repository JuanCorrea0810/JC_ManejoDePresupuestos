using System;
using System.Collections.Generic;

namespace ManejoDePresupuestos.Models;

public partial class TipoCuenta
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public int Orden { get; set; }

    public string UsuarioId { get; set; }

    public virtual ICollection<Cuenta> Cuenta { get; } 

    public virtual NewIdentityUser Usuario { get; set; }
}
