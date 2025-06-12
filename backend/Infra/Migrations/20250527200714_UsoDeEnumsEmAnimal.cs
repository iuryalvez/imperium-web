using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class UsoDeEnumsEmAnimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1e60ac77-56a6-4824-80fd-ddf359442ff6", new DateTime(2025, 5, 27, 20, 7, 12, 877, DateTimeKind.Utc).AddTicks(6941), new DateTime(2025, 5, 27, 20, 7, 12, 877, DateTimeKind.Utc).AddTicks(6950), "AQAAAAIAAYagAAAAEGuVcWirLP700urLM0t7Hc7YD4gX23IWm69DdHguVWvMcbEkAB7+zdpY5kGqbaCXkA==", "cac9bb79-e177-4e09-a8d5-d7f1dc0ba83d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3480a57d-ba75-4787-89f8-56423dbe0ad9", new DateTime(2025, 5, 27, 20, 7, 12, 877, DateTimeKind.Utc).AddTicks(6888), new DateTime(2025, 5, 27, 20, 7, 12, 877, DateTimeKind.Utc).AddTicks(6903), "AQAAAAIAAYagAAAAENH9BY2cSrcPuALEKG/QfBhdC9rZsplhYkOwUxasM6APeTMXxdejEhElkvRoYlBuSg==", "a6493c63-24e7-461a-a9d7-34bb24930bd7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "66e5e916-7f5f-40f5-8e64-3927139e48bf", new DateTime(2025, 5, 27, 20, 7, 12, 877, DateTimeKind.Utc).AddTicks(6840), new DateTime(2025, 5, 27, 20, 7, 12, 877, DateTimeKind.Utc).AddTicks(6855), "AQAAAAIAAYagAAAAEEaGIIpXZqp5OOl+tc+1st+y1X7M0sIowRdRMR+73RMouhAhpYDVpA7cM1zA1r5ZTA==", "9366ae67-00a2-467a-b8fb-e50908a5545d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6193233d-06fc-466b-987e-aeb8422803ce", new DateTime(2025, 5, 27, 20, 7, 12, 877, DateTimeKind.Utc).AddTicks(6787), new DateTime(2025, 5, 27, 20, 7, 12, 877, DateTimeKind.Utc).AddTicks(6801), "AQAAAAIAAYagAAAAEFKvevtUHEbkjXUIF3EjZ5I/vi0XKLwbG6E8NEZwLHBGad2XOLKwZ7ECey28gi2DbQ==", "9fb257f7-5e32-43ee-8e9e-a20aa2ee2762" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "93945232-625e-43b9-b504-0925fd592633", new DateTime(2025, 5, 24, 18, 53, 54, 872, DateTimeKind.Utc).AddTicks(9530), new DateTime(2025, 5, 24, 18, 53, 54, 872, DateTimeKind.Utc).AddTicks(9544), "AQAAAAIAAYagAAAAENhKIL7Iar4BdFNceluW7dFgf669SU0Ql9ZzoT23LkAnUh0/NB6PZDCrsbguU24WfQ==", "6388eebb-66da-4d59-b097-1bb5f9e596d2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b4f3d35d-c568-4ac6-b6aa-69bcfa17ef92", new DateTime(2025, 5, 24, 18, 53, 54, 872, DateTimeKind.Utc).AddTicks(9486), new DateTime(2025, 5, 24, 18, 53, 54, 872, DateTimeKind.Utc).AddTicks(9501), "AQAAAAIAAYagAAAAEKKg4aoymu0AgadnPvTAoPBt+q4SO3XSEErg8WrpSsXKADQ/ZBuiPYCVIHNU/Uh2Ug==", "7d16f7a6-536d-48ef-b052-743c548fffcd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "78d1b238-dbcf-4abb-a662-b36562e8b285", new DateTime(2025, 5, 24, 18, 53, 54, 872, DateTimeKind.Utc).AddTicks(9444), new DateTime(2025, 5, 24, 18, 53, 54, 872, DateTimeKind.Utc).AddTicks(9458), "AQAAAAIAAYagAAAAEMGe5ftPCYMPNd57eXheMxaqYKYXdkttxq/dDNhTS5RslgQ3iZW/oXv+cnqAyiLboA==", "c81911d1-1874-4434-82ca-8c620400e2b1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fd4fa1c6-c85b-4a55-b48b-274e9c7311e6", new DateTime(2025, 5, 24, 18, 53, 54, 872, DateTimeKind.Utc).AddTicks(9402), new DateTime(2025, 5, 24, 18, 53, 54, 872, DateTimeKind.Utc).AddTicks(9411), "AQAAAAIAAYagAAAAEMEY1jj0QsHjnZVTB2mROimLBIwXH7sPaXIFJK2+yIOgTIb3Tz7vIe+xQygBy8sGWA==", "4ec6faa4-d57b-451a-816b-5aa5b8bd4ea8" });
        }
    }
}
