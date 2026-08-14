using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddConfiguracionGlobal2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "Configuraciones",
                keyColumn: "Id",
                keyValue: 1,
                column: "UltimaModificacion",
                value: new DateTime(2026, 8, 14, 18, 39, 29, 943, DateTimeKind.Utc).AddTicks(4722));

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
    }
}
