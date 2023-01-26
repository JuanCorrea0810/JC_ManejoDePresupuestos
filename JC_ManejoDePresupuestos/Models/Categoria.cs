using System;
using System.Collections.Generic;

namespace ManejoDePresupuestos.Models;

public partial class Categoria
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public int TipoOperacionId { get; set; }

    public string UsuarioId { get; set; }

    public virtual TipoOperacion TipoOperacion { get; set; }

    public virtual ICollection<Transaccion> Transacciones { get; } 

    public virtual NewIdentityUser Usuario { get; set; }
}
