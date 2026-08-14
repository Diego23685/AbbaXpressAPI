using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Configuraciones",
                keyColumn: "Id",
                keyValue: 1,
                column: "UltimaModificacion",
                value: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Configuraciones",
                keyColumn: "Id",
                keyValue: 2,
                column: "UltimaModificacion",
                value: new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 19, 19, 48, 798, DateTimeKind.Utc).AddTicks(4758));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 19, 19, 48, 798, DateTimeKind.Utc).AddTicks(4770));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 19, 19, 48, 798, DateTimeKind.Utc).AddTicks(4772));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$TSxs0i3PeCTmctKmT5Wdk.JKNnV0UOQtFww2od/Z.Xc1VNW0b.2y6" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "$2a$11$3.OP7DXyqIb0MhfA9A593.uXP8GQ5A3EDOoQXigvtwqZhvMikJy5." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                column: "UltimaModificacion",
                value: new DateTime(2026, 8, 14, 19, 8, 21, 224, DateTimeKind.Utc).AddTicks(8467));

            migrationBuilder.UpdateData(
                table: "Configuraciones",
                keyColumn: "Id",
                keyValue: 2,
                column: "UltimaModificacion",
                value: new DateTime(2026, 8, 14, 19, 8, 21, 224, DateTimeKind.Utc).AddTicks(8484));

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
        }
    }
}
