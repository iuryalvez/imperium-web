using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class muitas_mudancas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_AcompanhamentosInds_AcompanhamentoIndId",
                table: "Consultas");

            migrationBuilder.RenameColumn(
                name: "AcompanhamentoIndId",
                table: "Consultas",
                newName: "ClinicaId");

            migrationBuilder.RenameIndex(
                name: "IX_Consultas_AcompanhamentoIndId",
                table: "Consultas",
                newName: "IX_Consultas_ClinicaId");

            migrationBuilder.AddColumn<int>(
                name: "AcompanhamentoIndId",
                table: "Relatorios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_AcompanhamentoIndId",
                table: "Relatorios",
                column: "AcompanhamentoIndId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_Clinicas_ClinicaId",
                table: "Consultas",
                column: "ClinicaId",
                principalTable: "Clinicas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Relatorios_AcompanhamentosInds_AcompanhamentoIndId",
                table: "Relatorios",
                column: "AcompanhamentoIndId",
                principalTable: "AcompanhamentosInds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultas_Clinicas_ClinicaId",
                table: "Consultas");

            migrationBuilder.DropForeignKey(
                name: "FK_Relatorios_AcompanhamentosInds_AcompanhamentoIndId",
                table: "Relatorios");

            migrationBuilder.DropIndex(
                name: "IX_Relatorios_AcompanhamentoIndId",
                table: "Relatorios");

            migrationBuilder.DropColumn(
                name: "AcompanhamentoIndId",
                table: "Relatorios");

            migrationBuilder.RenameColumn(
                name: "ClinicaId",
                table: "Consultas",
                newName: "AcompanhamentoIndId");

            migrationBuilder.RenameIndex(
                name: "IX_Consultas_ClinicaId",
                table: "Consultas",
                newName: "IX_Consultas_AcompanhamentoIndId");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultas_AcompanhamentosInds_AcompanhamentoIndId",
                table: "Consultas",
                column: "AcompanhamentoIndId",
                principalTable: "AcompanhamentosInds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
