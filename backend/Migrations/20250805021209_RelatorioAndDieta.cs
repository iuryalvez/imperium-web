using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RelatorioAndDieta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietasPersonalizadas_AcompanhamentosInds_AcompanhamentoIndId",
                table: "DietasPersonalizadas");

            migrationBuilder.DropIndex(
                name: "IX_DietasPersonalizadas_AcompanhamentoIndId",
                table: "DietasPersonalizadas");

            migrationBuilder.RenameColumn(
                name: "AcompanhamentoIndId",
                table: "DietasPersonalizadas",
                newName: "RelatorioId");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "817e9f98-35b6-4fa3-8aa0-5f97453b56cb", new DateTime(2025, 8, 5, 2, 12, 8, 731, DateTimeKind.Utc).AddTicks(5724), new DateTime(2025, 8, 5, 2, 12, 8, 731, DateTimeKind.Utc).AddTicks(5733), "AQAAAAIAAYagAAAAEL9hUIVnWfJvBPP9Rrs9/yWIq+OQ9htdX34ILVrJ43bKuAamA7ZYePcV5QaU1JVYog==", "e6792bb6-0cd7-4006-b0cc-3f2f76e0fb5e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9a078283-c0c6-48cc-ac16-54da08932727", new DateTime(2025, 8, 5, 2, 12, 8, 731, DateTimeKind.Utc).AddTicks(5681), new DateTime(2025, 8, 5, 2, 12, 8, 731, DateTimeKind.Utc).AddTicks(5695), "AQAAAAIAAYagAAAAEJcX3jVdIBrNRKH/t0w26h5Bg0/iYEYflWnw49fOOv4C/LyBQqx/oK6PDeTVyRk3xQ==", "8e325725-2d1d-4596-89e1-273d87182624" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "72b1d934-502d-426e-8d99-58214dcd0ce1", new DateTime(2025, 8, 5, 2, 12, 8, 731, DateTimeKind.Utc).AddTicks(5638), new DateTime(2025, 8, 5, 2, 12, 8, 731, DateTimeKind.Utc).AddTicks(5652), "AQAAAAIAAYagAAAAELCaTRvEXZjMarM1XXdmuvsOqCtm2St4dSdbXbtuSUBct0+6uQBhraIiiQ0lBsLiIA==", "bfdaae57-3685-45d4-9c34-a26a7c84f768" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "683d35b9-06a6-4b1c-ba28-39e8d998c4fd", new DateTime(2025, 8, 5, 2, 12, 8, 731, DateTimeKind.Utc).AddTicks(5594), new DateTime(2025, 8, 5, 2, 12, 8, 731, DateTimeKind.Utc).AddTicks(5609), "AQAAAAIAAYagAAAAEDXe5yuOX8rH3/gmx/p+IxaXeKI6B2AqpD613RfbuBDr9c9M6CYNJ7p3R2Wx1Uj0Dw==", "58cbc6b4-232f-4346-9612-fb94b34dfd08" });

            migrationBuilder.CreateIndex(
                name: "IX_DietasPersonalizadas_RelatorioId",
                table: "DietasPersonalizadas",
                column: "RelatorioId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DietasPersonalizadas_Relatorios_RelatorioId",
                table: "DietasPersonalizadas",
                column: "RelatorioId",
                principalTable: "Relatorios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietasPersonalizadas_Relatorios_RelatorioId",
                table: "DietasPersonalizadas");

            migrationBuilder.DropIndex(
                name: "IX_DietasPersonalizadas_RelatorioId",
                table: "DietasPersonalizadas");

            migrationBuilder.RenameColumn(
                name: "RelatorioId",
                table: "DietasPersonalizadas",
                newName: "AcompanhamentoIndId");

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

            migrationBuilder.CreateIndex(
                name: "IX_DietasPersonalizadas_AcompanhamentoIndId",
                table: "DietasPersonalizadas",
                column: "AcompanhamentoIndId");

            migrationBuilder.AddForeignKey(
                name: "FK_DietasPersonalizadas_AcompanhamentosInds_AcompanhamentoIndId",
                table: "DietasPersonalizadas",
                column: "AcompanhamentoIndId",
                principalTable: "AcompanhamentosInds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
