using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AtualizaNomenclaturaRacas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 160);

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 28,
                column: "Nome",
                value: "Pug");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 58,
                column: "Nome",
                value: "Dogue Alemão (Grande Dinamarquês)");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 78,
                column: "Nome",
                value: "Pequinês");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 84,
                column: "Nome",
                value: "Pointer Alemão (Braco Alemão)");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 88,
                column: "Nome",
                value: "Flat-Coated Retriever");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 91,
                column: "Nome",
                value: "Samoieda");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 109,
                column: "Nome",
                value: "Terra-nova");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 119,
                column: "Nome",
                value: "Abissínio");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 120,
                column: "Nome",
                value: "Bobtail Americano");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 121,
                column: "Nome",
                value: "Curl Americano");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 122,
                column: "Nome",
                value: "Gato de Pelo Curto Americano");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 124,
                column: "Nome",
                value: "Balinês");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 125,
                column: "Nome",
                value: "Bengal (Gato-de-Bengala)");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 126,
                column: "Nome",
                value: "Birmanês (Sagrado da Birmânia)");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 127,
                column: "Nome",
                value: "Bombay (Bombaim)");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 128,
                column: "Nome",
                value: "Gato de Pelo Curto Inglês");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 133,
                column: "Nome",
                value: "Mau Egípcio");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 134,
                column: "Nome",
                value: "Exótico");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 135,
                column: "Nome",
                value: "Havana Brown (Oriental Havana)");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 136,
                column: "Nome",
                value: "Himalaio");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 137,
                column: "Nome",
                value: "Bobtail Japonês");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 141,
                column: "Nome",
                value: "Manx (Manês)");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 142,
                column: "Nome",
                value: "Norueguês da Floresta");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 144,
                column: "Nome",
                value: "Oriental Shorthair");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 145,
                column: "Nome",
                value: "Persa");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 148,
                column: "Nome",
                value: "Azul Russo");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 152,
                column: "Nome",
                value: "Siamês");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 153,
                column: "Nome",
                value: "Siberiano");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 157,
                column: "Nome",
                value: "Tonquinês");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 158,
                column: "Nome",
                value: "Angorá (Turkish Angora)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 28,
                column: "Nome",
                value: "Carlino");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 58,
                column: "Nome",
                value: "Grand Danois");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 78,
                column: "Nome",
                value: "Pequinois");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 84,
                column: "Nome",
                value: "Ponteiro Alemão");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 88,
                column: "Nome",
                value: "Retrivier de Pêlo Liso");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 91,
                column: "Nome",
                value: "Samoyedo");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 109,
                column: "Nome",
                value: "Terranova");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 119,
                column: "Nome",
                value: "Abyssinian");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 120,
                column: "Nome",
                value: "American Bobtail");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 121,
                column: "Nome",
                value: "American Curl");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 122,
                column: "Nome",
                value: "American Shorthair");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 124,
                column: "Nome",
                value: "Balinese");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 125,
                column: "Nome",
                value: "Bengal");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 126,
                column: "Nome",
                value: "Birman");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 127,
                column: "Nome",
                value: "Bombay");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 128,
                column: "Nome",
                value: "British Shorthair");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 133,
                column: "Nome",
                value: "Egyptian Mau");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 134,
                column: "Nome",
                value: "Exotic Shorthair");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 135,
                column: "Nome",
                value: "Havana Brown");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 136,
                column: "Nome",
                value: "Himalayan");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 137,
                column: "Nome",
                value: "Japanese Bobtail");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 141,
                column: "Nome",
                value: "Manx");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 142,
                column: "Nome",
                value: "Norwegian Forest Cat");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 144,
                column: "Nome",
                value: "Oriental");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 145,
                column: "Nome",
                value: "Persian");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 148,
                column: "Nome",
                value: "Russian Blue");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 152,
                column: "Nome",
                value: "Siamese");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 153,
                column: "Nome",
                value: "Siberian");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 157,
                column: "Nome",
                value: "Tonkinese");

            migrationBuilder.UpdateData(
                table: "Racas",
                keyColumn: "Id",
                keyValue: 158,
                column: "Nome",
                value: "Turkish Angora");

            migrationBuilder.InsertData(
                table: "Racas",
                columns: new[] { "Id", "ClinicaId", "Especie", "Nome", "PesoMaximoReferenciaFemea", "PesoMaximoReferenciaMacho", "PesoMinimoReferenciaFemea", "PesoMinimoReferenciaMacho", "Porte", "Referencia" },
                values: new object[] { 160, null, 2, "Sem Raça Definida (SRD)", null, null, null, null, null, "" });
        }
    }
}
