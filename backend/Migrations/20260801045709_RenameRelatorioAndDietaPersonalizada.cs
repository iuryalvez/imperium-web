using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class RenameRelatorioAndDietaPersonalizada : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DietasPersonalizadas");

            migrationBuilder.DropTable(
                name: "Relatorios");

            migrationBuilder.CreateTable(
                name: "Avaliacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsultaId = table.Column<int>(type: "integer", nullable: false),
                    AcompanhamentoId = table.Column<int>(type: "integer", nullable: false),
                    UsuarioResponsavelId = table.Column<int>(type: "integer", nullable: true),
                    Idade = table.Column<int>(type: "integer", nullable: false),
                    Peso = table.Column<double>(type: "double precision", nullable: false),
                    PesoIdeal = table.Column<double>(type: "double precision", nullable: true),
                    CondicaoCorporal = table.Column<int>(type: "integer", nullable: false),
                    GrauAtividade = table.Column<int>(type: "integer", nullable: false),
                    IsGestante = table.Column<bool>(type: "boolean", nullable: false),
                    IsLactanteNumeroFilhotes = table.Column<int>(type: "integer", nullable: false),
                    SemanaLactacao = table.Column<int>(type: "integer", nullable: false),
                    SemanaGestacao = table.Column<int>(type: "integer", nullable: false),
                    RacaoId = table.Column<int>(type: "integer", nullable: false),
                    TipoDieta = table.Column<int>(type: "integer", nullable: false),
                    DescricaoDieta = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avaliacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_AcompanhamentosInds_AcompanhamentoId",
                        column: x => x.AcompanhamentoId,
                        principalTable: "AcompanhamentosInds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_Consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "Consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Avaliacoes_Racoes_RacaoId",
                        column: x => x.RacaoId,
                        principalTable: "Racoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dietas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RacaoId = table.Column<int>(type: "integer", nullable: false),
                    AvaliacaoId = table.Column<int>(type: "integer", nullable: false),
                    TipoDieta = table.Column<int>(type: "integer", nullable: false),
                    FatorK = table.Column<double>(type: "double precision", nullable: false),
                    Expoente = table.Column<double>(type: "double precision", nullable: false),
                    PerfilNutricional = table.Column<string>(type: "text", nullable: false),
                    IsCustomizado = table.Column<bool>(type: "boolean", nullable: false),
                    MultiplicadorEnergeticoCustom = table.Column<double>(type: "double precision", nullable: true),
                    PotenciaCustom = table.Column<double>(type: "double precision", nullable: true),
                    QtdAlimento = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaMetabolizavel = table.Column<double>(type: "double precision", nullable: true),
                    PesoAtual = table.Column<double>(type: "double precision", nullable: true),
                    PesoObjetivo = table.Column<double>(type: "double precision", nullable: true),
                    PesoIdeal = table.Column<double>(type: "double precision", nullable: true),
                    DataCriacao = table.Column<DateOnly>(type: "date", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    DuracaoEstimada = table.Column<int>(type: "integer", nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    FaseVida = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dietas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dietas_Avaliacoes_AvaliacaoId",
                        column: x => x.AvaliacaoId,
                        principalTable: "Avaliacoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Dietas_Racoes_RacaoId",
                        column: x => x.RacaoId,
                        principalTable: "Racoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_AcompanhamentoId",
                table: "Avaliacoes",
                column: "AcompanhamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_ConsultaId",
                table: "Avaliacoes",
                column: "ConsultaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Avaliacoes_RacaoId",
                table: "Avaliacoes",
                column: "RacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Dietas_AvaliacaoId",
                table: "Dietas",
                column: "AvaliacaoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dietas_RacaoId",
                table: "Dietas",
                column: "RacaoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dietas");

            migrationBuilder.DropTable(
                name: "Avaliacoes");

            migrationBuilder.CreateTable(
                name: "Relatorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AcompanhamentoId = table.Column<int>(type: "integer", nullable: false),
                    ConsultaId = table.Column<int>(type: "integer", nullable: false),
                    RacaoId = table.Column<int>(type: "integer", nullable: false),
                    CondicaoCorporal = table.Column<int>(type: "integer", nullable: false),
                    DescricaoDieta = table.Column<string>(type: "text", nullable: true),
                    GrauAtividade = table.Column<int>(type: "integer", nullable: false),
                    Idade = table.Column<int>(type: "integer", nullable: false),
                    IsGestante = table.Column<bool>(type: "boolean", nullable: false),
                    IsLactanteNumeroFilhotes = table.Column<int>(type: "integer", nullable: false),
                    Peso = table.Column<double>(type: "double precision", nullable: false),
                    PesoIdeal = table.Column<double>(type: "double precision", nullable: true),
                    SemanaGestacao = table.Column<int>(type: "integer", nullable: false),
                    SemanaLactacao = table.Column<int>(type: "integer", nullable: false),
                    TipoDieta = table.Column<int>(type: "integer", nullable: false),
                    UsuarioResponsavelId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relatorios_AcompanhamentosInds_AcompanhamentoId",
                        column: x => x.AcompanhamentoId,
                        principalTable: "AcompanhamentosInds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Relatorios_Consultas_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "Consultas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Relatorios_Racoes_RacaoId",
                        column: x => x.RacaoId,
                        principalTable: "Racoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DietasPersonalizadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RacaoId = table.Column<int>(type: "integer", nullable: false),
                    RelatorioId = table.Column<int>(type: "integer", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    DataCriacao = table.Column<DateOnly>(type: "date", nullable: false),
                    DuracaoEstimada = table.Column<int>(type: "integer", nullable: true),
                    EnergiaMetabolizavel = table.Column<double>(type: "double precision", nullable: true),
                    Expoente = table.Column<double>(type: "double precision", nullable: false),
                    FaseVida = table.Column<int>(type: "integer", nullable: true),
                    FatorK = table.Column<double>(type: "double precision", nullable: false),
                    IsCustomizado = table.Column<bool>(type: "boolean", nullable: false),
                    MultiplicadorEnergeticoCustom = table.Column<double>(type: "double precision", nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    PerfilNutricional = table.Column<string>(type: "text", nullable: false),
                    PesoAtual = table.Column<double>(type: "double precision", nullable: true),
                    PesoIdeal = table.Column<double>(type: "double precision", nullable: true),
                    PesoObjetivo = table.Column<double>(type: "double precision", nullable: true),
                    PotenciaCustom = table.Column<double>(type: "double precision", nullable: true),
                    QtdAlimento = table.Column<double>(type: "double precision", nullable: true),
                    TipoDieta = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietasPersonalizadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DietasPersonalizadas_Racoes_RacaoId",
                        column: x => x.RacaoId,
                        principalTable: "Racoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DietasPersonalizadas_Relatorios_RelatorioId",
                        column: x => x.RelatorioId,
                        principalTable: "Relatorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DietasPersonalizadas_RacaoId",
                table: "DietasPersonalizadas",
                column: "RacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DietasPersonalizadas_RelatorioId",
                table: "DietasPersonalizadas",
                column: "RelatorioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_AcompanhamentoId",
                table: "Relatorios",
                column: "AcompanhamentoId");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_ConsultaId",
                table: "Relatorios",
                column: "ConsultaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_RacaoId",
                table: "Relatorios",
                column: "RacaoId");
        }
    }
}
