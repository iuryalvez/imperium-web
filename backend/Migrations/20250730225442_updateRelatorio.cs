using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class updateRelatorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGestante",
                table: "Relatorios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IsLactanteNumeroFilhotes",
                table: "Relatorios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SemanaGestacao",
                table: "Relatorios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SemanaLactacao",
                table: "Relatorios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "18fb6ac1-88f4-45e0-b6f3-d0791d066283", new DateTime(2025, 7, 30, 22, 54, 41, 325, DateTimeKind.Utc).AddTicks(8367), new DateTime(2025, 7, 30, 22, 54, 41, 325, DateTimeKind.Utc).AddTicks(8377), "AQAAAAIAAYagAAAAEKKlWB3w7Z7TXW4fRzznB7JyllicXz70YZgiPdVgfW8ogAkly8+cbgfnGw51FGop3Q==", "d852edd6-1f5d-4816-b5ec-1d3b012230ff" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c1c94893-fe38-48a4-8b9a-2b05ee34d630", new DateTime(2025, 7, 30, 22, 54, 41, 325, DateTimeKind.Utc).AddTicks(8324), new DateTime(2025, 7, 30, 22, 54, 41, 325, DateTimeKind.Utc).AddTicks(8334), "AQAAAAIAAYagAAAAENIltYLC/kURhEOAqmD17tJGQuXeqxHtlhM/OVyuBGWaR3ZfA5wDI7n8k/Nx0q6NzQ==", "7c3b3f71-c440-4e5d-a418-b615fd451b67" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cf5b3013-87f8-4fda-bc37-bfb489e8aa93", new DateTime(2025, 7, 30, 22, 54, 41, 325, DateTimeKind.Utc).AddTicks(8280), new DateTime(2025, 7, 30, 22, 54, 41, 325, DateTimeKind.Utc).AddTicks(8295), "AQAAAAIAAYagAAAAED+PuTclHw3gVKYj6FT6qTOR7WiDxJ8gmKSFUlKFf9LqDwsvUu0DnVCCAZAvnBUN5g==", "147e0abd-6a59-403e-8729-9710a5d867ab" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d37583c2-f50e-4f0c-889d-e77267415441", new DateTime(2025, 7, 30, 22, 54, 41, 325, DateTimeKind.Utc).AddTicks(8237), new DateTime(2025, 7, 30, 22, 54, 41, 325, DateTimeKind.Utc).AddTicks(8251), "AQAAAAIAAYagAAAAEAJJjqcDXvPA0yPc8CemJOpc0tANIdSE2IBlEa0AC5npABqDpF7IMdrp4SBFWl0CJA==", "79ce9e13-8afc-4aac-89ff-1e632a8bbd8b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGestante",
                table: "Relatorios");

            migrationBuilder.DropColumn(
                name: "IsLactanteNumeroFilhotes",
                table: "Relatorios");

            migrationBuilder.DropColumn(
                name: "SemanaGestacao",
                table: "Relatorios");

            migrationBuilder.DropColumn(
                name: "SemanaLactacao",
                table: "Relatorios");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "557077ba-65b8-472f-a7d2-b3c92d015b31", new DateTime(2025, 7, 29, 4, 56, 30, 79, DateTimeKind.Utc).AddTicks(7671), new DateTime(2025, 7, 29, 4, 56, 30, 79, DateTimeKind.Utc).AddTicks(7681), "AQAAAAIAAYagAAAAEEuZWndJagmDJv/euRU40B7gmJRSburdtp7Tm2eVeJUMI7c/uZ23o7c70WtlGtstZw==", "80ae7dd7-e7d9-4c45-9879-6b05b82ee5cd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ef08f397-43f6-43d8-b247-528bb42718cd", new DateTime(2025, 7, 29, 4, 56, 30, 79, DateTimeKind.Utc).AddTicks(7628), new DateTime(2025, 7, 29, 4, 56, 30, 79, DateTimeKind.Utc).AddTicks(7643), "AQAAAAIAAYagAAAAEEDpjD4D1MMrO3S6otFfuofcyvS4kBU1Zg6F76mTaC3wDDeSCnamA/ck7RmTa/5BOA==", "05874753-4469-4335-9799-6f321fc67370" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "591eecf4-31d3-4dba-a197-319724f27f23", new DateTime(2025, 7, 29, 4, 56, 30, 79, DateTimeKind.Utc).AddTicks(7586), new DateTime(2025, 7, 29, 4, 56, 30, 79, DateTimeKind.Utc).AddTicks(7600), "AQAAAAIAAYagAAAAEMB/iDqB0AwZddoYCHBnc5fkpTBLTdTMRAe7R05ifVP3FyAU6oyw3qeSAi+gPE5iIA==", "e8293807-c814-49a6-9b5a-c531e7346160" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "91ca9dff-e5e4-4482-87e3-136589395522", new DateTime(2025, 7, 29, 4, 56, 30, 79, DateTimeKind.Utc).AddTicks(7544), new DateTime(2025, 7, 29, 4, 56, 30, 79, DateTimeKind.Utc).AddTicks(7553), "AQAAAAIAAYagAAAAEOTY5DIf8jctGBRF/A32B2fQ/AKjXNvHxh+cuY5ztaUMmK5LAsHK26TgvcnuBDwKAg==", "35a162b9-3bec-4bd1-94f2-158891cfa723" });
        }
    }
}
