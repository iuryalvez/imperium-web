using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RacaoClinica1N : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Eea",
                table: "Racoes",
                newName: "Ee");

            migrationBuilder.AddColumn<int>(
                name: "ClinicaId",
                table: "Racoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Racoes_ClinicaId",
                table: "Racoes",
                column: "ClinicaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Racoes_Clinicas_ClinicaId",
                table: "Racoes",
                column: "ClinicaId",
                principalTable: "Clinicas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Racoes_Clinicas_ClinicaId",
                table: "Racoes");

            migrationBuilder.DropIndex(
                name: "IX_Racoes_ClinicaId",
                table: "Racoes");

            migrationBuilder.DropColumn(
                name: "ClinicaId",
                table: "Racoes");

            migrationBuilder.RenameColumn(
                name: "Ee",
                table: "Racoes",
                newName: "Eea");
        }
    }
}
