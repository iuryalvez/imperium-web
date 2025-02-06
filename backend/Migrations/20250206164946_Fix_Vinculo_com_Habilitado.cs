using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Vinculo_com_Habilitado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Habilitado",
                table: "UsuariosPerfisClinicas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e71aae35-1850-4754-964d-9bea613375f1", new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3121), new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3134), "AQAAAAIAAYagAAAAEI5Jssua91v2aUR4Vhc3j9QClLI0DoCVC5bMKFquatARck/mRoSpvwUqURz82dyiUA==", "b5ae5f62-1911-46f1-8ff1-edbe119e1b84" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1e300581-7a5e-4504-aa13-4cc510384aef", new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3167), new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3181), "AQAAAAIAAYagAAAAEFEAiYKTxU4LPr9rCzFOkWOt12wX8uiCSseYb05G/eDlsLJMzby8rlcEtPjggiwPlw==", "e1507572-a0b3-4a28-aa91-406bf371b414" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e16ff4bb-ab3d-46be-8635-9d2d8616991c", new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3210), new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3219), "AQAAAAIAAYagAAAAEJDHdmJVYzdp+lwRF0oV0adQiOkvExC+/AFm5OTz2FHwJ6+SY1yC3s39M+8JvX311w==", "923b9652-fe0e-4065-9969-041a823d6871" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f3d8d015-f275-415b-85ce-de6b819a50fb", new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3253), new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3262), "AQAAAAIAAYagAAAAELUKQrK85XM2ZYq2rBA1/9Erry1FdxpZfNYh1ZsI9DD6XeX89E4k5az8OhlNPstPxg==", "6aa412bd-5236-4dad-a62c-12cad4881613" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "01dfae29-e26a-45d0-964c-4e9b3cf5d1b9", new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3292), new DateTime(2025, 2, 6, 16, 49, 45, 636, DateTimeKind.Utc).AddTicks(3306), "AQAAAAIAAYagAAAAENQEDzYCSAA7+xZbkFFr+Zxz+l0NHmjB3G3BDBRCSgaP6YqOE8E2J/rWdkeHkq8+yg==", "1ae04ecf-2cef-461d-89f7-99c3d350544b" });

            migrationBuilder.UpdateData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "NomeClinica", "NomeFantasia" },
                values: new object[] { "Clinica EVZ LTDA", "Clinica EVZ / UFG" });

            migrationBuilder.UpdateData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "NomeClinica", "NomeFantasia" },
                values: new object[] { "Clinica Ceron Empreendimentos", "Clinica Ceron" });

            migrationBuilder.UpdateData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "NomeClinica", "NomeFantasia" },
                values: new object[] { "Clinica HADN S.A.", "Clinica Nascimento" });

            migrationBuilder.UpdateData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 1, 1, 1 },
                column: "Habilitado",
                value: true);

            migrationBuilder.UpdateData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 2, 1, 1 },
                column: "Habilitado",
                value: true);

            migrationBuilder.UpdateData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 3, 1, 1 },
                column: "Habilitado",
                value: true);

            migrationBuilder.UpdateData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 1, 2, 2 },
                column: "Habilitado",
                value: true);

            migrationBuilder.UpdateData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 2, 2, 2 },
                column: "Habilitado",
                value: true);

            migrationBuilder.UpdateData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 3, 2, 2 },
                column: "Habilitado",
                value: true);

            migrationBuilder.UpdateData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 2, 3, 3 },
                column: "Habilitado",
                value: true);

            migrationBuilder.UpdateData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 2, 4, 4 },
                column: "Habilitado",
                value: true);

            migrationBuilder.UpdateData(
                table: "UsuariosPerfisClinicas",
                keyColumns: new[] { "ClinicaId", "PerfilId", "UsuarioId" },
                keyValues: new object[] { 3, 5, 5 },
                column: "Habilitado",
                value: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Habilitado",
                table: "UsuariosPerfisClinicas");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1eccb193-562c-44a8-bfcd-c8343a4a6b95", new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2272), new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2286), "AQAAAAIAAYagAAAAEO2agAEIfmth7Wm8BIcWRr4XHwmNLvyu/5a74lcjvhmNQyNxDcvF02Q3JmB6iRNEyQ==", "e0c355e2-9977-4c6b-a8be-744dad88ccdd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1fda7b85-c6ec-4b57-a751-a4b788d94a35", new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2315), new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2324), "AQAAAAIAAYagAAAAEDweCq4rJKFHX8dLIVBZhUB9Lqv6nla61bRTiolP7Tb/9zk+VZISpJAnDe2PyiJf+w==", "34be0bd3-6142-4eb4-b652-92ba29548cbb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "87e751fb-c48a-4090-9dbc-dcde26f70722", new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2352), new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2362), "AQAAAAIAAYagAAAAEG2nucriM+/v2hrGVVqHmMQXP88QMJsM6AQYq4pD1cBq6+iLjTTo8enMB65WfIqkCg==", "f1926567-9719-4d3a-b551-1d0f9bdeb84c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "23d09245-b0a9-4d57-8b78-52e756ec5d7c", new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2410), new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2419), "AQAAAAIAAYagAAAAEE8d4zBt3GxbPxj9C9VHT1gR+2s9WcIB/eQNgWRBtWtoJxR2cWM4oeEUB92Xm5fi0A==", "624e585c-0acc-4a05-936d-5ff040d48798" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "214e5df9-6b3a-4b92-af3d-461fa2210ef4", new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2449), new DateTime(2025, 2, 3, 19, 8, 26, 567, DateTimeKind.Utc).AddTicks(2463), "AQAAAAIAAYagAAAAEIMqNDV90jsMBSgA2ZrCoCqpf0EIkfcm2qnMfUxIhFeFz1sdkdwhLEBA/jEwn41f3A==", "538d9714-bdb5-4417-a782-2013e4039826" });

            migrationBuilder.UpdateData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "NomeClinica", "NomeFantasia" },
                values: new object[] { "Clinica Animais 1", "Clinica Teste 1" });

            migrationBuilder.UpdateData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "NomeClinica", "NomeFantasia" },
                values: new object[] { "Clinica Animais 2", "Clinica Teste 2" });

            migrationBuilder.UpdateData(
                table: "Clinicas",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "NomeClinica", "NomeFantasia" },
                values: new object[] { "Clinica Animais 3", "Clinica Teste 3" });
        }
    }
}
