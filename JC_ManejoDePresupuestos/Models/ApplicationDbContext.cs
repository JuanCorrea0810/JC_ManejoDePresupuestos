﻿using System;
using System.Collections.Generic;
using System.Data;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Transaccion>()
            .Property(p => p.Monto)
            .HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Cuenta>()
            .Property(p => p.Balance)
            .HasColumnType("decimal(18,2)");
        base.OnModelCreating(modelBuilder);
    }


}