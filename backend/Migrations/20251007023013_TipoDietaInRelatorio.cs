using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class TipoDietaInRelatorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TipoDieta",
                table: "Relatorios",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9e1a52ae-bbc8-46aa-a014-6f0b1c801416", new DateTime(2025, 10, 7, 2, 30, 12, 305, DateTimeKind.Utc).AddTicks(3967), new DateTime(2025, 10, 7, 2, 30, 12, 305, DateTimeKind.Utc).AddTicks(3981), "AQAAAAIAAYagAAAAEKdY6txkPUnMqnrK0d2jhUo8yPFdDB+khbmsOdHmEsU8wenNYj3POlD5MGex4yJhqg==", "b7c1233e-2672-49ad-aab4-2376a667d0cd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eb586afc-bb24-4c9e-9529-a05a347da6ef", new DateTime(2025, 10, 7, 2, 30, 12, 305, DateTimeKind.Utc).AddTicks(3918), new DateTime(2025, 10, 7, 2, 30, 12, 305, DateTimeKind.Utc).AddTicks(3927), "AQAAAAIAAYagAAAAEKttNqYpi0eedjoUQeJryIEGs7O97Tg5BVJz1RoZaAIpmUo98aoX1VGyxOhVnofSeQ==", "cc31938e-0417-41cc-849d-fcb4658239c4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b249c154-4cfd-4011-a77b-c07cf9d8c8ab", new DateTime(2025, 10, 7, 2, 30, 12, 305, DateTimeKind.Utc).AddTicks(3874), new DateTime(2025, 10, 7, 2, 30, 12, 305, DateTimeKind.Utc).AddTicks(3884), "AQAAAAIAAYagAAAAEN5TIoqyvBeyYtUcTZg99j+iv+ba65M3O4r10tYG6Z1XTCUgut98aTBYcs8uDho3nQ==", "af445d49-0c19-4f72-8759-95c3824cd329" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3da66b20-26cb-478d-93f8-4f7da9135b89", new DateTime(2025, 10, 7, 2, 30, 12, 305, DateTimeKind.Utc).AddTicks(3827), new DateTime(2025, 10, 7, 2, 30, 12, 305, DateTimeKind.Utc).AddTicks(3841), "AQAAAAIAAYagAAAAEFAR0rW9NottT6QNH/Jv6xA37t8rnavBe65inCxHQHn8VV9OdgUg8LQQJxB6VjBU6g==", "35afb9c2-c4c4-4012-a240-181bd0629de5" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoDieta",
                table: "Relatorios");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1f2196a8-dd33-4905-ba98-3a0b777949de", new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(575), new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(589), "AQAAAAIAAYagAAAAEIvOAdAfHTC0jQaMqgnNtAB3YmgEJeqqp1B1/E98hR2XpgFVmwsyo0pb+aR4Lvtwbg==", "08aca275-3ef2-448e-ad2b-b45ca99fd870" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "85b86f42-c722-4e41-ae35-c397e40304c2", new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(532), new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(542), "AQAAAAIAAYagAAAAEABIcWyn4BjvHJYHlDzvfrHOAWVZPhKM/Y92mcmjm/qwYIZWJTk/waXPBzre3zvkzw==", "752242a0-78cb-4197-afa8-a42d41aef7f4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c814c888-4fe1-448a-9fde-f448b4a5ed5b", new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(489), new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(504), "AQAAAAIAAYagAAAAEEggTRzQovc7A/iOMk5C/jtMP2PfJYyClJRbx0M0OgX3RK7iQ5ArVDiNCM/kKmUKQQ==", "16f00e54-a4c3-4762-8551-5ce6f4ccb4e5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e37f6333-700a-4de2-aca3-f439208ea316", new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(443), new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(461), "AQAAAAIAAYagAAAAEIiOThbrE3wdsdLHfEIndNjnrJNsEWRCGnLPnFuFg4dhO3ZvlVXMET27OM9zkNZSIw==", "5e416c1f-6880-45e2-90d8-da95dddf8665" });
        }
    }
}
