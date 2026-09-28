using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDietaIndependentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumeroFilhotes",
                table: "Dietas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SemanaGestacao",
                table: "Dietas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SemanaLactacao",
                table: "Dietas",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumeroFilhotes",
                table: "Dietas");

            migrationBuilder.DropColumn(
                name: "SemanaGestacao",
                table: "Dietas");

            migrationBuilder.DropColumn(
                name: "SemanaLactacao",
                table: "Dietas");
        }
    }
}
