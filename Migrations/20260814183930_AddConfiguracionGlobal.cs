using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracionGlobal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuraciones",
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
                    table.PrimaryKey("PK_Configuraciones", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 39, 30, 190, DateTimeKind.Utc).AddTicks(6524));

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 39, 30, 190, DateTimeKind.Utc).AddTicks(6538));

            migrationBuilder.InsertData(
                table: "Configuraciones",
                columns: new[] { "Id", "CostoProveedorAereo", "CostoProveedorMaritimo", "TarifaAereoGeneral", "TarifaCelularFija", "TarifaMaritimoGeneral", "TarifaTvAereo", "TarifaTvMaritimo", "TipoCambioNIO", "UltimaModificacion" },
                values: new object[] { 1, 3.80m, 1.50m, 7.00m, 35.00m, 4.00m, 7.50m, 3.50m, 36.6243m, new DateTime(2026, 8, 14, 18, 39, 29, 943, DateTimeKind.Utc).AddTicks(4722) });

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 39, 29, 943, DateTimeKind.Utc).AddTicks(4559));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 39, 29, 943, DateTimeKind.Utc).AddTicks(4565));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 39, 29, 943, DateTimeKind.Utc).AddTicks(4566));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 39, 29, 943, DateTimeKind.Utc).AddTicks(4748), "$2a$11$IVmjCghtKJWYpNb3gLW7eOPelVtxq5OUcobRJzLKHJk.S5u5QK3MS" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 39, 30, 63, DateTimeKind.Utc).AddTicks(2299), "$2a$11$ANCBIeVl/uEwhkAOIns/1uh3bIq4sKvl4j3diAR4qfCEWGyiJ3Qaa" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuraciones");

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
        }
    }
}
