using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RefactorAcompanhamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relatorios_AcompanhamentosInds_AcompanhamentoIndividualizad~",
                table: "Relatorios");

            migrationBuilder.RenameColumn(
                name: "AcompanhamentoIndividualizadoId",
                table: "Relatorios",
                newName: "AcompanhamentoId");

            migrationBuilder.RenameIndex(
                name: "IX_Relatorios_AcompanhamentoIndividualizadoId",
                table: "Relatorios",
                newName: "IX_Relatorios_AcompanhamentoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Relatorios_AcompanhamentosInds_AcompanhamentoId",
                table: "Relatorios",
                column: "AcompanhamentoId",
                principalTable: "AcompanhamentosInds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Relatorios_AcompanhamentosInds_AcompanhamentoId",
                table: "Relatorios");

            migrationBuilder.RenameColumn(
                name: "AcompanhamentoId",
                table: "Relatorios",
                newName: "AcompanhamentoIndividualizadoId");

            migrationBuilder.RenameIndex(
                name: "IX_Relatorios_AcompanhamentoId",
                table: "Relatorios",
                newName: "IX_Relatorios_AcompanhamentoIndividualizadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Relatorios_AcompanhamentosInds_AcompanhamentoIndividualizad~",
                table: "Relatorios",
                column: "AcompanhamentoIndividualizadoId",
                principalTable: "AcompanhamentosInds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
