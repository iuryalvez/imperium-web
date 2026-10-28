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
                    ClinicaId = table.Column<int>(type: "integer", nullable: true),
                    CategoriaMetabolica = table.Column<int>(type: "integer", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    DataDesativacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
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
                    ClinicaId = table.Column<int>(type: "integer", nullable: true),
                    Especie = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    IsPorcentagem = table.Column<bool>(type: "boolean", nullable: false),
                    Umidade = table.Column<double>(type: "double precision", nullable: false),
                    ProteinaBruta = table.Column<double>(type: "double precision", nullable: false),
                    ExtratoEtereo = table.Column<double>(type: "double precision", nullable: false),
                    MateriaMineral = table.Column<double>(type: "double precision", nullable: false),
                    FibraBruta = table.Column<double>(type: "double precision", nullable: false),
                    MateriaSeca = table.Column<double>(type: "double precision", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    DataDesativacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                name: "Tutores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Whatsapp = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    DataDesativacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_Tutores_Clinicas_ClinicaId",
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
                name: "Animais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Especie = table.Column<int>(type: "integer", nullable: false),
                    RacaId = table.Column<int>(type: "integer", nullable: true),
                    Nascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataModificado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    DataDesativacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
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
                        name: "FK_Animais_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Animais_Racas_RacaId",
                        column: x => x.RacaId,
                        principalTable: "Racas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RacasOcultasClinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RacaId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    DataOcultacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RacasOcultasClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RacasOcultasClinicas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RacasOcultasClinicas_Racas_RacaId",
                        column: x => x.RacaId,
                        principalTable: "Racas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RacoesOcultasClinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RacaoId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    DataOcultacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RacoesOcultasClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RacoesOcultasClinicas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RacoesOcultasClinicas_Racoes_RacaoId",
                        column: x => x.RacaoId,
                        principalTable: "Racoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "Consultas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    TutorId = table.Column<int>(type: "integer", nullable: true),
                    TutorIsCadastrado = table.Column<bool>(type: "boolean", nullable: true),
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
                    CondicaoCorporal = table.Column<int>(type: "integer", nullable: true),
                    GrauAtividade = table.Column<int>(type: "integer", nullable: true),
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
                    PerfilMetabolico = table.Column<int>(type: "integer", nullable: true),
                    IsCustomizado = table.Column<bool>(type: "boolean", nullable: false),
                    MultiplicadorEnergetico = table.Column<double>(type: "double precision", nullable: true),
                    Potencia = table.Column<double>(type: "double precision", nullable: true),
                    QtdAlimento = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaMetabolizavel = table.Column<double>(type: "double precision", nullable: true),
                    PesoAtual = table.Column<double>(type: "double precision", nullable: true),
                    PesoObjetivo = table.Column<double>(type: "double precision", nullable: true),
                    PesoIdeal = table.Column<double>(type: "double precision", nullable: true),
                    DataCriacao = table.Column<DateOnly>(type: "date", nullable: false),
                    CriadoPor = table.Column<string>(type: "text", nullable: true),
                    DuracaoEstimada = table.Column<int>(type: "integer", nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    FaseVida = table.Column<int>(type: "integer", nullable: true),
                    HasAdicionalGestacaoCao = table.Column<bool>(type: "boolean", nullable: false),
                    NumeroFilhotes = table.Column<int>(type: "integer", nullable: true),
                    SemanaLactacao = table.Column<int>(type: "integer", nullable: true),
                    SemanaGestacao = table.Column<int>(type: "integer", nullable: true),
                    PrevisoesJson = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    DataInicio = table.Column<DateOnly>(type: "date", nullable: true),
                    DataConclusao = table.Column<DateOnly>(type: "date", nullable: true),
                    NivelRestricao = table.Column<int>(type: "integer", nullable: true)
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
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Cpf", "DataCadastro", "DataUltimoAcesso", "Email", "EmailConfirmed", "Formacao", "IsAdmin", "IsAtivo", "LockoutEnabled", "LockoutEnd", "Nome", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Telefone", "TwoFactorEnabled", "UserName", "Whatsapp" },
                values: new object[,]
                {
                    { -4, 0, "87654321-4321-4321-4321-cba987654321", "01234567893", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ceron@gsmail.com", true, "Administrador do Sistema", true, true, false, null, "Admin Ceron", "CERON@GSMAIL.COM", "ADMIN CERON", "AQAAAAIAAYagAAAAEEqfDnpAOVKyqg7A7FlvRKFZPdqMTKTlOPRzjPP3uyO/vtYKGA+Bd6wmkUSWOJcEZw==", null, false, "12345678-1234-1234-1234-123456789abc", "62940302010", false, "ceron", "62940302010" },
                    { -3, 0, "87654321-4321-4321-4321-cba987654321", "01234567892", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "hugo@gsmail.com", true, "Desenvolvedor do Sistema", true, true, false, null, "Admin Hugo", "HUGO@GSMAIL.COM", "ADMIN HUGO", "AQAAAAIAAYagAAAAEEqfDnpAOVKyqg7A7FlvRKFZPdqMTKTlOPRzjPP3uyO/vtYKGA+Bd6wmkUSWOJcEZw==", null, false, "12345678-1234-1234-1234-123456789abc", "62910203040", false, "hugo", "62910203040" },
                    { -2, 0, "87654321-4321-4321-4321-cba987654321", "01234567891", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "rafael@gsmail.com", true, "Desenvolvedor do Sistema", true, true, false, null, "Admin Rafael", "RAFAEL@GSMAIL.COM", "ADMIN RAFAEL", "AQAAAAIAAYagAAAAEEqfDnpAOVKyqg7A7FlvRKFZPdqMTKTlOPRzjPP3uyO/vtYKGA+Bd6wmkUSWOJcEZw==", null, false, "12345678-1234-1234-1234-123456789abc", "62912345678", false, "admin.rafael", "62912345678" },
                    { -1, 0, "87654321-4321-4321-4321-cba987654321", "01234567890", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "iury@gsmail.com", true, "Desenvolvedor do Sistema", true, true, false, null, "Admin Iury", "IURY@GSMAIL.COM", "IURY", "AQAAAAIAAYagAAAAEEqfDnpAOVKyqg7A7FlvRKFZPdqMTKTlOPRzjPP3uyO/vtYKGA+Bd6wmkUSWOJcEZw==", null, false, "12345678-1234-1234-1234-123456789abc", "62987654321", false, "admin.iury", "62987654321" }
                });

            migrationBuilder.InsertData(
                table: "Racas",
                columns: new[] { "Id", "CategoriaMetabolica", "ClinicaId", "DataCadastro", "DataDesativacao", "Especie", "IsAtivo", "Nome", "PesoMaximoReferenciaFemea", "PesoMaximoReferenciaMacho", "PesoMinimoReferenciaFemea", "PesoMinimoReferenciaMacho", "Porte", "Referencia" },
                values: new object[,]
                {
                    { 1, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Airedale Terrier", 19.0, 30.0, 19.0, 30.0, 2, "Mark Morris Institute" },
                    { 2, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Akita Inu", 34.0, 46.0, 34.0, 46.0, 3, "Mark Morris Institute" },
                    { 3, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Alaskan Malamute", 34.0, 57.0, 34.0, 57.0, 3, "Mark Morris Institute" },
                    { 4, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Basenji", 10.0, 11.0, 10.0, 11.0, 2, "Mark Morris Institute" },
                    { 5, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Basset Hound", 18.0, 27.0, 18.0, 27.0, 2, "Mark Morris Institute" },
                    { 6, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Beagle", 12.0, 14.0, 12.0, 14.0, 2, "Mark Morris Institute" },
                    { 7, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bearded Collie", 18.0, 27.0, 18.0, 27.0, 2, "Mark Morris Institute" },
                    { 8, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Beauceron", 30.0, 38.0, 30.0, 38.0, 2, "Mark Morris Institute" },
                    { 9, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bichon Frisé", 4.5, 5.5, 4.5, 5.5, 1, "Mark Morris Institute" },
                    { 10, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bichon Maltês", 1.7999999523162842, 2.7000000476837158, 1.7999999523162842, 2.7000000476837158, 1, "Mark Morris Institute" },
                    { 11, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bloodhound", 45.5, 50.0, 36.0, 41.0, 3, "Mark Morris Institute" },
                    { 12, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bobtail", 25.0, 30.0, 25.0, 30.0, 2, "Mark Morris Institute" },
                    { 13, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Boiadeiro de Berna", 40.0, 50.0, 40.0, 50.0, 3, "Mark Morris Institute" },
                    { 14, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Boiadeiro de Flandres", 35.0, 40.0, 27.0, 35.0, 3, "Mark Morris Institute" },
                    { 15, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Border Collie", 13.5, 20.5, 13.5, 20.5, 2, "Mark Morris Institute" },
                    { 16, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Border Terrier", 6.4000000953674316, 6.8000001907348633, 5.0, 6.8000001907348633, 1, "Mark Morris Institute" },
                    { 17, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Boston Terrier", 9.0, 9.0, 9.0, 9.0, 1, "Mark Morris Institute" },
                    { 18, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Boxer", 24.0, 32.0, 24.0, 32.0, 2, "Mark Morris Institute" },
                    { 19, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Braco Francês", 22.700000762939453, 27.0, 22.700000762939453, 27.0, 2, "Mark Morris Institute" },
                    { 20, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Braco Húngaro / Vizsla", 20.0, 30.0, 20.0, 30.0, 2, "Mark Morris Institute" },
                    { 21, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Braco de Saint Germain", 18.0, 26.0, 18.0, 26.0, 2, "Mark Morris Institute" },
                    { 22, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bull Terrier", 23.5, 28.0, 23.5, 28.0, 2, "Mark Morris Institute" },
                    { 23, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bull Terrier Miniatura", 4.5, 18.0, 4.5, 18.0, 1, "Mark Morris Institute" },
                    { 24, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bulldog", 23.0, 25.0, 18.0, 23.0, 2, "Mark Morris Institute" },
                    { 25, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bulldog Francês", 8.0, 13.0, 8.0, 13.0, 1, "Mark Morris Institute" },
                    { 26, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Bullmastiff", 54.0, 65.0, 54.0, 65.0, 3, "Mark Morris Institute" },
                    { 27, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Cairn Terrier", 6.0, 7.5, 6.0, 7.5, 1, "Mark Morris Institute" },
                    { 28, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pug", 6.5, 8.0, 6.5, 8.0, 1, "Mark Morris Institute" },
                    { 29, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Cavalier King Charles Spaniel", 5.0, 8.0, 5.0, 8.0, 1, "Mark Morris Institute" },
                    { 30, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Chesapeake Bay Retriever", 32.0, 36.0, 25.0, 29.5, 2, "Mark Morris Institute" },
                    { 31, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Chihuahua", 2.7000000476837158, 2.7000000476837158, 1.7999999523162842, 1.7999999523162842, 1, "Mark Morris Institute" },
                    { 32, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Chow Chow", 20.0, 30.0, 20.0, 30.0, 2, "Mark Morris Institute" },
                    { 33, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Clumber Spaniel", 32.0, 38.0, 25.0, 32.0, 2, "Mark Morris Institute" },
                    { 34, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Cocker Spaniel Americano", 11.0, 12.5, 11.0, 12.5, 2, "Mark Morris Institute" },
                    { 35, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Cocker Spaniel Inglês", 14.5, 15.5, 12.0, 12.5, 2, "Mark Morris Institute" },
                    { 36, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Collie", 30.0, 34.0, 20.0, 25.0, 2, "Mark Morris Institute" },
                    { 37, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Coton de Tulear", 5.0, 6.8000001907348633, 5.0, 6.8000001907348633, 1, "Mark Morris Institute" },
                    { 38, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Cão da Montanha dos Pirineus", 45.0, 55.0, 38.5, 45.5, 3, "Mark Morris Institute" },
                    { 39, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Cão de Água Americano", 18.0, 20.5, 11.0, 13.5, 2, "Mark Morris Institute" },
                    { 40, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Cão de Água Português", 23.0, 27.0, 16.0, 19.0, 2, "Mark Morris Institute" },
                    { 41, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Dachshund Miniatura", 4.5, 4.5, 4.5, 4.5, 1, "Mark Morris Institute" },
                    { 42, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Dandie Dinmont Terrier", 8.0, 11.0, 8.0, 11.0, 1, "Mark Morris Institute" },
                    { 43, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Doberman", 29.0, 39.0, 29.0, 39.0, 2, "Mark Morris Institute" },
                    { 44, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Dogue de Bordeuax", 54.0, 65.0, 54.0, 65.0, 3, "Mark Morris Institute" },
                    { 45, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Dogue de Bordeaux (grande)", 60.0, 65.0, 45.0, 45.0, 3, "Mark Morris Institute" },
                    { 46, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Dogue de Bordeaux (médio)", 40.0, 45.0, 35.0, 38.0, 3, "Mark Morris Institute" },
                    { 47, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Dálmata", 22.700000762939453, 27.0, 22.700000762939453, 27.0, 2, "Mark Morris Institute" },
                    { 48, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Epagneul Breton", 13.5, 18.0, 13.5, 18.0, 2, "Mark Morris Institute" },
                    { 49, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Fila Brasileiro", 40.5, 45.0, 40.5, 45.0, 3, "Mark Morris Institute" },
                    { 50, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Fox Terrier", 7.6999998092651367, 8.1999998092651367, 6.8000001907348633, 7.0, 1, "Mark Morris Institute" },
                    { 51, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Foxhound Inglês", 29.5, 32.0, 29.5, 32.0, 2, "Mark Morris Institute" },
                    { 52, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Galgo Afegão", 23.0, 27.0, 23.0, 27.0, 2, "Mark Morris Institute" },
                    { 53, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Galgo Espanhol", 27.0, 27.0, 27.0, 27.0, 2, "Mark Morris Institute" },
                    { 54, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Galgo Inglês", 29.5, 32.0, 27.0, 29.5, 2, "Mark Morris Institute" },
                    { 55, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Galgo Russo (Borzoi)", 40.0, 48.0, 35.0, 34.0, 3, "Mark Morris Institute" },
                    { 56, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Golden Retriever", 29.5, 34.0, 25.0, 29.5, 2, "Mark Morris Institute" },
                    { 57, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Gordon Setter", 32.0, 36.0, 20.5, 25.0, 2, "Mark Morris Institute" },
                    { 58, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Dogue Alemão (Grande Dinamarquês)", 55.0, 80.0, 55.0, 80.0, 3, "Mark Morris Institute" },
                    { 59, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Griffon Vendeen", 11.5, 16.0, 11.5, 16.0, 2, "Mark Morris Institute" },
                    { 60, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Griffon de Bruxelas", 2.2000000476837158, 5.5, 2.2000000476837158, 5.5, 1, "Mark Morris Institute" },
                    { 61, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Griffon de Pêlo Duro", 23.0, 27.0, 23.0, 27.0, 2, "Mark Morris Institute" },
                    { 62, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Husky Siberiano", 22.5, 27.0, 16.0, 20.0, 2, "Mark Morris Institute" },
                    { 63, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Komondor", 50.0, 68.0, 36.0, 45.0, 3, "Mark Morris Institute" },
                    { 64, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Kuvasz", 41.0, 52.0, 32.0, 45.0, 3, "Mark Morris Institute" },
                    { 65, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Labrador Retriever", 32.0, 36.0, 25.0, 29.5, 2, "Mark Morris Institute" },
                    { 66, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Leonberger", 36.299999237060547, 68.0, 36.299999237060547, 68.0, 3, "Mark Morris Institute" },
                    { 67, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Lhasa Apso", 6.4000000953674316, 8.0, 6.4000000953674316, 8.0, 1, "Mark Morris Institute" },
                    { 68, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Lulu da Pomerânia (Spitz anão)", 1.5, 3.2000000476837158, 1.5, 3.2000000476837158, 1, "Mark Morris Institute" },
                    { 69, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Mastim Espanhol", 50.0, 60.0, 50.0, 60.0, 3, "Mark Morris Institute" },
                    { 70, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Mastim Inglês", 75.0, 90.0, 75.0, 90.0, 3, "Mark Morris Institute" },
                    { 71, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Mastim Napolitano", 50.0, 70.0, 50.0, 70.0, 3, "Mark Morris Institute" },
                    { 72, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Mastim dos Pirineus", 55.0, 70.0, 55.0, 70.0, 3, "Mark Morris Institute" },
                    { 73, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pastor Alemão", 32.0, 43.0, 32.0, 43.0, 3, "Mark Morris Institute" },
                    { 74, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pastor Belga", 28.0, 30.0, 28.0, 30.0, 2, "Mark Morris Institute" },
                    { 75, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pastor Catalão", 30.0, 40.0, 20.0, 30.0, 2, "Mark Morris Institute" },
                    { 76, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pastor de Bergamasco", 32.0, 38.0, 26.0, 32.0, 3, "Mark Morris Institute" },
                    { 77, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pastor de Brie", 34.0, 34.0, 34.0, 34.0, 3, "Mark Morris Institute" },
                    { 78, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pequinês", 4.0, 6.5, 4.0, 3.5999999046325684, 1, "Mark Morris Institute" },
                    { 79, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Perdigueiro Alemão", 20.0, 30.0, 20.0, 30.0, 2, "Mark Morris Institute" },
                    { 80, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pinscher", 4.0, 5.0, 4.0, 5.0, 1, "Mark Morris Institute" },
                    { 81, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pinscher Miniatura", 4.5, 4.5, 4.5, 4.5, 1, "Mark Morris Institute" },
                    { 82, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Podengo Ibérico", 20.5, 23.0, 20.5, 23.0, 2, "Mark Morris Institute" },
                    { 83, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pointer", 29.5, 32.0, 20.0, 25.0, 2, "Mark Morris Institute" },
                    { 84, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Pointer Alemão (Braco Alemão)", 27.0, 32.0, 20.5, 25.0, 2, "Mark Morris Institute" },
                    { 85, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Poodle Miniatura", 5.0, 5.0, 5.0, 5.0, 1, "Mark Morris Institute" },
                    { 86, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Puli", 14.0, 15.0, 14.0, 13.0, 2, "Mark Morris Institute" },
                    { 87, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Rafeiro Alentejano", 45.0, 45.0, 35.5, 40.0, 3, "Mark Morris Institute" },
                    { 88, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Flat-Coated Retriever", 34.0, 36.0, 25.0, 25.0, 3, "Mark Morris Institute" },
                    { 89, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Rottweiler", 40.0, 50.0, 40.0, 50.0, 3, "Mark Morris Institute" },
                    { 90, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Saluki", 13.0, 30.0, 13.0, 30.0, 2, "Mark Morris Institute" },
                    { 91, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Samoieda", 25.0, 30.0, 17.0, 20.0, 2, "Mark Morris Institute" },
                    { 92, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Schnauzer Gigante", 30.0, 35.0, 30.0, 35.0, 2, "Mark Morris Institute" },
                    { 93, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Schnauzer Miniatura", 5.5, 7.0, 5.5, 7.0, 1, "Mark Morris Institute" },
                    { 94, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Schnauzer Médio", 15.0, 18.0, 15.0, 18.0, 2, "Mark Morris Institute" },
                    { 95, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Scottish Terrier", 9.5, 10.0, 8.0, 8.5, 2, "Mark Morris Institute" },
                    { 96, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Setter Inglês", 18.0, 25.0, 18.0, 25.0, 2, "Mark Morris Institute" },
                    { 97, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Setter Irlandês", 27.200000762939453, 31.700000762939453, 27.200000762939453, 31.700000762939453, 2, "Mark Morris Institute" },
                    { 98, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Shar-Pei", 18.0, 25.0, 18.0, 25.0, 2, "Mark Morris Institute" },
                    { 99, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Shih Tzu", 4.0, 8.0, 4.0, 8.0, 1, "Mark Morris Institute" },
                    { 100, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Silky Terrier Australiano", 3.5999999046325684, 4.5, 3.5999999046325684, 4.5, 1, "Mark Morris Institute" },
                    { 101, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Skye Terrier", 11.5, 11.5, 11.5, 11.5, 2, "Mark Morris Institute" },
                    { 102, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Spitz Alemão", 11.300000190734863, 11.300000190734863, 11.300000190734863, 11.300000190734863, 2, "Mark Morris Institute" },
                    { 103, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Spitz Finlandês", 11.300000190734863, 16.0, 11.300000190734863, 16.0, 2, "Mark Morris Institute" },
                    { 104, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Springer Spaniel Galês", 16.0, 20.0, 16.0, 20.0, 2, "Mark Morris Institute" },
                    { 105, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Springer Spaniel Inglês", 18.0, 22.5, 18.0, 22.5, 2, "Mark Morris Institute" },
                    { 106, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Staffordshire Bull Terrier", 15.0, 17.0, 11.0, 13.0, 2, "Mark Morris Institute" },
                    { 107, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Sussex Spaniel", 16.0, 18.0, 16.0, 18.0, 2, "Mark Morris Institute" },
                    { 108, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "São Bernardo", 50.0, 90.0, 50.0, 90.0, 3, "Mark Morris Institute" },
                    { 109, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Terra-nova", 55.0, 69.0, 50.0, 60.0, 3, "Mark Morris Institute" },
                    { 110, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Terrier Australiano", 6.5, 6.5, 6.5, 6.5, 1, "Mark Morris Institute" },
                    { 111, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Terrier Galês", 9.0, 9.5, 9.0, 9.5, 1, "Mark Morris Institute" },
                    { 112, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Terrier Irlandês", 11.0, 12.0, 11.0, 12.0, 2, "Mark Morris Institute" },
                    { 113, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Toy Spaniel Inglês", 3.5, 6.5, 3.5, 6.5, 1, "Mark Morris Institute" },
                    { 114, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Welsh Corgi Cardigan", 15.5, 15.5, 11.399999618530273, 13.600000381469727, 2, "Mark Morris Institute" },
                    { 115, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "West Highland White Terrier", 7.0, 10.0, 7.0, 10.0, 1, "Mark Morris Institute" },
                    { 116, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Whippet", 13.0, 13.0, 13.0, 13.0, 2, "Mark Morris Institute" },
                    { 117, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Yorkshire Terrier", 3.5, 3.5, 3.5, 3.5, 1, "Mark Morris Institute" },
                    { 118, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "Sem Raça Definida (SRD)", null, null, null, null, null, "" },
                    { 119, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Abissínio", null, null, null, null, null, "" },
                    { 120, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Bobtail Americano", null, null, null, null, null, "" },
                    { 121, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Curl Americano", null, null, null, null, null, "" },
                    { 122, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "American Shorthair", null, null, null, null, null, "" },
                    { 123, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "American Wirehair", null, null, null, null, null, "" },
                    { 124, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Balinês", null, null, null, null, null, "" },
                    { 125, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Bengal (Gato-de-Bengala)", null, null, null, null, null, "" },
                    { 126, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Birmanês (Sagrado da Birmânia)", null, null, null, null, null, "" },
                    { 127, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Bombay (Bombaim)", null, null, null, null, null, "" },
                    { 128, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "British Shorthair", null, null, null, null, null, "" },
                    { 129, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Burmese", null, null, null, null, null, "" },
                    { 130, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Chartreux", null, null, null, null, null, "" },
                    { 131, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Cornish Rex", null, null, null, null, null, "" },
                    { 132, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Devon Rex", null, null, null, null, null, "" },
                    { 133, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Mau Egípcio", null, null, null, null, null, "" },
                    { 134, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Exótico", null, null, null, null, null, "" },
                    { 135, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Havana Brown (Oriental Havana)", null, null, null, null, null, "" },
                    { 136, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Himalaio", null, null, null, null, null, "" },
                    { 137, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Bobtail Japonês", null, null, null, null, null, "" },
                    { 138, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Korat", null, null, null, null, null, "" },
                    { 139, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "LaPerm", null, null, null, null, null, "" },
                    { 140, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Maine Coon", null, null, null, null, null, "" },
                    { 141, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Manx (Manês)", null, null, null, null, null, "" },
                    { 142, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Norueguês da Floresta", null, null, null, null, null, "" },
                    { 143, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Ocicat", null, null, null, null, null, "" },
                    { 144, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Oriental Shorthair", null, null, null, null, null, "" },
                    { 145, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Persa", null, null, null, null, null, "" },
                    { 146, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Ragamuffin", null, null, null, null, null, "" },
                    { 147, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Ragdoll", null, null, null, null, null, "" },
                    { 148, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Azul Russo", null, null, null, null, null, "" },
                    { 149, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Scottish Fold", null, null, null, null, null, "" },
                    { 150, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Selkirk Rex", null, null, null, null, null, "" },
                    { 151, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Sem Raça Definida (SRD)", null, null, null, null, null, "" },
                    { 152, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Siamês", null, null, null, null, null, "" },
                    { 153, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Siberiano", null, null, null, null, null, "" },
                    { 154, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Singapura", null, null, null, null, null, "" },
                    { 155, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Somali", null, null, null, null, null, "" },
                    { 156, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Sphynx", null, null, null, null, null, "" },
                    { 157, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Tonquinês", null, null, null, null, null, "" },
                    { 158, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Angorá (Turkish Angora)", null, null, null, null, null, "" },
                    { 159, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, true, "Turkish Van", null, null, null, null, null, "" },
                    { 160, 0, null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, true, "American Pit Bull Terrier", null, null, null, null, 2, "" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcompanhamentosInds_AnimalId",
                table: "AcompanhamentosInds",
                column: "AnimalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Animais_ClinicaId",
                table: "Animais",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Animais_RacaId",
                table: "Animais",
                column: "RacaId");

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
                name: "IX_Dietas_AvaliacaoId",
                table: "Dietas",
                column: "AvaliacaoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dietas_RacaoId",
                table: "Dietas",
                column: "RacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Racas_ClinicaId",
                table: "Racas",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_RacasOcultasClinicas_ClinicaId_RacaId",
                table: "RacasOcultasClinicas",
                columns: new[] { "ClinicaId", "RacaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RacasOcultasClinicas_RacaId",
                table: "RacasOcultasClinicas",
                column: "RacaId");

            migrationBuilder.CreateIndex(
                name: "IX_Racoes_ClinicaId",
                table: "Racoes",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_RacoesOcultasClinicas_ClinicaId_RacaoId",
                table: "RacoesOcultasClinicas",
                columns: new[] { "ClinicaId", "RacaoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RacoesOcultasClinicas_RacaoId",
                table: "RacoesOcultasClinicas",
                column: "RacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosPesoDieta_DietaId_Semana",
                table: "RegistrosPesoDieta",
                columns: new[] { "DietaId", "Semana" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tutores_ClinicaId_Cpf",
                table: "Tutores",
                columns: new[] { "ClinicaId", "Cpf" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TutoresAnimais_AnimalId",
                table: "TutoresAnimais",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_TutoresAnimais_TutorId",
                table: "TutoresAnimais",
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
                name: "RacasOcultasClinicas");

            migrationBuilder.DropTable(
                name: "RacoesOcultasClinicas");

            migrationBuilder.DropTable(
                name: "RegistrosPesoDieta");

            migrationBuilder.DropTable(
                name: "TutoresAnimais");

            migrationBuilder.DropTable(
                name: "UsuariosPerfisClinicas");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Dietas");

            migrationBuilder.DropTable(
                name: "Avaliacoes");

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
