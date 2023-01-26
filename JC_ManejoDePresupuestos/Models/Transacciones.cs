using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManejoDePresupuestos.Models;

public partial class Transaccion
{
    public int Id { get; set; }

    public DateTime FechaTransaccion { get; set; }
    
    public decimal Monto { get; set; }

    public string Nota { get; set; }

    public int TipoOperacionId { get; set; }

    public int CuentaId { get; set; }

    public int CategoriaId { get; set; }

    public string UsuarioId { get; set; }

    public virtual Categoria Categoria { get; set; }

    public virtual Cuenta Cuenta { get; set; }

    public virtual TipoOperacion TipoOperacion { get; set; }

    public virtual NewIdentityUser Usuario { get; set; }
}
