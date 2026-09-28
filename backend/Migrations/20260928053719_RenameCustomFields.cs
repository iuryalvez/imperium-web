using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RenameCustomFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PotenciaCustom",
                table: "Dietas",
                newName: "Potencia");

            migrationBuilder.RenameColumn(
                name: "MultiplicadorEnergeticoCustom",
                table: "Dietas",
                newName: "MultiplicadorEnergetico");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Potencia",
                table: "Dietas",
                newName: "PotenciaCustom");

            migrationBuilder.RenameColumn(
                name: "MultiplicadorEnergetico",
                table: "Dietas",
                newName: "MultiplicadorEnergeticoCustom");
        }
    }
}
