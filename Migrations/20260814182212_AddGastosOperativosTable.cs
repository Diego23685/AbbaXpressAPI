using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGastosOperativosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GastosOperativos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SucursalId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MontoUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TipoCambioAplicado = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    MetodoPago = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaGasto = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NumeroComprobante = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GastosOperativos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GastosOperativos_Sucursales_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GastosOperativos_Usuarios_UsuarioId",
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

            migrationBuilder.CreateIndex(
                name: "IX_GastosOperativos_SucursalId",
                table: "GastosOperativos",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_GastosOperativos_UsuarioId",
                table: "GastosOperativos",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GastosOperativos");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 17, 20, 402, DateTimeKind.Utc).AddTicks(3588));

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 17, 20, 402, DateTimeKind.Utc).AddTicks(3601));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 17, 20, 153, DateTimeKind.Utc).AddTicks(2940));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 17, 20, 153, DateTimeKind.Utc).AddTicks(2947));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 17, 20, 153, DateTimeKind.Utc).AddTicks(2948));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 17, 20, 153, DateTimeKind.Utc).AddTicks(3087), "$2a$11$olZBaSGC4YOAuqHiP1ndw.oUHJrPFk3ZUSSeZHIoMYS1W8tIllkRi" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 17, 20, 274, DateTimeKind.Utc).AddTicks(8769), "$2a$11$r.lozKgfF0VeW8eXuEx5FOjAaTlZdl3QWJvRdR8sAbeuIiswwVu6K" });
        }
    }
}
