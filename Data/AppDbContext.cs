using Microsoft.EntityFrameworkCore;
using AbbaXpress.API.Models;

namespace AbbaXpress.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Sucursal> Sucursales => Set<Sucursal>();
        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Proveedor> Proveedores => Set<Proveedor>();
        public DbSet<Proforma> Proformas => Set<Proforma>();
        public DbSet<Paquete> Paquetes => Set<Paquete>();
        public DbSet<SesionCaja> SesionesCaja => Set<SesionCaja>();
        public DbSet<DenominacionCaja> DenominacionesCaja => Set<DenominacionCaja>();
        public DbSet<MovimientoCaja> MovimientosCaja => Set<MovimientoCaja>();
        public DbSet<NotificacionSucursal> NotificacionesSucursales => Set<NotificacionSucursal>();
        public DbSet<GastoOperativo> GastosOperativos => Set<GastoOperativo>();
        public DbSet<EnvioExportacion> EnviosExportacion => Set<EnvioExportacion>();
        public DbSet<ItemExportacion> ItemsExportacion => Set<ItemExportacion>();
        public DbSet<ConfiguracionSucursal> Configuraciones => Set<ConfiguracionSucursal>();
        public DbSet<ConfiguracionGlobal> ConfiguracionesGlobales => Set<ConfiguracionGlobal>();
        public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // REGLAS FLUENT API: EVITAR CICLOS CASCADE
            // ==========================================

            // Proforma -> SucursalOrigen / SucursalDestino
            modelBuilder.Entity<Proforma>()
                .HasOne(p => p.SucursalOrigen)
                .WithMany()
                .HasForeignKey(p => p.SucursalOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proforma>()
                .HasOne(p => p.SucursalDestino)
                .WithMany()
                .HasForeignKey(p => p.SucursalDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Proforma>()
                .HasOne(p => p.GastoGeneradoLeon)
                .WithMany()
                .HasForeignKey(p => p.GastoGeneradoLeonId)
                .OnDelete(DeleteBehavior.SetNull);

            // SesionCaja -> Usuario Apertura y Cierre
            modelBuilder.Entity<SesionCaja>()
                .HasOne(s => s.UsuarioApertura)
                .WithMany()
                .HasForeignKey(s => s.UsuarioAperturaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SesionCaja>()
                .HasOne(s => s.UsuarioCierre)
                .WithMany()
                .HasForeignKey(s => s.UsuarioCierreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Paquete -> Proveedor
            modelBuilder.Entity<Paquete>()
                .HasOne(p => p.Proveedor)
                .WithMany()
                .HasForeignKey(p => p.ProveedorId)
                .OnDelete(DeleteBehavior.SetNull);

            // Índices para optimización de reportes y búsqueda rápida
            modelBuilder.Entity<Paquete>()
                .HasIndex(p => p.Tracking);

            modelBuilder.Entity<Proforma>()
                .HasIndex(p => p.NumeroProforma);

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Telefono);

            // ==========================================
            // SEED DATA
            // ==========================================

            // 1. Sucursales
            modelBuilder.Entity<Sucursal>().HasData(
                new Sucursal
                {
                    Id = 1,
                    Nombre = "Sucursal Bolonia - Central",
                    Ciudad = "Managua",
                    Direccion = "Rotonda El Güegüense, 4c abajo, 1c al norte y media c arriba. Plaza Kamelu, módulo 2",
                    Telefono = "+505 2222-1111",
                    TipoSucursal = "PROPIA",
                    Activa = true,
                    FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Sucursal
                {
                    Id = 2,
                    Nombre = "Sucursal Ciudad Doral",
                    Ciudad = "Managua",
                    Direccion = "Avenida 15, calle 4, módulo P-14",
                    Telefono = "+505 2222-2222",
                    TipoSucursal = "PROPIA",
                    Activa = true,
                    FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Sucursal
                {
                    Id = 3,
                    Nombre = "Sucursal León",
                    Ciudad = "León",
                    Direccion = "De la estatua de la madre 1c y media al norte",
                    Telefono = "+505 8765-4321",
                    TipoSucursal = "FRANQUICIA_B2B",
                    Activa = true,
                    FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 2. Proveedores Base
            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor
                {
                    Id = 1,
                    Nombre = "AereoMar Express",
                    Contacto = "Soporte Carga",
                    Telefono = "+1 305-555-0199",
                    CostoLibraAereo = 3.80m,
                    CostoLibraMaritimo = 1.50m,
                    Activo = true,
                    FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 3. Configuraciones por Sucursal
            modelBuilder.Entity<ConfiguracionSucursal>().HasData(
                new ConfiguracionSucursal
                {
                    Id = 1,
                    SucursalId = 1,
                    TipoCambioNIO = 36.6243m,
                    TarifaAereoGeneral = 7.00m,
                    TarifaMaritimoGeneral = 4.00m,
                    TarifaCelularFija = 35.00m,
                    TarifaTvMaritimo = 3.50m,
                    TarifaTvAereo = 7.50m,
                    CostoProveedorAereo = 3.80m,
                    CostoProveedorMaritimo = 1.50m,
                    UltimaModificacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new ConfiguracionSucursal
                {
                    Id = 2,
                    SucursalId = 3, // Sucursal León
                    TipoCambioNIO = 36.6243m,
                    TarifaAereoGeneral = 7.50m,
                    TarifaMaritimoGeneral = 4.50m,
                    TarifaCelularFija = 35.00m,
                    TarifaTvMaritimo = 4.00m,
                    TarifaTvAereo = 8.00m,
                    CostoProveedorAereo = 5.50m,
                    CostoProveedorMaritimo = 3.00m,
                    UltimaModificacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 4. Usuarios
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Junior Ramírez (Super Admin)",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
                    Rol = "SUPER_ADMIN",
                    SucursalId = 1,
                    Activo = true,
                    FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Usuario
                {
                    Id = 2,
                    Nombre = "Admin Sucursal León",
                    Username = "adminleon",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Leon123*"),
                    Rol = "ADMIN_SUCURSAL_INDEPENDIENTE",
                    SucursalId = 3,
                    Activo = true,
                    FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 5. Clientes
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente
                {
                    Id = 1,
                    SucursalId = 1,
                    Nombre = "Juan Carlos Pérez",
                    CodigoPais = "+505",
                    Telefono = "88997711",
                    Email = "juan.perez@email.com",
                    TarifaAereo = 7.00m,
                    TarifaMaritimo = 4.00m,
                    TarifaCelularFija = 35.00m,
                    TarifaTvAereo = 7.50m,
                    TarifaTvMaritimo = 3.50m,
                    Direccion = "Managua, Reparto San Juan",
                    TipoCliente = "CLIENTE_GENERAL",
                    Activo = true,
                    FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Cliente
                {
                    Id = 2,
                    SucursalId = 1,
                    Nombre = "Sucursal León (Subagencia)",
                    CodigoPais = "+505",
                    Telefono = "87654321",
                    Email = "leon@abbaexpress.com",
                    TarifaAereo = 5.50m,
                    TarifaMaritimo = 3.00m,
                    TarifaCelularFija = 30.00m,
                    TarifaTvAereo = 6.50m,
                    TarifaTvMaritimo = 3.00m,
                    Direccion = "De la estatua de la madre 1c y media al norte",
                    TipoCliente = "SUBAGENCIA",
                    Activo = true,
                    FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}