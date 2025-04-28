using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClinicaInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCad",
                table: "Clinicas",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "Clinicas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAtivo",
                table: "Clinicas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Clinicas",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4632f56d-dc73-4aa9-9c18-209fe09f3e6f", new DateTime(2025, 4, 28, 20, 44, 29, 712, DateTimeKind.Utc).AddTicks(241), "AQAAAAIAAYagAAAAEBfTUYTxVRcTvhC+KpJmgo0nl2/qtqKsw/Fv+KW1ayc0zawvMyZGoC7wbn5ObWeE9Q==", "c3edbf53-9bc1-412f-a881-8d93448736bb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "PasswordHash", "SecurityStamp" },
                values: new object[] { "65fc9d8a-0b41-477c-975d-45f86235b7fa", new DateTime(2025, 4, 28, 20, 44, 29, 712, DateTimeKind.Utc).AddTicks(208), "AQAAAAIAAYagAAAAENOp3WNeAArOU+6qPdrYpktOpx5goB0cUvJYwyttgZS/gtXjNTNa2Exa5XDBEgqqqw==", "2ba4bb82-486b-4e49-afa0-0255584da5e4" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "PasswordHash", "SecurityStamp" },
                values: new object[] { "badfe6a7-580c-434a-94a4-c58ebe6325c3", new DateTime(2025, 4, 28, 20, 44, 29, 712, DateTimeKind.Utc).AddTicks(180), "AQAAAAIAAYagAAAAEJCdlBahqQ0vgJ1QBgLoM6/kyqS9IEu41Mobq0wulZycV1UZGQPcdT4npYRV9HzpqA==", "573a13e4-49c7-4228-a298-de076092aecd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3866b235-bf74-459b-8ade-fd89bd470266", new DateTime(2025, 4, 28, 20, 44, 29, 712, DateTimeKind.Utc).AddTicks(147), "AQAAAAIAAYagAAAAEFhQmKNMEZbIMq3g6LYQKHJnUqsmvbylGLaNgcZgQ0uOJonVQlkRQccqufmCyZpjVQ==", "cd79a3ba-5a55-46d2-ac23-e17f5eab721c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCad",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "IsAtivo",
                table: "Clinicas");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Clinicas");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f9a4cd6-dbc1-49ba-8d04-724360a78d43", new DateTime(2025, 4, 2, 4, 43, 14, 161, DateTimeKind.Utc).AddTicks(6239), "AQAAAAIAAYagAAAAECmVohQfbkLWx5Abw48BMZQ8eqjzpnSHA6L8lOt1hZ5RiNlnw30HHHnhlqBIIcLCOQ==", "619d3a47-20bc-4432-8863-c845ff604714" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "PasswordHash", "SecurityStamp" },
                values: new object[] { "943c050a-956e-41cd-85aa-823908b618ea", new DateTime(2025, 4, 2, 4, 43, 14, 161, DateTimeKind.Utc).AddTicks(6211), "AQAAAAIAAYagAAAAEOTuXvmxphht1tXDpgrSf+zcESQa6q1BhWO83zLgYmUZCpfpfkBy3tRV5PObr5DBFg==", "99d0ed61-b105-4d2f-bc4f-e44fea8fe322" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8f5b4b04-40df-4024-a43f-d50de225809f", new DateTime(2025, 4, 2, 4, 43, 14, 161, DateTimeKind.Utc).AddTicks(6183), "AQAAAAIAAYagAAAAEDLQHK26PK6dUhjVCsH6hu8IdabLk+t7Q+C86+br5ITe+s2M7+XJ0Syt79ZFOvkh9g==", "3cd9168e-8cc3-4fa8-9d59-aa79898e16ee" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5a8ede89-5305-4eda-aeae-3b54fda16c6d", new DateTime(2025, 4, 2, 4, 43, 14, 161, DateTimeKind.Utc).AddTicks(6153), "AQAAAAIAAYagAAAAELavlJCsGqSFtUvchdht5N5BupQQUzuNik2K3kT6n3Yy7ty5dLEiYOS9GCZoFkLlCw==", "6bf8767e-8dd6-40e2-b0d6-a3ae5d087be0" });
        }
    }
}
