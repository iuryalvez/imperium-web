using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class testeInjectionPerfis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
