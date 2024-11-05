using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class updateusuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Descricao", "Name", "NormalizedName", "Tipo" },
                values: new object[,]
                {
                    { 1, null, "Administrador do sistema", "ADMIN", null, 1 },
                    { 2, null, "Gestor de clínica", "GESTOR", null, 2 },
                    { 3, null, "Veterinário de clínica", "VETERINARIO", null, 3 },
                    { 4, null, "Secretario de clínica", "SECRETARIO", null, 4 },
                    { 5, null, "Tecnico de clinica", "TECNICO", null, 5 },
                    { 98, null, "Usuário pendente de ativação", "PENDENTE", null, 98 },
                    { 99, null, "Usuário inativo", "INATIVO", null, 99 }
                });

            migrationBuilder.InsertData(
                table: "Clinicas",
                columns: new[] { "Id", "Cep", "Cnpj", "Email", "EndCidade", "EndEstado", "EndNumero", "Endereco", "NomeClinica", "NomeFantasia", "Telefone1", "Whatsapp" },
                values: new object[] { 1, "", "00000000000100", "", "", "", 0, "", "Clinica Animais", "Clinica Teste", "", "" });
        }
    }
}
