using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class NovosEnumsECalculoDeIdadeEFaseDaVidaAnimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FaseVida",
                table: "Animais");

            migrationBuilder.DropColumn(
                name: "Idade",
                table: "Animais");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -4,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "12dfd203-6ec1-4afb-ae88-0f102090d250", new DateTime(2025, 5, 27, 20, 28, 18, 442, DateTimeKind.Utc).AddTicks(7814), new DateTime(2025, 5, 27, 20, 28, 18, 442, DateTimeKind.Utc).AddTicks(7829), "AQAAAAIAAYagAAAAEHODsyrE5tagrkc9XijAiuTmO/+LI5e0bhyApgB38PxkUpZMd+zOL4fi9Ycyb24nPw==", "83f70a95-76b4-4546-b720-4102f2e36ffb" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -3,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b9d43fdd-5fda-4ef1-8cff-d871b24d0079", new DateTime(2025, 5, 27, 20, 28, 18, 442, DateTimeKind.Utc).AddTicks(7771), new DateTime(2025, 5, 27, 20, 28, 18, 442, DateTimeKind.Utc).AddTicks(7785), "AQAAAAIAAYagAAAAEKZdo4Osbs7/GYtd8/2ruLWK4TIZUBahdPAqrfBzZHv4Ve5rZ1w6ewdOJr0NcepPQA==", "a16887c6-91ec-41c7-b633-79af30a5f3c6" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -2,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8dae64c0-4c9a-494c-af73-84fe12baef26", new DateTime(2025, 5, 27, 20, 28, 18, 442, DateTimeKind.Utc).AddTicks(7729), new DateTime(2025, 5, 27, 20, 28, 18, 442, DateTimeKind.Utc).AddTicks(7743), "AQAAAAIAAYagAAAAEOPyp0IS34kxAfhmJj+RwT4t67AQzxeaLBzIfHUJxcTLp9+LELckdWQVJoAw9Y70MQ==", "1980d630-5a25-4d9e-8d7f-9799f4095602" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: -1,
                columns: new[] { "ConcurrencyStamp", "DataCad", "DataUltimoAcesso", "PasswordHash", "SecurityStamp" },
                values: new object[] { "205b2f4e-08bf-4e66-a37e-3019ac3cf066", new DateTime(2025, 5, 27, 20, 28, 18, 442, DateTimeKind.Utc).AddTicks(7686), new DateTime(2025, 5, 27, 20, 28, 18, 442, DateTimeKind.Utc).AddTicks(7696), "AQAAAAIAAYagAAAAEN3khUMdbBHJqk5c1R+L2qD6owjhLPNWyGHn+qWXgJeCOPeu2FIBckY79qn98elTBQ==", "b26a9e15-949d-48e1-ae71-ff3f31e57a0b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FaseVida",
                table: "Animais",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Idade",
                table: "Animais",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
    }
}
