using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoDotNet10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "87654321-4321-4321-4321-cba987654321", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEEqfDnpAOVKyqg7A7FlvRKFZPdqMTKTlOPRzjPP3uyO/vtYKGA+Bd6wmkUSWOJcEZw==", "12345678-1234-1234-1234-123456789abc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "87654321-4321-4321-4321-cba987654321", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEEqfDnpAOVKyqg7A7FlvRKFZPdqMTKTlOPRzjPP3uyO/vtYKGA+Bd6wmkUSWOJcEZw==", "12345678-1234-1234-1234-123456789abc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "87654321-4321-4321-4321-cba987654321", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEEqfDnpAOVKyqg7A7FlvRKFZPdqMTKTlOPRzjPP3uyO/vtYKGA+Bd6wmkUSWOJcEZw==", "12345678-1234-1234-1234-123456789abc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCadastro", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "87654321-4321-4321-4321-cba987654321", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEEqfDnpAOVKyqg7A7FlvRKFZPdqMTKTlOPRzjPP3uyO/vtYKGA+Bd6wmkUSWOJcEZw==", "12345678-1234-1234-1234-123456789abc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
