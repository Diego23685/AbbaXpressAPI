using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClientesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CodigoPais = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TarifaAereo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TarifaMaritimo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Direccion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoCliente = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rol = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SucursalAsignada = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Activo", "CodigoPais", "Direccion", "Email", "FechaRegistro", "Nombre", "TarifaAereo", "TarifaMaritimo", "Telefono", "TipoCliente" },
                values: new object[,]
                {
                    { 1, true, "+505", "Managua, Reparto San Juan", "juan.perez@email.com", new DateTime(2026, 8, 14, 18, 10, 8, 23, DateTimeKind.Utc).AddTicks(8084), "Juan Carlos Pérez", 7.00m, 4.00m, "88997711", "CONSUMIDOR_FINAL" },
                    { 2, true, "+505", "León, Parque Central 1c al norte", "leon@tienditaabba.com", new DateTime(2026, 8, 14, 18, 10, 8, 23, DateTimeKind.Utc).AddTicks(8173), "Sucursal León (Mayorista B2B)", 5.50m, 3.00m, "87654321", "SUCURSAL_B2B" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "FechaCreacion", "Nombre", "PasswordHash", "Rol", "SucursalAsignada", "Username" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2026, 8, 14, 18, 10, 7, 776, DateTimeKind.Utc).AddTicks(4115), "Junior Ramírez (Super Admin)", "$2a$11$Wwkn9Hx7/B.9W2LpntXedu2/GmFyVIda8176/qtBAhL5AuuRVfg2m", "SUPER_ADMIN", "Sucursal Bolonia - Central", "admin" },
                    { 2, true, new DateTime(2026, 8, 14, 18, 10, 7, 899, DateTimeKind.Utc).AddTicks(2565), "Admin Sucursal León", "$2a$11$sIATnddbI5pjZbxlNl7Ufe/LfJxLfrjc6rgeNSfYy0BNMmamYC9RG", "ADMIN_SUCURSAL_INDEPENDIENTE", "Sucursal León", "adminleon" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
