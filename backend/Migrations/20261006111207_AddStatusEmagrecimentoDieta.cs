using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusEmagrecimentoDieta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "DataConclusao",
                table: "Dietas",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataInicio",
                table: "Dietas",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NivelRestricao",
                table: "Dietas",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Dietas",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "RegistrosPesoDieta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DietaId = table.Column<int>(type: "integer", nullable: false),
                    Semana = table.Column<int>(type: "integer", nullable: false),
                    DataRegistro = table.Column<DateOnly>(type: "date", nullable: false),
                    PesoReal = table.Column<double>(type: "double precision", nullable: false),
                    Observacao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosPesoDieta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosPesoDieta_Dietas_DietaId",
                        column: x => x.DietaId,
                        principalTable: "Dietas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPesoDieta_DietaId_Semana",
                table: "RegistrosPesoDieta",
                columns: new[] { "DietaId", "Semana" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosPesoDieta");

            migrationBuilder.DropColumn(
                name: "DataConclusao",
                table: "Dietas");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Dietas");

            migrationBuilder.DropColumn(
                name: "NivelRestricao",
                table: "Dietas");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Dietas");
        }
    }
}
