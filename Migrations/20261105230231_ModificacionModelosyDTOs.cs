using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionModelosyDTOs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPiesCubicos",
                table: "Proformas",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AltoIn",
                table: "Paquetes",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AnchoIn",
                table: "Paquetes",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Paquetes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "LargoIn",
                table: "Paquetes",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PiesCubicos",
                table: "Paquetes",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TarifaPieCubicoGeneral",
                table: "Configuraciones",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Configuraciones",
                keyColumn: "Id",
                keyValue: 1,
                column: "TarifaPieCubicoGeneral",
                value: 22.00m);

            migrationBuilder.UpdateData(
                table: "Configuraciones",
                keyColumn: "Id",
                keyValue: 2,
                column: "TarifaPieCubicoGeneral",
                value: 22.00m);

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$o.urzG/.z/5OIwMHV0zmrumxn7riZk6xk4T/QEQzC35VbH35cRe1e");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$icTMyIesMUIvj/PAeV5FAupgYxH3jpzU8TTBDepLtiELg/na8f1LC");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPiesCubicos",
                table: "Proformas");

            migrationBuilder.DropColumn(
                name: "AltoIn",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "AnchoIn",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "LargoIn",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "PiesCubicos",
                table: "Paquetes");

            migrationBuilder.DropColumn(
                name: "TarifaPieCubicoGeneral",
                table: "Configuraciones");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$BL60KFr6Y1zWIbx/hY/bSedEdu9ddM.FzluIl5P./bZv0J5XAygrW");

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$fi.xeeT7smhCsWKubydhBeFhqqsTNY9VI9tQvqPLMM5z88a.s9VfC");
        }
    }
}
