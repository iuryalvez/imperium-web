using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "Descricao",
                value: "Secretário de clínica");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "Descricao",
                value: "Técnico de clínica");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "Email", "EmailConfirmed", "Formacao", "LockoutEnabled", "LockoutEnd", "Nome", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 1, 0, "edb70cdb-a404-4a5c-b0d7-bcfade249e73", new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(5965), new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(5979), "admin@gmail.com", false, "Administrador do sistema", false, null, "Administrador", "ADMIN@GMAIL.COM", "ADMIN@GMAIL.COM", "AQAAAAIAAYagAAAAEFJrVIR2h5CespazX1J/RrN5Sbr1xQta+DhcNhhz9GMp35+L5dNw7yElaqoaS7NYgg==", null, false, "cba05c24-2084-4d45-a713-27ce5be3192f", false, "admin@gmail.com" },
                    { 2, 0, "6e4bf261-2fe7-431e-a1ad-79512baf36d2", new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(6027), new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(6041), "gestor@gmail.com", false, "Gestão Clínica", false, null, "Gestor da Clínica", "GESTOR@GMAIL.COM", "GESTOR@GMAIL.COM", "AQAAAAIAAYagAAAAEPssqgjmVfUKKc+dF0TO0v52aqOSBuMNYzJL+94sikuZc1uvh+520+OuFqklbZg50w==", null, false, "51df7511-3264-4c55-b659-040a587b7973", false, "gestor@gmail.com" },
                    { 3, 0, "ef03c35f-2a7c-4f99-af06-5aac4dc89e18", new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(6084), new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(6094), "veterinario@gmail.com", false, "Medicina Veterinária", false, null, "Veterinário", "VETERINARIO@GMAIL.COM", "VETERINARIO@GMAIL.COM", "AQAAAAIAAYagAAAAEDjFg443m+Bu7hLUxUmWsL7XBgbKaBlmKC/4F8v55cdS2OHdNk9SXVTUCK2yijXDBQ==", null, false, "f9cf95ea-0b56-4c94-b807-ea2a3634e8a4", false, "veterinario@gmail.com" },
                    { 4, 0, "85a9f739-4ec0-4f4d-99a6-b5610acf66e2", new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(6142), new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(6152), "secretario@gmail.com", false, "Secretariado", false, null, "Secretário", "SECRETARIO@GMAIL.COM", "SECRETARIO@GMAIL.COM", "AQAAAAIAAYagAAAAEHqCO5lwmWZ+UoKOXstUGUF9zFwG6jEgcIjYODUFvtMMEuWr8g4H1z4TCQDYJC5YGA==", null, false, "4c6c7e8d-2a28-42e1-9060-74e648e50e57", false, "secretario@gmail.com" },
                    { 5, 0, "82842c66-6e19-4f1d-b49a-c4c7623c62eb", new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(6199), new DateTime(2025, 1, 7, 22, 52, 27, 879, DateTimeKind.Utc).AddTicks(6214), "tecnico@gmail.com", false, "Técnico Veterinário", false, null, "Técnico", "TECNICO@GMAIL.COM", "TECNICO@GMAIL.COM", "AQAAAAIAAYagAAAAEEFBcjlinY/7gT78Xdym3rtCncCBnZoEbe3OSnH4OgAspVQjulJg1c9l45kY8Gz+gQ==", null, false, "d3ee448a-bf3a-40e7-83e3-24a7a33e0e07", false, "tecnico@gmail.com" }
                });

            migrationBuilder.InsertData(
                table: "Clinicas",
                columns: new[] { "Id", "Cep", "Cnpj", "Email", "EndCidade", "EndEstado", "EndNumero", "Endereco", "NomeClinica", "NomeFantasia", "Telefone1", "Whatsapp" },
                values: new object[,]
                {
                    { 1, "", "00000000000100", "", "", "", 0, "", "Clinica Animais 1", "Clinica Teste 1", "", "" },
                    { 2, "", "00000000000200", "", "", "", 0, "", "Clinica Animais 2", "Clinica Teste 2", "", "" },
                    { 3, "", "00000000000300", "", "", "", 0, "", "Clinica Animais 3", "Clinica Teste 3", "", "" }
                });

            migrationBuilder.InsertData(
                table: "UsuariosPerfisClinicas",
                columns: new[] { "ClinicaId", "PerfilId", "UsuarioId", "Id" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 1, 2, 2, 2 },
                    { 2, 2, 2, 3 },
                    { 3, 2, 2, 4 },
                    { 2, 3, 3, 5 },
                    { 2, 4, 4, 6 },
                    { 3, 5, 5, 7 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 1, 1, 1 });

            migrationBuilder.DeleteData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 1, 2, 2 });

            migrationBuilder.DeleteData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 2, 2, 2 });

            migrationBuilder.DeleteData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 3, 2, 2 });

            migrationBuilder.DeleteData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 2, 3, 3 });

            migrationBuilder.DeleteData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 2, 4, 4 });

            migrationBuilder.DeleteData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 3, 5, 5 });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "Descricao",
                value: "Secretario de clínica");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "Descricao",
                value: "Tecnico de clinica");
        }
    }
}
