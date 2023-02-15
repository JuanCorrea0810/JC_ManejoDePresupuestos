using System;
using System.Collections.Generic;

namespace ManejoDePresupuestos.Models;

public partial class TipoOperacion
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public virtual ICollection<Categoria> Categoria { get; } 

}
