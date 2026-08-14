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
        public DbSet<Proforma> Proformas => Set<Proforma>();
        public DbSet<Paquete> Paquetes => Set<Paquete>();
        public DbSet<GastoOperativo> GastosOperativos => Set<GastoOperativo>();
        public DbSet<EnvioExportacion> EnviosExportacion => Set<EnvioExportacion>();
        public DbSet<ItemExportacion> ItemsExportacion => Set<ItemExportacion>();
        public DbSet<ConfiguracionSucursal> Configuraciones => Set<ConfiguracionSucursal>();
        public DbSet<LogAuditoria> LogsAuditoria => Set<LogAuditoria>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Seed Data Sucursales
            modelBuilder.Entity<Sucursal>().HasData(
                new Sucursal
                {
                    Id = 1,
                    Nombre = "Sucursal Bolonia - Central",
                    Ciudad = "Managua",
                    Direccion = "Bolonia, de Plaza España 1c al oeste",
                    Telefono = "+505 2222-1111",
                    TipoSucursal = "PROPIA",
                    Activa = true
                },
                new Sucursal
                {
                    Id = 2,
                    Nombre = "Sucursal Doral",
                    Ciudad = "Managua",
                    Direccion = "Carretera Norte, Edificio Doral",
                    Telefono = "+505 2222-2222",
                    TipoSucursal = "PROPIA",
                    Activa = true
                },
                new Sucursal
                {
                    Id = 3,
                    Nombre = "Sucursal León",
                    Ciudad = "León",
                    Direccion = "Parque Central 1c al norte",
                    Telefono = "+505 8765-4321",
                    TipoSucursal = "FRANQUICIA_B2B",
                    Activa = true
                }
            );

            // 2. Seed Data Configuraciones por Sucursal
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

            // 3. Seed Data Usuarios (Superadmin Managua e Independiente León)
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    Nombre = "Junior Ramírez (Super Admin)",
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123*"),
                    Rol = "SUPER_ADMIN",
                    SucursalId = 1, // Bolonia Central
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
                    SucursalId = 3, // Sucursal León
                    Activo = true,
                    FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 4. Seed Data Clientes
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente
                {
                    Id = 1,
                    SucursalId = 1, // Cliente de Managua (Bolonia)
                    Nombre = "Juan Carlos Pérez",
                    CodigoPais = "+505",
                    Telefono = "88997711",
                    Email = "juan.perez@email.com",
                    TarifaAereo = 7.00m,
                    TarifaMaritimo = 4.00m,
                    Direccion = "Managua, Reparto San Juan",
                    TipoCliente = "CONSUMIDOR_FINAL",
                    Activo = true,
                    FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Cliente
                {
                    Id = 2,
                    SucursalId = 1, // Cliente B2B registrado por Managua para cobrarle a León
                    Nombre = "Sucursal León (Mayorista B2B)",
                    CodigoPais = "+505",
                    Telefono = "87654321",
                    Email = "leon@tienditaabba.com",
                    TarifaAereo = 5.50m,
                    TarifaMaritimo = 3.00m,
                    Direccion = "León, Parque Central 1c al norte",
                    TipoCliente = "SUCURSAL_B2B",
                    Activo = true,
                    FechaRegistro = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}