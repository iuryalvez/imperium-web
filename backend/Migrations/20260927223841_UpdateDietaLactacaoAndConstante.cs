using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDietaLactacaoAndConstante : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdicionalGestacao",
                table: "Dietas",
                newName: "AdicionalConstante");

            migrationBuilder.AddColumn<double>(
                name: "AdicionalValor",
                table: "Dietas",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrevisoesJson",
                table: "Dietas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdicionalValor",
                table: "Dietas");

            migrationBuilder.DropColumn(
                name: "PrevisoesJson",
                table: "Dietas");

            migrationBuilder.RenameColumn(
                name: "AdicionalConstante",
                table: "Dietas",
                newName: "AdicionalGestacao");
        }
    }
}
