using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataUltimoAcesso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Formacao = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    Telefone = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Whatsapp = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeClinica = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    NomeFantasia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Whatsapp = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Instagram = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtiva = table.Column<bool>(type: "boolean", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    Cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    Logradouro = table.Column<string>(type: "text", nullable: true),
                    Complemento = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tutores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Whatsapp = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    Logradouro = table.Column<string>(type: "text", nullable: true),
                    Complemento = table.Column<string>(type: "text", nullable: true),
                    Bairro = table.Column<string>(type: "text", nullable: true),
                    Cidade = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: true),
                    Uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Racas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Porte = table.Column<int>(type: "integer", nullable: true),
                    Especie = table.Column<int>(type: "integer", nullable: false),
                    PesoMinimoReferenciaFemea = table.Column<double>(type: "double precision", nullable: true),
                    PesoMaximoReferenciaFemea = table.Column<double>(type: "double precision", nullable: true),
                    PesoMinimoReferenciaMacho = table.Column<double>(type: "double precision", nullable: true),
                    PesoMaximoReferenciaMacho = table.Column<double>(type: "double precision", nullable: true),
                    Referencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Racas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Racas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Racoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    Especie = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    IsPorcentagem = table.Column<bool>(type: "boolean", nullable: false),
                    Umidade = table.Column<double>(type: "double precision", nullable: false),
                    ProteinaBruta = table.Column<double>(type: "double precision", nullable: false),
                    ExtratoEtereo = table.Column<double>(type: "double precision", nullable: false),
                    MateriaMineral = table.Column<double>(type: "double precision", nullable: false),
                    FibraBruta = table.Column<double>(type: "double precision", nullable: false),
                    MateriaSeca = table.Column<double>(type: "double precision", nullable: true),
                    ExtrativoNaoNitrogenado = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaBruta = table.Column<double>(type: "double precision", nullable: true),
                    FibraDieteticaTotal = table.Column<double>(type: "double precision", nullable: true),
                    CoeficienteDigestibilidadeEnergia = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaDigestivel = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaMetabolizavel = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaProteina = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaExtratoEtereo = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaExtrativoNaoNitrogenado = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaTotalEstimativa = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Racoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Racoes_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosPerfisClinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    Perfis = table.Column<List<string>>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosPerfisClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosPerfisClinicas_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosPerfisClinicas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TutoresClinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TutorId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    DataAssociacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataDesativacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutoresClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutoresClinicas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TutoresClinicas_Tutores_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Animais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Especie = table.Column<int>(type: "integer", nullable: false),
                    RacaId = table.Column<int>(type: "integer", nullable: true),
                    Nascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataModificado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Sexo = table.Column<int>(type: "integer", nullable: false),
                    Peso = table.Column<double>(type: "double precision", nullable: true),
                    PesoMinimo = table.Column<double>(type: "double precision", nullable: true),
                    PesoMaximo = table.Column<double>(type: "double precision", nullable: true),
                    PesoIdeal = table.Column<double>(type: "double precision", nullable: true),
                    CondicaoCorporal = table.Column<int>(type: "integer", nullable: true),
                    GrauAtividade = table.Column<int>(type: "integer", nullable: true),
                    IsCastrado = table.Column<bool>(type: "boolean", nullable: true),
                    IsGestante = table.Column<bool>(type: "boolean", nullable: true),
                    IsLactanteNumeroFilhotes = table.Column<int>(type: "integer", nullable: true),
                    SemanaLactacao = table.Column<int>(type: "integer", nullable: true),
                    SemanaGestacao = table.Column<int>(type: "integer", nullable: true),
                    PerfilCalculado = table.Column<int>(type: "integer", nullable: false),
                    NecessidadeEnergeticaDiaria = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animais_Racas_RacaId",
                        column: x => x.RacaId,
                        principalTable: "Racas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AcompanhamentosInds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnimalId = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    DataFim = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcompanhamentosInds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcompanhamentosInds_Animais_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimaisClinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnimalId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    DataAssociacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataDesativacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimaisClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimaisClinicas_Animais_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AnimaisClinicas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Consultas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    TutorId = table.Column<int>(type: "integer", nullable: false),
                    TutorIsCadastrado = table.Column<bool>(type: "boolean", nullable: false),
                    AnimalId = table.Column<int>(type: "integer", nullable: false),
                    DataConsulta = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Consultas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Consultas_Animais_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Consultas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Consultas_Tutores_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TutoresAnimais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TutorId = table.Column<int>(type: "integer", nullable: false),
                    AnimalId = table.Column<int>(type: "integer", nullable: false),
                    DataAssociacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutoresAnimais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutoresAnimais_Animais_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TutoresAnimais_Tutores_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Relatorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsultaId = table.Column<int>(type: "integer", nullable: false),
                    AcompanhamentoIndividualizadoId = table.Column<int>(type: "integer", nullable: false),
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
                    DescricaoDieta = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relatorios_AcompanhamentosInds_AcompanhamentoIndividualizad~",
                        column: x => x.AcompanhamentoIndividualizadoId,
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
                    TipoDieta = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_DietasPersonalizadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DietasPersonalizadas_Racoes_RacaoId",
                        column: x => x.RacaoId,
                        principalTable: "Racoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DietasPersonalizadas_Relatorios_RelatorioId",
                        column: x => x.RelatorioId,
                        principalTable: "Relatorios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Cpf", "DataCadastro", "DataUltimoAcesso", "Email", "EmailConfirmed", "Formacao", "IsAdmin", "IsAtivo", "LockoutEnabled", "LockoutEnd", "Nome", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Telefone", "TwoFactorEnabled", "UserName", "Whatsapp" },
                values: new object[,]
                {
                    { -4, 0, "1f2196a8-dd33-4905-ba98-3a0b777949de", "01234567893", new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(575), new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(589), "ceron@gsmail.com", true, "Administrador do Sistema", true, true, false, null, "Admin Ceron", "CERON@GSMAIL.COM", "ADMIN CERON", "AQAAAAIAAYagAAAAEIvOAdAfHTC0jQaMqgnNtAB3YmgEJeqqp1B1/E98hR2XpgFVmwsyo0pb+aR4Lvtwbg==", null, false, "08aca275-3ef2-448e-ad2b-b45ca99fd870", "62940302010", false, "ceron", "62940302010" },
                    { -3, 0, "85b86f42-c722-4e41-ae35-c397e40304c2", "01234567892", new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(532), new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(542), "hugo@gsmail.com", true, "Desenvolvedor do Sistema", true, true, false, null, "Admin Hugo", "HUGO@GSMAIL.COM", "ADMIN HUGO", "AQAAAAIAAYagAAAAEABIcWyn4BjvHJYHlDzvfrHOAWVZPhKM/Y92mcmjm/qwYIZWJTk/waXPBzre3zvkzw==", null, false, "752242a0-78cb-4197-afa8-a42d41aef7f4", "62910203040", false, "hugo", "62910203040" },
                    { -2, 0, "c814c888-4fe1-448a-9fde-f448b4a5ed5b", "01234567891", new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(489), new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(504), "rafael@gsmail.com", true, "Desenvolvedor do Sistema", true, true, false, null, "Admin Rafael", "RAFAEL@GSMAIL.COM", "ADMIN RAFAEL", "AQAAAAIAAYagAAAAEEggTRzQovc7A/iOMk5C/jtMP2PfJYyClJRbx0M0OgX3RK7iQ5ArVDiNCM/kKmUKQQ==", null, false, "16f00e54-a4c3-4762-8551-5ce6f4ccb4e5", "62912345678", false, "admin.rafael", "62912345678" },
                    { -1, 0, "e37f6333-700a-4de2-aca3-f439208ea316", "01234567890", new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(443), new DateTime(2025, 9, 15, 1, 21, 34, 360, DateTimeKind.Utc).AddTicks(461), "iury@gsmail.com", true, "Desenvolvedor do Sistema", true, true, false, null, "Admin Iury", "IURY@GSMAIL.COM", "IURY", "AQAAAAIAAYagAAAAEIiOThbrE3wdsdLHfEIndNjnrJNsEWRCGnLPnFuFg4dhO3ZvlVXMET27OM9zkNZSIw==", null, false, "5e416c1f-6880-45e2-90d8-da95dddf8665", "62987654321", false, "admin.iury", "62987654321" }
                });

            migrationBuilder.InsertData(
                table: "Racas",
                columns: new[] { "Id", "ClinicaId", "Especie", "Nome", "PesoMaximoReferenciaFemea", "PesoMaximoReferenciaMacho", "PesoMinimoReferenciaFemea", "PesoMinimoReferenciaMacho", "Porte", "Referencia" },
                values: new object[,]
                {
                    { 1, null, 1, "Airedale Terrier", 19.0, 30.0, 19.0, 30.0, 2, "Mark Morris Institute" },
                    { 2, null, 1, "Akita Inu", 34.0, 46.0, 34.0, 46.0, 3, "Mark Morris Institute" },
                    { 3, null, 1, "Alaskan Malamute", 34.0, 57.0, 34.0, 57.0, 3, "Mark Morris Institute" },
                    { 4, null, 1, "Basenji", 10.0, 11.0, 10.0, 11.0, 2, "Mark Morris Institute" },
                    { 5, null, 1, "Basset Hound", 18.0, 27.0, 18.0, 27.0, 2, "Mark Morris Institute" },
                    { 6, null, 1, "Beagle", 12.0, 14.0, 12.0, 14.0, 2, "Mark Morris Institute" },
                    { 7, null, 1, "Bearded Collie", 18.0, 27.0, 18.0, 27.0, 2, "Mark Morris Institute" },
                    { 8, null, 1, "Beauceron", 30.0, 38.0, 30.0, 38.0, 2, "Mark Morris Institute" },
                    { 9, null, 1, "Bichon Frisé", 4.5, 5.5, 4.5, 5.5, 1, "Mark Morris Institute" },
                    { 10, null, 1, "Bichon Maltês", 1.7999999523162842, 2.7000000476837158, 1.7999999523162842, 2.7000000476837158, 1, "Mark Morris Institute" },
                    { 11, null, 1, "Bloodhound", 45.5, 50.0, 36.0, 41.0, 3, "Mark Morris Institute" },
                    { 12, null, 1, "Bobtail", 25.0, 30.0, 25.0, 30.0, 2, "Mark Morris Institute" },
                    { 13, null, 1, "Boiadeiro de Berna", 40.0, 50.0, 40.0, 50.0, 3, "Mark Morris Institute" },
                    { 14, null, 1, "Boiadeiro de Flandres", 35.0, 40.0, 27.0, 35.0, 3, "Mark Morris Institute" },
                    { 15, null, 1, "Border Collie", 13.5, 20.5, 13.5, 20.5, 2, "Mark Morris Institute" },
                    { 16, null, 1, "Border Terrier", 6.4000000953674316, 6.8000001907348633, 5.0, 6.8000001907348633, 1, "Mark Morris Institute" },
                    { 17, null, 1, "Boston Terrier", 9.0, 9.0, 9.0, 9.0, 1, "Mark Morris Institute" },
                    { 18, null, 1, "Boxer", 24.0, 32.0, 24.0, 32.0, 2, "Mark Morris Institute" },
                    { 19, null, 1, "Braco Francês", 22.700000762939453, 27.0, 22.700000762939453, 27.0, 2, "Mark Morris Institute" },
                    { 20, null, 1, "Braco Húngaro / Vizsla", 20.0, 30.0, 20.0, 30.0, 2, "Mark Morris Institute" },
                    { 21, null, 1, "Braco de Saint Germain", 18.0, 26.0, 18.0, 26.0, 2, "Mark Morris Institute" },
                    { 22, null, 1, "Bull Terrier", 23.5, 28.0, 23.5, 28.0, 2, "Mark Morris Institute" },
                    { 23, null, 1, "Bull Terrier Miniatura", 4.5, 18.0, 4.5, 18.0, 1, "Mark Morris Institute" },
                    { 24, null, 1, "Bulldog", 23.0, 25.0, 18.0, 23.0, 2, "Mark Morris Institute" },
                    { 25, null, 1, "Bulldog Francês", 8.0, 13.0, 8.0, 13.0, 1, "Mark Morris Institute" },
                    { 26, null, 1, "Bullmastiff", 54.0, 65.0, 54.0, 65.0, 3, "Mark Morris Institute" },
                    { 27, null, 1, "Cairn Terrier", 6.0, 7.5, 6.0, 7.5, 1, "Mark Morris Institute" },
                    { 28, null, 1, "Carlino", 6.5, 8.0, 6.5, 8.0, 1, "Mark Morris Institute" },
                    { 29, null, 1, "Cavalier King Charles Spaniel", 5.0, 8.0, 5.0, 8.0, 1, "Mark Morris Institute" },
                    { 30, null, 1, "Chesapeake Bay Retriever", 32.0, 36.0, 25.0, 29.5, 2, "Mark Morris Institute" },
                    { 31, null, 1, "Chihuahua", 2.7000000476837158, 2.7000000476837158, 1.7999999523162842, 1.7999999523162842, 1, "Mark Morris Institute" },
                    { 32, null, 1, "Chow Chow", 20.0, 30.0, 20.0, 30.0, 2, "Mark Morris Institute" },
                    { 33, null, 1, "Clumber Spaniel", 32.0, 38.0, 25.0, 32.0, 2, "Mark Morris Institute" },
                    { 34, null, 1, "Cocker Spaniel Americano", 11.0, 12.5, 11.0, 12.5, 2, "Mark Morris Institute" },
                    { 35, null, 1, "Cocker Spaniel Inglês", 14.5, 15.5, 12.0, 12.5, 2, "Mark Morris Institute" },
                    { 36, null, 1, "Collie", 30.0, 34.0, 20.0, 25.0, 2, "Mark Morris Institute" },
                    { 37, null, 1, "Coton de Tulear", 5.0, 6.8000001907348633, 5.0, 6.8000001907348633, 1, "Mark Morris Institute" },
                    { 38, null, 1, "Cão da Montanha dos Pirineus", 45.0, 55.0, 38.5, 45.5, 3, "Mark Morris Institute" },
                    { 39, null, 1, "Cão de Água Americano", 18.0, 20.5, 11.0, 13.5, 2, "Mark Morris Institute" },
                    { 40, null, 1, "Cão de Água Português", 23.0, 27.0, 16.0, 19.0, 2, "Mark Morris Institute" },
                    { 41, null, 1, "Dachshund Miniatura", 4.5, 4.5, 4.5, 4.5, 1, "Mark Morris Institute" },
                    { 42, null, 1, "Dandie Dinmont Terrier", 8.0, 11.0, 8.0, 11.0, 1, "Mark Morris Institute" },
                    { 43, null, 1, "Doberman", 29.0, 39.0, 29.0, 39.0, 2, "Mark Morris Institute" },
                    { 44, null, 1, "Dogue de Bordeuax", 54.0, 65.0, 54.0, 65.0, 3, "Mark Morris Institute" },
                    { 45, null, 1, "Dogue de Bordeaux (grande)", 60.0, 65.0, 45.0, 45.0, 3, "Mark Morris Institute" },
                    { 46, null, 1, "Dogue de Bordeaux (médio)", 40.0, 45.0, 35.0, 38.0, 3, "Mark Morris Institute" },
                    { 47, null, 1, "Dálmata", 22.700000762939453, 27.0, 22.700000762939453, 27.0, 2, "Mark Morris Institute" },
                    { 48, null, 1, "Epagneul Breton", 13.5, 18.0, 13.5, 18.0, 2, "Mark Morris Institute" },
                    { 49, null, 1, "Fila Brasileiro", 40.5, 45.0, 40.5, 45.0, 3, "Mark Morris Institute" },
                    { 50, null, 1, "Fox Terrier", 7.6999998092651367, 8.1999998092651367, 6.8000001907348633, 7.0, 1, "Mark Morris Institute" },
                    { 51, null, 1, "Foxhound Inglês", 29.5, 32.0, 29.5, 32.0, 2, "Mark Morris Institute" },
                    { 52, null, 1, "Galgo Afegão", 23.0, 27.0, 23.0, 27.0, 2, "Mark Morris Institute" },
                    { 53, null, 1, "Galgo Espanhol", 27.0, 27.0, 27.0, 27.0, 2, "Mark Morris Institute" },
                    { 54, null, 1, "Galgo Inglês", 29.5, 32.0, 27.0, 29.5, 2, "Mark Morris Institute" },
                    { 55, null, 1, "Galgo Russo (Borzoi)", 40.0, 48.0, 35.0, 34.0, 3, "Mark Morris Institute" },
                    { 56, null, 1, "Golden Retriever", 29.5, 34.0, 25.0, 29.5, 2, "Mark Morris Institute" },
                    { 57, null, 1, "Gordon Setter", 32.0, 36.0, 20.5, 25.0, 2, "Mark Morris Institute" },
                    { 58, null, 1, "Grand Danois", 55.0, 80.0, 55.0, 80.0, 3, "Mark Morris Institute" },
                    { 59, null, 1, "Griffon Vendeen", 11.5, 16.0, 11.5, 16.0, 2, "Mark Morris Institute" },
                    { 60, null, 1, "Griffon de Bruxelas", 2.2000000476837158, 5.5, 2.2000000476837158, 5.5, 1, "Mark Morris Institute" },
                    { 61, null, 1, "Griffon de Pêlo Duro", 23.0, 27.0, 23.0, 27.0, 2, "Mark Morris Institute" },
                    { 62, null, 1, "Husky Siberiano", 22.5, 27.0, 16.0, 20.0, 2, "Mark Morris Institute" },
                    { 63, null, 1, "Komondor", 50.0, 68.0, 36.0, 45.0, 3, "Mark Morris Institute" },
                    { 64, null, 1, "Kuvasz", 41.0, 52.0, 32.0, 45.0, 3, "Mark Morris Institute" },
                    { 65, null, 1, "Labrador Retriever", 32.0, 36.0, 25.0, 29.5, 2, "Mark Morris Institute" },
                    { 66, null, 1, "Leonberger", 36.299999237060547, 68.0, 36.299999237060547, 68.0, 3, "Mark Morris Institute" },
                    { 67, null, 1, "Lhasa Apso", 6.4000000953674316, 8.0, 6.4000000953674316, 8.0, 1, "Mark Morris Institute" },
                    { 68, null, 1, "Lulu da Pomerânia (Spitz anão)", 1.5, 3.2000000476837158, 1.5, 3.2000000476837158, 1, "Mark Morris Institute" },
                    { 69, null, 1, "Mastim Espanhol", 50.0, 60.0, 50.0, 60.0, 3, "Mark Morris Institute" },
                    { 70, null, 1, "Mastim Inglês", 75.0, 90.0, 75.0, 90.0, 3, "Mark Morris Institute" },
                    { 71, null, 1, "Mastim Napolitano", 50.0, 70.0, 50.0, 70.0, 3, "Mark Morris Institute" },
                    { 72, null, 1, "Mastim dos Pirineus", 55.0, 70.0, 55.0, 70.0, 3, "Mark Morris Institute" },
                    { 73, null, 1, "Pastor Alemão", 32.0, 43.0, 32.0, 43.0, 3, "Mark Morris Institute" },
                    { 74, null, 1, "Pastor Belga", 28.0, 30.0, 28.0, 30.0, 2, "Mark Morris Institute" },
                    { 75, null, 1, "Pastor Catalão", 30.0, 40.0, 20.0, 30.0, 2, "Mark Morris Institute" },
                    { 76, null, 1, "Pastor de Bergamasco", 32.0, 38.0, 26.0, 32.0, 3, "Mark Morris Institute" },
                    { 77, null, 1, "Pastor de Brie", 34.0, 34.0, 34.0, 34.0, 3, "Mark Morris Institute" },
                    { 78, null, 1, "Pequinois", 4.0, 6.5, 4.0, 3.5999999046325684, 1, "Mark Morris Institute" },
                    { 79, null, 1, "Perdigueiro Alemão", 20.0, 30.0, 20.0, 30.0, 2, "Mark Morris Institute" },
                    { 80, null, 1, "Pinscher", 4.0, 5.0, 4.0, 5.0, 1, "Mark Morris Institute" },
                    { 81, null, 1, "Pinscher Miniatura", 4.5, 4.5, 4.5, 4.5, 1, "Mark Morris Institute" },
                    { 82, null, 1, "Podengo Ibérico", 20.5, 23.0, 20.5, 23.0, 2, "Mark Morris Institute" },
                    { 83, null, 1, "Pointer", 29.5, 32.0, 20.0, 25.0, 2, "Mark Morris Institute" },
                    { 84, null, 1, "Ponteiro Alemão", 27.0, 32.0, 20.5, 25.0, 2, "Mark Morris Institute" },
                    { 85, null, 1, "Poodle Miniatura", 5.0, 5.0, 5.0, 5.0, 1, "Mark Morris Institute" },
                    { 86, null, 1, "Puli", 14.0, 15.0, 14.0, 13.0, 2, "Mark Morris Institute" },
                    { 87, null, 1, "Rafeiro Alentejano", 45.0, 45.0, 35.5, 40.0, 3, "Mark Morris Institute" },
                    { 88, null, 1, "Retrivier de Pêlo Liso", 34.0, 36.0, 25.0, 25.0, 3, "Mark Morris Institute" },
                    { 89, null, 1, "Rottweiler", 40.0, 50.0, 40.0, 50.0, 3, "Mark Morris Institute" },
                    { 90, null, 1, "Saluki", 13.0, 30.0, 13.0, 30.0, 2, "Mark Morris Institute" },
                    { 91, null, 1, "Samoyedo", 25.0, 30.0, 17.0, 20.0, 2, "Mark Morris Institute" },
                    { 92, null, 1, "Schnauzer Gigante", 30.0, 35.0, 30.0, 35.0, 2, "Mark Morris Institute" },
                    { 93, null, 1, "Schnauzer Miniatura", 5.5, 7.0, 5.5, 7.0, 1, "Mark Morris Institute" },
                    { 94, null, 1, "Schnauzer Médio", 15.0, 18.0, 15.0, 18.0, 2, "Mark Morris Institute" },
                    { 95, null, 1, "Scottish Terrier", 9.5, 10.0, 8.0, 8.5, 2, "Mark Morris Institute" },
                    { 96, null, 1, "Setter Inglês", 18.0, 25.0, 18.0, 25.0, 2, "Mark Morris Institute" },
                    { 97, null, 1, "Setter Irlandês", 27.200000762939453, 31.700000762939453, 27.200000762939453, 31.700000762939453, 2, "Mark Morris Institute" },
                    { 98, null, 1, "Shar-Pei", 18.0, 25.0, 18.0, 25.0, 2, "Mark Morris Institute" },
                    { 99, null, 1, "Shih Tzu", 4.0, 8.0, 4.0, 8.0, 1, "Mark Morris Institute" },
                    { 100, null, 1, "Silky Terrier Australiano", 3.5999999046325684, 4.5, 3.5999999046325684, 4.5, 1, "Mark Morris Institute" },
                    { 101, null, 1, "Skye Terrier", 11.5, 11.5, 11.5, 11.5, 2, "Mark Morris Institute" },
                    { 102, null, 1, "Spitz Alemão", 11.300000190734863, 11.300000190734863, 11.300000190734863, 11.300000190734863, 2, "Mark Morris Institute" },
                    { 103, null, 1, "Spitz Finlandês", 11.300000190734863, 16.0, 11.300000190734863, 16.0, 2, "Mark Morris Institute" },
                    { 104, null, 1, "Springer Spaniel Galês", 16.0, 20.0, 16.0, 20.0, 2, "Mark Morris Institute" },
                    { 105, null, 1, "Springer Spaniel Inglês", 18.0, 22.5, 18.0, 22.5, 2, "Mark Morris Institute" },
                    { 106, null, 1, "Staffordshire Bull Terrier", 15.0, 17.0, 11.0, 13.0, 2, "Mark Morris Institute" },
                    { 107, null, 1, "Sussex Spaniel", 16.0, 18.0, 16.0, 18.0, 2, "Mark Morris Institute" },
                    { 108, null, 1, "São Bernardo", 50.0, 90.0, 50.0, 90.0, 3, "Mark Morris Institute" },
                    { 109, null, 1, "Terranova", 55.0, 69.0, 50.0, 60.0, 3, "Mark Morris Institute" },
                    { 110, null, 1, "Terrier Australiano", 6.5, 6.5, 6.5, 6.5, 1, "Mark Morris Institute" },
                    { 111, null, 1, "Terrier Galês", 9.0, 9.5, 9.0, 9.5, 1, "Mark Morris Institute" },
                    { 112, null, 1, "Terrier Irlandês", 11.0, 12.0, 11.0, 12.0, 2, "Mark Morris Institute" },
                    { 113, null, 1, "Toy Spaniel Inglês", 3.5, 6.5, 3.5, 6.5, 1, "Mark Morris Institute" },
                    { 114, null, 1, "Welsh Corgi Cardigan", 15.5, 15.5, 11.399999618530273, 13.600000381469727, 2, "Mark Morris Institute" },
                    { 115, null, 1, "West Highland White Terrier", 7.0, 10.0, 7.0, 10.0, 1, "Mark Morris Institute" },
                    { 116, null, 1, "Whippet", 13.0, 13.0, 13.0, 13.0, 2, "Mark Morris Institute" },
                    { 117, null, 1, "Yorkshire Terrier", 3.5, 3.5, 3.5, 3.5, 1, "Mark Morris Institute" },
                    { 118, null, 1, "Sem Raça Definida (SRD)", null, null, null, null, null, "" },
                    { 119, null, 2, "Abyssinian", null, null, null, null, null, "" },
                    { 120, null, 2, "American Bobtail", null, null, null, null, null, "" },
                    { 121, null, 2, "American Curl", null, null, null, null, null, "" },
                    { 122, null, 2, "American Shorthair", null, null, null, null, null, "" },
                    { 123, null, 2, "American Wirehair", null, null, null, null, null, "" },
                    { 124, null, 2, "Balinese", null, null, null, null, null, "" },
                    { 125, null, 2, "Bengal", null, null, null, null, null, "" },
                    { 126, null, 2, "Birman", null, null, null, null, null, "" },
                    { 127, null, 2, "Bombay", null, null, null, null, null, "" },
                    { 128, null, 2, "British Shorthair", null, null, null, null, null, "" },
                    { 129, null, 2, "Burmese", null, null, null, null, null, "" },
                    { 130, null, 2, "Chartreux", null, null, null, null, null, "" },
                    { 131, null, 2, "Cornish Rex", null, null, null, null, null, "" },
                    { 132, null, 2, "Devon Rex", null, null, null, null, null, "" },
                    { 133, null, 2, "Egyptian Mau", null, null, null, null, null, "" },
                    { 134, null, 2, "Exotic Shorthair", null, null, null, null, null, "" },
                    { 135, null, 2, "Havana Brown", null, null, null, null, null, "" },
                    { 136, null, 2, "Himalayan", null, null, null, null, null, "" },
                    { 137, null, 2, "Japanese Bobtail", null, null, null, null, null, "" },
                    { 138, null, 2, "Korat", null, null, null, null, null, "" },
                    { 139, null, 2, "LaPerm", null, null, null, null, null, "" },
                    { 140, null, 2, "Maine Coon", null, null, null, null, null, "" },
                    { 141, null, 2, "Manx", null, null, null, null, null, "" },
                    { 142, null, 2, "Norwegian Forest Cat", null, null, null, null, null, "" },
                    { 143, null, 2, "Ocicat", null, null, null, null, null, "" },
                    { 144, null, 2, "Oriental", null, null, null, null, null, "" },
                    { 145, null, 2, "Persian", null, null, null, null, null, "" },
                    { 146, null, 2, "Ragamuffin", null, null, null, null, null, "" },
                    { 147, null, 2, "Ragdoll", null, null, null, null, null, "" },
                    { 148, null, 2, "Russian Blue", null, null, null, null, null, "" },
                    { 149, null, 2, "Scottish Fold", null, null, null, null, null, "" },
                    { 150, null, 2, "Selkirk Rex", null, null, null, null, null, "" },
                    { 151, null, 2, "Sem Raça Definida (SRD)", null, null, null, null, null, "" },
                    { 152, null, 2, "Siamese", null, null, null, null, null, "" },
                    { 153, null, 2, "Siberian", null, null, null, null, null, "" },
                    { 154, null, 2, "Singapura", null, null, null, null, null, "" },
                    { 155, null, 2, "Somali", null, null, null, null, null, "" },
                    { 156, null, 2, "Sphynx", null, null, null, null, null, "" },
                    { 157, null, 2, "Tonkinese", null, null, null, null, null, "" },
                    { 158, null, 2, "Turkish Angora", null, null, null, null, null, "" },
                    { 159, null, 2, "Turkish Van", null, null, null, null, null, "" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcompanhamentosInds_AnimalId",
                table: "AcompanhamentosInds",
                column: "AnimalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Animais_RacaId",
                table: "Animais",
                column: "RacaId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimaisClinicas_AnimalId",
                table: "AnimaisClinicas",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimaisClinicas_ClinicaId",
                table: "AnimaisClinicas",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_AnimalId",
                table: "Consultas",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_ClinicaId",
                table: "Consultas",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_TutorId",
                table: "Consultas",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_Consultas_UsuarioId",
                table: "Consultas",
                column: "UsuarioId");

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
                name: "IX_Racas_ClinicaId",
                table: "Racas",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Racoes_ClinicaId",
                table: "Racoes",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_AcompanhamentoIndividualizadoId",
                table: "Relatorios",
                column: "AcompanhamentoIndividualizadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_ConsultaId",
                table: "Relatorios",
                column: "ConsultaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_RacaoId",
                table: "Relatorios",
                column: "RacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_TutoresAnimais_AnimalId",
                table: "TutoresAnimais",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_TutoresAnimais_TutorId",
                table: "TutoresAnimais",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_TutoresClinicas_ClinicaId",
                table: "TutoresClinicas",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_TutoresClinicas_TutorId",
                table: "TutoresClinicas",
                column: "TutorId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPerfisClinicas_ClinicaId",
                table: "UsuariosPerfisClinicas",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosPerfisClinicas_UsuarioId",
                table: "UsuariosPerfisClinicas",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnimaisClinicas");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DietasPersonalizadas");

            migrationBuilder.DropTable(
                name: "TutoresAnimais");

            migrationBuilder.DropTable(
                name: "TutoresClinicas");

            migrationBuilder.DropTable(
                name: "UsuariosPerfisClinicas");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Relatorios");

            migrationBuilder.DropTable(
                name: "AcompanhamentosInds");

            migrationBuilder.DropTable(
                name: "Consultas");

            migrationBuilder.DropTable(
                name: "Racoes");

            migrationBuilder.DropTable(
                name: "Animais");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Tutores");

            migrationBuilder.DropTable(
                name: "Racas");

            migrationBuilder.DropTable(
                name: "Clinicas");
        }
    }
}
