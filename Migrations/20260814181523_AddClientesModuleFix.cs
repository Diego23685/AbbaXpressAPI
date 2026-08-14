using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClientesModuleFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SucursalAsignada",
                table: "Usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Usuarios",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Usuarios",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Usuarios",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SucursalId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Sucursales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ciudad = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TipoSucursal = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Activa = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursales", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 15, 23, 432, DateTimeKind.Utc).AddTicks(7828));

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 15, 23, 432, DateTimeKind.Utc).AddTicks(7834));

            migrationBuilder.InsertData(
                table: "Sucursales",
                columns: new[] { "Id", "Activa", "Ciudad", "Direccion", "FechaRegistro", "Nombre", "Telefono", "TipoSucursal" },
                values: new object[,]
                {
                    { 1, true, "Managua", "Bolonia, de Plaza España 1c al oeste", new DateTime(2026, 8, 14, 18, 15, 23, 180, DateTimeKind.Utc).AddTicks(9414), "Sucursal Bolonia - Central", "+505 2222-1111", "PROPIA" },
                    { 2, true, "Managua", "Carretera Norte, Edificio Doral", new DateTime(2026, 8, 14, 18, 15, 23, 180, DateTimeKind.Utc).AddTicks(9418), "Sucursal Doral", "+505 2222-2222", "PROPIA" },
                    { 3, true, "León", "Parque Central 1c al norte", new DateTime(2026, 8, 14, 18, 15, 23, 180, DateTimeKind.Utc).AddTicks(9420), "Sucursal León", "+505 8765-4321", "FRANQUICIA_B2B" }
                });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash", "SucursalId" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 15, 23, 180, DateTimeKind.Utc).AddTicks(9552), "$2a$11$nZ6XJMNb1yehYG8Kdv180.Jdl6h3ixdw0GbvgpDyz1udJKM79JqdG", 1 });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash", "SucursalId" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 15, 23, 306, DateTimeKind.Utc).AddTicks(7764), "$2a$11$6tMysVXk5kjibdK5VKHXfec0T1UjkvOEB1lOVtjw.fmD2zEx2ESwO", 3 });

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_SucursalId",
                table: "Usuarios",
                column: "SucursalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Sucursales_SucursalId",
                table: "Usuarios",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Sucursales_SucursalId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Sucursales");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_SucursalId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "SucursalId",
                table: "Usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Rol",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(150)",
                oldMaxLength: 150)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SucursalAsignada",
                table: "Usuarios",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 10, 8, 23, DateTimeKind.Utc).AddTicks(8084));

            migrationBuilder.UpdateData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 10, 8, 23, DateTimeKind.Utc).AddTicks(8173));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash", "SucursalAsignada" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 10, 7, 776, DateTimeKind.Utc).AddTicks(4115), "$2a$11$Wwkn9Hx7/B.9W2LpntXedu2/GmFyVIda8176/qtBAhL5AuuRVfg2m", "Sucursal Bolonia - Central" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash", "SucursalAsignada" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 10, 7, 899, DateTimeKind.Utc).AddTicks(2565), "$2a$11$sIATnddbI5pjZbxlNl7Ufe/LfJxLfrjc6rgeNSfYy0BNMmamYC9RG", "Sucursal León" });
        }
    }
}
