using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGestacaoDietaFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdicionalValor",
                table: "Dietas");

            migrationBuilder.RenameColumn(
                name: "AdicionalConstante",
                table: "Dietas",
                newName: "HasAdicionalGestacaoCao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasAdicionalGestacaoCao",
                table: "Dietas",
                newName: "AdicionalConstante");

            migrationBuilder.AddColumn<double>(
                name: "AdicionalValor",
                table: "Dietas",
                type: "double precision",
                nullable: true);
        }
    }
}
