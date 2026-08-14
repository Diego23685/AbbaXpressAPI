using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbbaXpress.API.Migrations
{
    /// <inheritdoc />
    public partial class AddProformasAndPaquetesTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proformas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NumeroProforma = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    SucursalOrigenId = table.Column<int>(type: "int", nullable: false),
                    SucursalDestinoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioCreacionId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MetodoPago = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalLbs = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CargoDeliveryUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DescuentoUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalCobradoUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalCostoProveedorUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TipoCambioAplicado = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaFacturacion = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proformas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proformas_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proformas_Sucursales_SucursalDestinoId",
                        column: x => x.SucursalDestinoId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proformas_Sucursales_SucursalOrigenId",
                        column: x => x.SucursalOrigenId,
                        principalTable: "Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Proformas_Usuarios_UsuarioCreacionId",
                        column: x => x.UsuarioCreacionId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Paquetes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProformaId = table.Column<int>(type: "int", nullable: false),
                    Tracking = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Label = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PesoLbs = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ViaEnvio = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Categoria = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TarifaAplicada = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CostoProveedor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SubtotalUSD = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paquetes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Paquetes_Proformas_ProformaId",
                        column: x => x.ProformaId,
                        principalTable: "Proformas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_Paquetes_ProformaId",
                table: "Paquetes",
                column: "ProformaId");

            migrationBuilder.CreateIndex(
                name: "IX_Proformas_ClienteId",
                table: "Proformas",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Proformas_SucursalDestinoId",
                table: "Proformas",
                column: "SucursalDestinoId");

            migrationBuilder.CreateIndex(
                name: "IX_Proformas_SucursalOrigenId",
                table: "Proformas",
                column: "SucursalOrigenId");

            migrationBuilder.CreateIndex(
                name: "IX_Proformas_UsuarioCreacionId",
                table: "Proformas",
                column: "UsuarioCreacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Paquetes");

            migrationBuilder.DropTable(
                name: "Proformas");

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

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 1,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 15, 23, 180, DateTimeKind.Utc).AddTicks(9414));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 2,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 15, 23, 180, DateTimeKind.Utc).AddTicks(9418));

            migrationBuilder.UpdateData(
                table: "Sucursales",
                keyColumn: "Id",
                keyValue: 3,
                column: "FechaRegistro",
                value: new DateTime(2026, 8, 14, 18, 15, 23, 180, DateTimeKind.Utc).AddTicks(9420));

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 15, 23, 180, DateTimeKind.Utc).AddTicks(9552), "$2a$11$nZ6XJMNb1yehYG8Kdv180.Jdl6h3ixdw0GbvgpDyz1udJKM79JqdG" });

            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FechaCreacion", "PasswordHash" },
                values: new object[] { new DateTime(2026, 8, 14, 18, 15, 23, 306, DateTimeKind.Utc).AddTicks(7764), "$2a$11$6tMysVXk5kjibdK5VKHXfec0T1UjkvOEB1lOVtjw.fmD2zEx2ESwO" });
        }
    }
}
