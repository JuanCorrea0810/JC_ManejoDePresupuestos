using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManejoDePresupuestos.Models;

public partial class Cuenta
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public int TipoCuentasId { get; set; }
    
    public decimal Balance { get; set; }

    public string Descripcion { get; set; }

    public string UsuarioId { get; set; }

    public virtual TipoCuenta TipoCuentas { get; set; }

    public virtual ICollection<Transaccion> Transacciones { get; } 

    public virtual NewIdentityUser Usuario { get; set; }
}
