using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSucursalToProveedor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SucursalId",
                table: "Proveedores",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.UpdateData(
                table: "Proveedores",
                keyColumn: "Id",
                keyValue: 1,
                column: "SucursalId",
                value: 1);

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

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_SucursalId",
                table: "Proveedores",
                column: "SucursalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Proveedores_Sucursales_SucursalId",
                table: "Proveedores",
                column: "SucursalId",
                principalTable: "Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proveedores_Sucursales_SucursalId",
                table: "Proveedores");

            migrationBuilder.DropIndex(
                name: "IX_Proveedores_SucursalId",
                table: "Proveedores");

            migrationBuilder.DropColumn(
                name: "SucursalId",
                table: "Proveedores");

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
        }
    }
}
