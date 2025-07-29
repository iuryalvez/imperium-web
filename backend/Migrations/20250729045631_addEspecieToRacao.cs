using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class addEspecieToRacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Especie",
                table: "Racoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Especie",
                table: "Racoes");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b5d29e94-239d-4267-a9a1-f121394c4232", new DateTime(2025, 7, 21, 5, 17, 2, 485, DateTimeKind.Utc).AddTicks(1911), new DateTime(2025, 7, 21, 5, 17, 2, 485, DateTimeKind.Utc).AddTicks(1926), "AQAAAAIAAYagAAAAEKV0Xr505ydKgJt2CW+dI0nzKkb6tJAdiPK4bSpxlDN/vtc19Q8ffUaNHwN55gYNaA==", "294da1be-0edb-43b4-a69a-a89aed069b49" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "162a78d1-d96c-4f63-a905-72aa09a5c67f", new DateTime(2025, 7, 21, 5, 17, 2, 485, DateTimeKind.Utc).AddTicks(1872), new DateTime(2025, 7, 21, 5, 17, 2, 485, DateTimeKind.Utc).AddTicks(1887), "AQAAAAIAAYagAAAAEL5lfTEepY/jPjxTt1aVGC7Wf/znzQTEGoMMNmq02raKhyQAgruTq5guy8OwJ0v1Xg==", "c5474512-5bd5-4220-9ce8-e4e755195ef6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6d80c6eb-741a-4c59-8fdd-7c69584a1b90", new DateTime(2025, 7, 21, 5, 17, 2, 485, DateTimeKind.Utc).AddTicks(1833), new DateTime(2025, 7, 21, 5, 17, 2, 485, DateTimeKind.Utc).AddTicks(1848), "AQAAAAIAAYagAAAAEMJuoO/04u7JstPWQNrNhie55ciliXVdFOgkwMyqVy8XZL6gTt+sXoFDr2jNz3stAg==", "fdc305cb-a725-4d77-87aa-7247eb80b954" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cabfd487-cb66-4b42-8858-a244ae45657d", new DateTime(2025, 7, 21, 5, 17, 2, 485, DateTimeKind.Utc).AddTicks(1758), new DateTime(2025, 7, 21, 5, 17, 2, 485, DateTimeKind.Utc).AddTicks(1772), "AQAAAAIAAYagAAAAEIQPxDB9ISjr+LdPcKyBuBqy8gz97Jk2Fxe/BVnAulbAjcD2lGrfBp6XMiRVCIlmjw==", "1601d5c6-2663-4e5b-af1a-0dbb68ec8eda" });
        }
    }
}
