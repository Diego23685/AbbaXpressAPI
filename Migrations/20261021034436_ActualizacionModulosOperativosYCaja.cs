using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class ActualizacionModulosOperativosYCaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "EsCargaInterSucursal",
                table: "Proformas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GastoGeneradoLeonId",
                table: "Proformas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoComisionPOS",
                table: "Proformas",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoRecibidoEfectivo",
                table: "Proformas",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoVueltoEntregado",
                table: "Proformas",
                type: "decimal(10,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PorcentajeComisionPOS",
                table: "Proformas",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SesionCajaId",
                table: "Proformas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProveedorId",
                table: "Paquetes",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Clientes",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "TarifaCelularFija",
                table: "Clientes",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TarifaTvAereo",
                table: "Clientes",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TarifaTvMaritimo",
                table: "Clientes",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConfiguracionesGlobales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TipoCambioNIO = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    TarifaAereoGeneral = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TarifaMaritimoGeneral = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TarifaCelularFija = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TarifaTvMaritimo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TarifaTvAereo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CostoProveedorAereo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CostoProveedorMaritimo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UltimaModificacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesGlobales", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificacionesSucursales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SucursalDestinoId = table.Column<int>(type: "int", nullable: false),
                    ProformaId = table.Column<int>(type: "int", nullable: true),
                    Mensaje = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Leida = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificacionesSucursales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificacionesSucursales_Proformas_ProformaId",
                        column: x => x.ProformaId,
                        principalTable: "Proformas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NotificacionesSucursales_Sucursales_SucursalDestinoId",
                        column: x => x.SucursalDestinoId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contacto = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CostoLibraAereo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CostoLibraMaritimo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Activo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SesionesCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SucursalId = table.Column<int>(type: "int", nullable: false),
                    UsuarioAperturaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCierreId = table.Column<int>(type: "int", nullable: true),
                    FechaApertura = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    BultosFisicosApertura = table.Column<int>(type: "int", nullable: false),
                    BultosFisicosCierre = table.Column<int>(type: "int", nullable: true),
                    MontoAperturaCordobas = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    MontoAperturaDolares = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    MontoCierreCordobas = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    MontoCierreDolares = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Estado = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SesionesCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SesionesCaja_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SesionesCaja_Usuarios_UsuarioAperturaId",
                        column: x => x.UsuarioAperturaId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SesionesCaja_Usuarios_UsuarioCierreId",
                        column: x => x.UsuarioCierreId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DenominacionesCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SesionCajaId = table.Column<int>(type: "int", nullable: false),
                    Momento = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Moneda = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Denominacion = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenominacionesCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DenominacionesCaja_SesionesCaja_SesionCajaId",
                        column: x => x.SesionCajaId,
                        principalTable: "SesionesCaja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MovimientosCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SesionCajaId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Concepto = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Beneficiario = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Moneda = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientosCaja_SesionesCaja_SesionCajaId",
                        column: x => x.SesionCajaId,
                        principalTable: "SesionesCaja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovimientosCaja_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "TarifaCelularFija", "TarifaTvAereo", "TarifaTvMaritimo", "TipoCliente" },
                values: new object[] { 35.00m, 7.50m, 3.50m, "CLIENTE_GENERAL" });

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Direccion", "Email", "Nombre", "TarifaCelularFija", "TarifaTvAereo", "TarifaTvMaritimo", "TipoCliente" },
                values: new object[] { "De la estatua de la madre 1c y media al norte", "leon@abbaexpress.com", "Sucursal León (Subagencia)", 30.00m, 6.50m, 3.00m, "SUBAGENCIA" });

            migrationBuilder.InsertData(
                table: "Proveedores",
                columns: new[] { "Id", "Activo", "Contacto", "CostoLibraAereo", "CostoLibraMaritimo", "FechaRegistro", "Nombre", "Telefono" },
                values: new object[] { 1, true, "Soporte Carga", 3.80m, 1.50m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AereoMar Express", "+1 305-555-0199" });

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Direccion", "FechaRegistro" },
                values: new object[] { "Rotonda El Güegüense, 4c abajo, 1c al norte y media c arriba. Plaza Kamelu, módulo 2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Direccion", "FechaRegistro", "Nombre" },
                values: new object[] { "Avenida 15, calle 4, módulo P-14", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sucursal Ciudad Doral" });

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Direccion", "FechaRegistro" },
                values: new object[] { "De la estatua de la madre 1c y media al norte", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$eBh3HybZL/LWBWLrdahpXOxVAscjIFjBrDRrzFStCmDsWLATH2rQy");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$6.0Z.Rm.bgB/Pi7GdTBA.uF84BD3SH5W8HtoHdTz3HYRXiH2dYduK");

            migrationBuilder.CreateIndex(
                name: "IX_Proformas_GastoGeneradoLeonId",
                table: "Proformas",
                column: "GastoGeneradoLeonId");

            migrationBuilder.CreateIndex(
                name: "IX_Proformas_NumeroProforma",
                table: "Proformas",
                column: "NumeroProforma");

            migrationBuilder.CreateIndex(
                name: "IX_Proformas_SesionCajaId",
                table: "Proformas",
                column: "SesionCajaId");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_ProveedorId",
                table: "Paquetes",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_Tracking",
                table: "Paquetes",
                column: "Tracking");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Telefono",
                table: "Clientes",
                column: "Telefono");

            migrationBuilder.CreateIndex(
                name: "IX_DenominacionesCaja_SesionCajaId",
                table: "DenominacionesCaja",
                column: "SesionCajaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCaja_SesionCajaId",
                table: "MovimientosCaja",
                column: "SesionCajaId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosCaja_UsuarioId",
                table: "MovimientosCaja",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionesSucursales_ProformaId",
                table: "NotificacionesSucursales",
                column: "ProformaId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificacionesSucursales_SucursalDestinoId",
                table: "NotificacionesSucursales",
                column: "SucursalDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_SesionesCaja_SucursalId",
                table: "SesionesCaja",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_SesionesCaja_UsuarioAperturaId",
                table: "SesionesCaja",
                column: "UsuarioAperturaId");

            migrationBuilder.CreateIndex(
                name: "IX_SesionesCaja_UsuarioCierreId",
                table: "SesionesCaja",
                column: "UsuarioCierreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Paquetes_Proveedores_ProveedorId",
                table: "Paquetes",
                column: "ProveedorId",
                principalTable: "Proveedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Proformas_GastosOperativos_GastoGeneradoLeonId",
                table: "Proformas",
                column: "GastoGeneradoLeonId",
                principalTable: "GastosOperativos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Proformas_SesionesCaja_SesionCajaId",
                table: "Proformas",
                column: "SesionCajaId",
                principalTable: "SesionesCaja",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proformas_Sucursales_SucursalDestinoId",
                table: "Proformas",
                column: "SucursalDestinoId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Proformas_Sucursales_SucursalOrigenId",
                table: "Proformas",
                column: "SucursalOrigenId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Paquetes_Proveedores_ProveedorId",
                table: "Paquetes");

            migrationBuilder.DropForeignKey(
                name: "FK_Proformas_GastosOperativos_GastoGeneradoLeonId",
                table: "Proformas");

            migrationBuilder.DropForeignKey(
                name: "FK_Proformas_SesionesCaja_SesionCajaId",
                table: "Proformas");

            migrationBuilder.DropForeignKey(
                name: "FK_Proformas_Sucursales_SucursalDestinoId",
                table: "Proformas");

            migrationBuilder.DropForeignKey(
                name: "FK_Proformas_Sucursales_SucursalOrigenId",
                table: "Proformas");

            migrationBuilder.DropTable(
                name: "ConfiguracionesGlobales");

            migrationBuilder.DropTable(
                name: "DenominacionesCaja");

            migrationBuilder.DropTable(
                name: "MovimientosCaja");

            migrationBuilder.DropTable(
                name: "NotificacionesSucursales");

            migrationBuilder.DropTable(
                name: "Proveedores");

            migrationBuilder.DropTable(
                name: "SesionesCaja");

            migrationBuilder.DropIndex(
                name: "IX_Proformas_GastoGeneradoLeonId",
                table: "Proformas");

            migrationBuilder.DropIndex(
                name: "IX_Proformas_NumeroProforma",
                table: "Proformas");

            migrationBuilder.DropIndex(
                name: "IX_Proformas_SesionCajaId",
                table: "Proformas");

            migrationBuilder.DropIndex(
                name: "IX_Paquetes_ProveedorId",
                table: "Paquetes");

            migrationBuilder.DropIndex(
                name: "IX_Paquetes_Tracking",
                table: "Paquetes");

            migrationBuilder.DropIndex(
                name: "IX_Clientes_Telefono",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "EsCargaInterSucursal",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "GastoGeneradoLeonId",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "MontoComisionPOS",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "MontoRecibidoEfectivo",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "MontoVueltoEntregado",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "PorcentajeComisionPOS",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "SesionCajaId",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "ProveedorId",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "TarifaCelularFija",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TarifaTvAereo",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "TarifaTvMaritimo",
                table: "Clientes");

            migrationBuilder.AlterColumn<string>(
                name: "Telefono",
                table: "Clientes",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "TipoCliente",
                value: "CONSUMIDOR_FINAL");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Direccion", "Email", "Nombre", "TipoCliente" },
                values: new object[] { "León, Parque Central 1c al norte", "leon@tienditaabba.com", "Sucursal León (Mayorista B2B)", "SUCURSAL_B2B" });

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Direccion", "FechaRegistro" },
                values: new object[] { "Bolonia, de Plaza España 1c al oeste", new DateTime(2026, 8, 14, 22, 10, 40, 403, DateTimeKind.Utc).AddTicks(7136) });

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Direccion", "FechaRegistro", "Nombre" },
                values: new object[] { "Carretera Norte, Edificio Doral", new DateTime(2026, 8, 14, 22, 10, 40, 403, DateTimeKind.Utc).AddTicks(7143), "Sucursal Doral" });

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Direccion", "FechaRegistro" },
                values: new object[] { "Parque Central 1c al norte", new DateTime(2026, 8, 14, 22, 10, 40, 403, DateTimeKind.Utc).AddTicks(7144) });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$kFLS5kIjL/7.6CXaDYgBKuAMFQXctB1rkxkCXPGlDePE6XnlhGawG");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$hvfGzTfXPawFALMIHRhIEOpnaOsxj4kBX2lWWUKG8Q/G9xnVkiB0q");

            migrationBuilder.AddForeignKey(
                name: "FK_Proformas_Sucursales_SucursalDestinoId",
                table: "Proformas",
                column: "SucursalDestinoId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Proformas_Sucursales_SucursalOrigenId",
                table: "Proformas",
                column: "SucursalOrigenId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
