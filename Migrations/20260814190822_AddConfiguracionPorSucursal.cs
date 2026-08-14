using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracionPorSucursal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SucursalId",
                table: "Configuraciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 19, 8, 21, 472, DateTimeKind.Utc).AddTicks(4107));

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 19, 8, 21, 472, DateTimeKind.Utc).AddTicks(4120));

            migrationBuilder.UpdateData(
                table: "Configuraciones",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "SucursalId", "UltimaModificacion" },
                values: new object[] { 1, new DateTime(2026, 8, 14, 19, 8, 21, 224, DateTimeKind.Utc).AddTicks(8467) });

            migrationBuilder.InsertData(
                table: "Configuraciones",
                columns: new[] { "Id", "CostoProveedorAereo", "CostoProveedorMaritimo", "SucursalId", "TarifaAereoGeneral", "TarifaCelularFija", "TarifaMaritimoGeneral", "TarifaTvAereo", "TarifaTvMaritimo", "TipoCambioNIO", "UltimaModificacion" },
                values: new object[] { 2, 5.50m, 3.00m, 3, 7.50m, 35.00m, 4.50m, 8.00m, 4.00m, 36.6243m, new DateTime(2026, 8, 14, 19, 8, 21, 224, DateTimeKind.Utc).AddTicks(8484) });

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 19, 8, 21, 224, DateTimeKind.Utc).AddTicks(8295));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 19, 8, 21, 224, DateTimeKind.Utc).AddTicks(8301));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 19, 8, 21, 224, DateTimeKind.Utc).AddTicks(8302));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 19, 8, 21, 224, DateTimeKind.Utc).AddTicks(8516), "$2a$11$96Urda4nsggm4slZZw4yW.OrF05fny/Xsqxfw0Ceo6n7oTcUtM6ku" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 19, 8, 21, 345, DateTimeKind.Utc).AddTicks(1354), "$2a$11$XKj7wXdGRHY0efq0u452Te1jgslxAAoG9WJCSvcgeJqm2heErXt6W" });

            migrationBuilder.CreateIndex(
                name: "IX_Configuraciones_SucursalId",
                table: "Configuraciones",
                column: "SucursalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Configuraciones_Sucursales_SucursalId",
                table: "Configuraciones",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Configuraciones_Sucursales_SucursalId",
                table: "Configuraciones");

            migrationBuilder.DropIndex(
                name: "IX_Configuraciones_SucursalId",
                table: "Configuraciones");

            migrationBuilder.DeleteData(
                table: "Configuraciones",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "SucursalId",
                table: "Configuraciones");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 42, 11, 324, DateTimeKind.Utc).AddTicks(4731));

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 42, 11, 324, DateTimeKind.Utc).AddTicks(4747));

            migrationBuilder.UpdateData(
                table: "Configuraciones",
                keyColumn: "Id",
                keyValue: 1,
                column: "UltimaModificacion",
                value: new DateTime(2026, 8, 14, 18, 42, 11, 81, DateTimeKind.Utc).AddTicks(5242));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 42, 11, 81, DateTimeKind.Utc).AddTicks(5080));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 42, 11, 81, DateTimeKind.Utc).AddTicks(5090));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 42, 11, 81, DateTimeKind.Utc).AddTicks(5091));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 42, 11, 81, DateTimeKind.Utc).AddTicks(5267), "$2a$11$6Pc7agGRB0FALCMpji6l0uu.tol2W0N7lXPPBZewaD/Uhx7jA/QFG" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 42, 11, 198, DateTimeKind.Utc).AddTicks(5454), "$2a$11$I0pBP1wq5Hdq0fTzoNx4V.y9Nbnwe9Sqo6YvCBE8yfoNu16PMiFYS" });
        }
    }
}
