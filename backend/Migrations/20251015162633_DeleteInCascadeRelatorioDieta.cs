using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class DeleteInCascadeRelatorioDieta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietasPersonalizadas_Racoes_RacaoId",
                table: "DietasPersonalizadas");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Tutores",
                type: "character varying(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Racoes",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3c107449-5e78-43db-844b-9e38c26b02fc", new DateTime(2025, 10, 15, 16, 26, 32, 200, DateTimeKind.Utc).AddTicks(966), new DateTime(2025, 10, 15, 16, 26, 32, 200, DateTimeKind.Utc).AddTicks(981), "AQAAAAIAAYagAAAAEFedxCR1l4TGgBOriC3BxTcTnf/DZSIDJFrwj7/RkItkZOyhbZSzZjtVn0emd8mTCw==", "493b21bf-945c-4682-84ab-4ba4bf9ed724" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e4c6708f-4604-4e49-8104-6455c2c7caa9", new DateTime(2025, 10, 15, 16, 26, 32, 200, DateTimeKind.Utc).AddTicks(861), new DateTime(2025, 10, 15, 16, 26, 32, 200, DateTimeKind.Utc).AddTicks(875), "AQAAAAIAAYagAAAAEKlfrUmiLLtVu+EdTga3DejXG7pIm9ugBx+tMmavC9x08wqT8snQjttcZc+WdHLC4w==", "aa2e0018-6349-4485-bcef-51a166bdad4f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "54eaf776-f0c7-4711-83bd-1fe54729d7a2", new DateTime(2025, 10, 15, 16, 26, 32, 200, DateTimeKind.Utc).AddTicks(818), new DateTime(2025, 10, 15, 16, 26, 32, 200, DateTimeKind.Utc).AddTicks(828), "AQAAAAIAAYagAAAAEP+q6+x6UBEzLchZ436KOf8NkkE13GM+Uc7zzad0BkjrIREPLRbYp3j1zb584IX85w==", "e8f465ec-68fd-4d40-9667-05cc67192ec6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "24e986b2-9b3c-4b25-a3cd-8b540f82db62", new DateTime(2025, 10, 15, 16, 26, 32, 200, DateTimeKind.Utc).AddTicks(771), new DateTime(2025, 10, 15, 16, 26, 32, 200, DateTimeKind.Utc).AddTicks(785), "AQAAAAIAAYagAAAAELijWJBS4MxXbIzllPNJoizqs3oEX1tbCknDc838Lh7ftEysdkptKYKYxBZ73meptA==", "3bba5045-5aa9-4d39-96a7-011434c92327" });

            migrationBuilder.AddForeignKey(
                name: "FK_DietasPersonalizadas_Racoes_RacaoId",
                table: "DietasPersonalizadas",
                column: "RacaoId",
                principalTable: "Racoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DietasPersonalizadas_Racoes_RacaoId",
                table: "DietasPersonalizadas");

            migrationBuilder.AlterColumn<string>(
                name: "Cpf",
                table: "Tutores",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(11)",
                oldMaxLength: 11);

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Racoes",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

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

            migrationBuilder.AddForeignKey(
                name: "FK_DietasPersonalizadas_Racoes_RacaoId",
                table: "DietasPersonalizadas",
                column: "RacaoId",
                principalTable: "Racoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
