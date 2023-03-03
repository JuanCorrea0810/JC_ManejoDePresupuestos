using System;
using System.Collections.Generic;
using System.Data;
using ManejoDePresupuestos.Utilidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ManejoDePresupuestos.Models;

public partial class ApplicationDbContext : IdentityDbContext<NewIdentityUser>
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<Cuenta> Cuentas { get; set; }

    public virtual DbSet<TipoCuenta> TipoCuentas { get; set; }

    public virtual DbSet<TipoOperacion> TiposOperaciones { get; set; }

    public virtual DbSet<Transaccion> Transacciones { get; set; }
    public virtual DbSet<TransaccionesSemanalesViewModel> TransaccionesPorSemana { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Transaccion>()
            .Property(p => p.Monto)
            .HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Cuenta>()
            .Property(p => p.Balance)
            .HasColumnType("decimal(18,2)");
        
        modelBuilder.Entity<TransaccionesSemanalesViewModel>().HasNoKey();
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
    }


}
