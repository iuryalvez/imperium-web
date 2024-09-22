using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class TutorAnimalClinica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animais_Clinicas_ClinicaId",
                table: "Animais");

            migrationBuilder.DropForeignKey(
                name: "FK_Animais_Tutores_TutorId",
                table: "Animais");

            migrationBuilder.DropForeignKey(
                name: "FK_Tutores_Clinicas_ClinicaId",
                table: "Tutores");

            migrationBuilder.DropIndex(
                name: "IX_Animais_ClinicaId",
                table: "Animais");

            migrationBuilder.DropIndex(
                name: "IX_Animais_TutorId",
                table: "Animais");

            migrationBuilder.DropColumn(
                name: "ClinicaId",
                table: "Animais");

            migrationBuilder.DropColumn(
                name: "TutorId",
                table: "Animais");

            migrationBuilder.CreateTable(
                name: "TutoresAnimaisClinicas",
                columns: table => new
                {
                    TutorId = table.Column<int>(type: "integer", nullable: false),
                    AnimalId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutoresAnimaisClinicas", x => new { x.TutorId, x.AnimalId, x.ClinicaId });
                    table.ForeignKey(
                        name: "FK_TutoresAnimaisClinicas_Animais_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TutoresAnimaisClinicas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TutoresAnimaisClinicas_Tutores_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TutoresAnimaisClinicas_AnimalId",
                table: "TutoresAnimaisClinicas",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_TutoresAnimaisClinicas_ClinicaId",
                table: "TutoresAnimaisClinicas",
                column: "ClinicaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tutores_Clinicas_ClinicaId",
                table: "Tutores",
                column: "ClinicaId",
                principalTable: "Clinicas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tutores_Clinicas_ClinicaId",
                table: "Tutores");

            migrationBuilder.DropTable(
                name: "TutoresAnimaisClinicas");

            migrationBuilder.AddColumn<int>(
                name: "ClinicaId",
                table: "Animais",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TutorId",
                table: "Animais",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Animais_ClinicaId",
                table: "Animais",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Animais_TutorId",
                table: "Animais",
                column: "TutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Animais_Clinicas_ClinicaId",
                table: "Animais",
                column: "ClinicaId",
                principalTable: "Clinicas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Animais_Tutores_TutorId",
                table: "Animais",
                column: "TutorId",
                principalTable: "Tutores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tutores_Clinicas_ClinicaId",
                table: "Tutores",
                column: "ClinicaId",
                principalTable: "Clinicas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
