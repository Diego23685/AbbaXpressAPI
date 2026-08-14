using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddExportacionFedExModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EnviosExportacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CodigoEnvio = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TrackingFedEx = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SucursalOrigenId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    RemitenteNombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemitenteTelefono = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemitenteDireccion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinatarioNombre = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinatarioTelefono = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinatarioEstado = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinatarioCiudad = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinatarioZipCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DestinatarioDireccion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PesoTotalLbs = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TarifaBaseUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RecargoEstadoUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalCobradoUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TipoCambioAplicado = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    EstadoOperativo = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnviosExportacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnviosExportacion_Sucursales_SucursalOrigenId",
                        column: x => x.SucursalOrigenId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EnviosExportacion_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemsExportacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EnvioExportacionId = table.Column<int>(type: "int", nullable: false),
                    DescripcionES = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DescripcionEN = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Cantidad = table.Column<int>(type: "int", nullable: false),
                    PesoLbs = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ValorDeclaradoUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsExportacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsExportacion_EnviosExportacion_EnvioExportacionId",
                        column: x => x.EnvioExportacionId,
                        principalTable: "EnviosExportacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 35, 59, 551, DateTimeKind.Utc).AddTicks(822));

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 35, 59, 551, DateTimeKind.Utc).AddTicks(828));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 35, 59, 312, DateTimeKind.Utc).AddTicks(460));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 35, 59, 312, DateTimeKind.Utc).AddTicks(465));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 35, 59, 312, DateTimeKind.Utc).AddTicks(466));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 35, 59, 312, DateTimeKind.Utc).AddTicks(621), "$2a$11$NZbqYdkaGBT4jgxZvm12IOALaDvD8igdrSH2sDtH9eKFvJyOtqnKq" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 35, 59, 430, DateTimeKind.Utc).AddTicks(7367), "$2a$11$U7iQeeMa/MGLXxujnBdEOueY9L.ZsXVSOaAy4FfYxlLHwiAlT9TKq" });

            migrationBuilder.CreateIndex(
                name: "IX_EnviosExportacion_SucursalOrigenId",
                table: "EnviosExportacion",
                column: "SucursalOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_EnviosExportacion_UsuarioId",
                table: "EnviosExportacion",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsExportacion_EnvioExportacionId",
                table: "ItemsExportacion",
                column: "EnvioExportacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsExportacion");

            migrationBuilder.DropTable(
                name: "EnviosExportacion");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 22, 11, 902, DateTimeKind.Utc).AddTicks(5041));

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 22, 11, 902, DateTimeKind.Utc).AddTicks(5057));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 22, 11, 636, DateTimeKind.Utc).AddTicks(1252));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 22, 11, 636, DateTimeKind.Utc).AddTicks(1263));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 22, 11, 636, DateTimeKind.Utc).AddTicks(1265));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 22, 11, 636, DateTimeKind.Utc).AddTicks(1482), "$2a$11$LPfmPl8A41qqziGcLgBK2ObzMwC8CJUf931ucD4cZfP3ap8nAEHhS" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 22, 11, 768, DateTimeKind.Utc).AddTicks(644), "$2a$11$96grnGCLB5Ob3BEQfRyEWOmwQV1j5zCbj6VRWdXIiMGp9b2wXYfGa" });
        }
    }
}
