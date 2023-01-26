using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ManejoDePresupuestos.Models
{
    public class NewIdentityUser : IdentityUser
    {

        public string Dni { get; set; }


        public string FirstName { get; set; }


        public string? SecondName { get; set; }



        public string FirstSurName { get; set; }


        public string? SecondSurName { get; set; }



        public string Country { get; set; }



        public string Address { get; set; }

        public int Age { get; set; }

        public virtual ICollection<Categoria> Categoria { get; } 

        public virtual ICollection<Cuenta> Cuenta { get; } 

        public virtual ICollection<TipoCuenta> TipoCuenta { get; } 

        public virtual ICollection<Transaccion> Transacciones { get; } 
    }
}
