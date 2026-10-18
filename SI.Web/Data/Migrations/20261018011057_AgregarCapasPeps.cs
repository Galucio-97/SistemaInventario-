using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SI.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCapasPeps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CapasPeps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductoId = table.Column<int>(type: "int", nullable: false),
                    MovimientoId = table.Column<int>(type: "int", nullable: false),
                    FechaEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantidadInicial = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CantidadDisponible = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapasPeps", x => x.Id);
                    table.CheckConstraint("CK_CapasPeps_CantidadDisponible", "[CantidadDisponible] >= 0");
                    table.ForeignKey(
                        name: "FK_CapasPeps_Movimientos_MovimientoId",
                        column: x => x.MovimientoId,
                        principalTable: "Movimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CapasPeps_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapasPeps_MovimientoId",
                table: "CapasPeps",
                column: "MovimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_CapasPeps_Producto_FechaEntrada_Id",
                table: "CapasPeps",
                columns: new[] { "ProductoId", "FechaEntrada", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapasPeps");
        }
    }
}
