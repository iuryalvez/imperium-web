using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposFormulaDieta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Expoente",
                table: "DietasPersonalizadas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FatorK",
                table: "DietasPersonalizadas",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomizado",
                table: "DietasPersonalizadas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PerfilNutricional",
                table: "DietasPersonalizadas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 122,
                column: "Nome",
                value: "American Shorthair");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 128,
                column: "Nome",
                value: "British Shorthair");

            migrationBuilder.InsertData(
                table: "Racas",
                columns: new[] { "Id", "ClinicaId", "Especie", "Nome", "PesoMaximoReferenciaFemea", "PesoMaximoReferenciaMacho", "PesoMinimoReferenciaFemea", "PesoMinimoReferenciaMacho", "Porte", "Referencia" },
                values: new object[] { 160, null, 1, "American Pit Bull Terrier", null, null, null, null, 2, "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 160);

            migrationBuilder.DropColumn(
                name: "Expoente",
                table: "DietasPersonalizadas");

            migrationBuilder.DropColumn(
                name: "FatorK",
                table: "DietasPersonalizadas");

            migrationBuilder.DropColumn(
                name: "IsCustomizado",
                table: "DietasPersonalizadas");

            migrationBuilder.DropColumn(
                name: "PerfilNutricional",
                table: "DietasPersonalizadas");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 122,
                column: "Nome",
                value: "Gato de Pelo Curto Americano");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 128,
                column: "Nome",
                value: "Gato de Pelo Curto Inglês");
        }
    }
}
