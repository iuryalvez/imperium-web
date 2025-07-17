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
                    DataCad = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataUltimoAcesso = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Formacao = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsAtivo = table.Column<bool>(type: "boolean", nullable: false),
                    Telefone1 = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
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
                    Endereco = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    EndNumero = table.Column<int>(type: "integer", nullable: false),
                    EndEstado = table.Column<string>(type: "text", nullable: true),
                    EndCidade = table.Column<string>(type: "text", nullable: true),
                    Cep = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    Telefone1 = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Telefone2 = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Whatsapp = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Instagram = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DataCad = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsAtiva = table.Column<bool>(type: "boolean", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinicas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Racas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Especie = table.Column<int>(type: "integer", nullable: false),
                    PesoMinimoReferencia = table.Column<float>(type: "real", nullable: true),
                    PesoMaximoReferencia = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Racas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tutores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    Endereco = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    EndNumero = table.Column<int>(type: "integer", nullable: false),
                    EndEstado = table.Column<string>(type: "text", nullable: true),
                    EndCidade = table.Column<string>(type: "text", nullable: true),
                    Cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    Telefone1 = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Telefone2 = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Whatsapp = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    DataCad = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "Racoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Umidade = table.Column<float>(type: "real", nullable: false),
                    ProteinaBruta = table.Column<float>(type: "real", nullable: false),
                    ExtratoEtereo = table.Column<float>(type: "real", nullable: false),
                    MateriaMineral = table.Column<float>(type: "real", nullable: false),
                    FibraBruta = table.Column<float>(type: "real", nullable: false),
                    MateriaSeca = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Racoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Racoes_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosPerfisClinicas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    DataCad = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataModificado = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Sexo = table.Column<int>(type: "integer", nullable: false),
                    Castrado = table.Column<bool>(type: "boolean", nullable: false),
                    Peso = table.Column<float>(type: "real", nullable: false),
                    CondicaoCorporal = table.Column<int>(type: "integer", nullable: false),
                    GrauAtividade = table.Column<int>(type: "integer", nullable: false),
                    IsGestante = table.Column<bool>(type: "boolean", nullable: false),
                    IsLactanteNumeroFilhotes = table.Column<int>(type: "integer", nullable: false),
                    SemanaLactacao = table.Column<int>(type: "integer", nullable: false),
                    SemanaGestacao = table.Column<int>(type: "integer", nullable: false)
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
                name: "TutoresClinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TutorId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    DataAssociacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutoresClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TutoresClinicas_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TutoresClinicas_Tutores_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutores",
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnimaisClinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AnimalId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: false),
                    DataAssociacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimaisClinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimaisClinicas_Animais_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "Animais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TutoresAnimais_Tutores_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutores",
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
                    AcompanhamentoIndId = table.Column<int>(type: "integer", nullable: false),
                    TipoDieta = table.Column<int>(type: "integer", nullable: false),
                    QtdAlimento = table.Column<float>(type: "real", nullable: false),
                    EnergiaMetabolizavel = table.Column<float>(type: "real", nullable: false),
                    PesoAtual = table.Column<float>(type: "real", nullable: false),
                    PesoObjetivo = table.Column<float>(type: "real", nullable: false),
                    PesoIdeal = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietasPersonalizadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DietasPersonalizadas_AcompanhamentosInds_AcompanhamentoIndId",
                        column: x => x.AcompanhamentoIndId,
                        principalTable: "AcompanhamentosInds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DietasPersonalizadas_Racoes_RacaoId",
                        column: x => x.RacaoId,
                        principalTable: "Racoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Relatorios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConsultaId = table.Column<int>(type: "integer", nullable: false),
                    Idade = table.Column<int>(type: "integer", nullable: false),
                    Peso = table.Column<float>(type: "real", nullable: false),
                    CondicaoCorporal = table.Column<int>(type: "integer", nullable: false),
                    GrauAtividade = table.Column<int>(type: "integer", nullable: false),
                    RacaoId = table.Column<int>(type: "integer", nullable: false),
                    DescricaoDieta = table.Column<string>(type: "text", nullable: true),
                    AcompanhamentoIndId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Relatorios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Relatorios_AcompanhamentosInds_AcompanhamentoIndId",
                        column: x => x.AcompanhamentoIndId,
                        principalTable: "AcompanhamentosInds",
                        principalColumn: "Id");
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

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Cpf", "DataCad", "DataUltimoAcesso", "Email", "EmailConfirmed", "Formacao", "IsAdmin", "IsAtivo", "LockoutEnabled", "LockoutEnd", "Nome", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Telefone1", "TwoFactorEnabled", "UserName", "Whatsapp" },
                values: new object[,]
                {
                    { -4, 0, "0d749bb4-6ed3-4497-b347-5dd7b821edbf", "01234567893", new DateTime(2025, 7, 17, 2, 51, 39, 174, DateTimeKind.Utc).AddTicks(5477), new DateTime(2025, 7, 17, 2, 51, 39, 174, DateTimeKind.Utc).AddTicks(5491), "ceron@gsmail.com", true, "Administrador do Sistema", true, true, false, null, "Admin Ceron", "CERON@GSMAIL.COM", "ADMIN CERON", "AQAAAAIAAYagAAAAEKG+4oQlxTU55oOtxh9NNRUSOFaFcKgWEbW2P3le1Nbbjl7AX89Bk7HHDb6o+qZaUw==", null, false, "2bb8dc0f-6ece-446d-bfa1-207fa29e44ea", "62940302010", false, "ceron", "62940302010" },
                    { -3, 0, "da924918-cff2-4408-8c9a-2a1f01d5fc74", "01234567892", new DateTime(2025, 7, 17, 2, 51, 39, 174, DateTimeKind.Utc).AddTicks(5438), new DateTime(2025, 7, 17, 2, 51, 39, 174, DateTimeKind.Utc).AddTicks(5447), "hugo@gsmail.com", true, "Desenvolvedor do Sistema", true, true, false, null, "Admin Hugo", "HUGO@GSMAIL.COM", "ADMIN HUGO", "AQAAAAIAAYagAAAAEHGrxMvem3b7iTgafsjqOYWK0VOEi5OEl1ZeKHIwSnt0J638u5Kn8n6JV04MqVzGuQ==", null, false, "5b0b1580-1674-404e-80be-fb95f1c2b9e1", "62910203040", false, "hugo", "62910203040" },
                    { -2, 0, "e32620b8-6c4a-46f3-847c-f40ba4b6244a", "01234567891", new DateTime(2025, 7, 17, 2, 51, 39, 174, DateTimeKind.Utc).AddTicks(5395), new DateTime(2025, 7, 17, 2, 51, 39, 174, DateTimeKind.Utc).AddTicks(5405), "rafael@gsmail.com", true, "Desenvolvedor do Sistema", true, true, false, null, "Admin Rafael", "RAFAEL@GSMAIL.COM", "ADMIN RAFAEL", "AQAAAAIAAYagAAAAEBoJ8MrDkFffmjrajS/fjyaS/mRJ2iU42tWvLZxCAmsO36g55dbkF4IaHfSGpa3g2w==", null, false, "10fd176e-0476-433a-88f5-c1365169ab54", "62912345678", false, "admin.rafael", "62912345678" },
                    { -1, 0, "28eb9747-7b3f-4851-872f-0e9b7c847f95", "01234567890", new DateTime(2025, 7, 17, 2, 51, 39, 174, DateTimeKind.Utc).AddTicks(5347), new DateTime(2025, 7, 17, 2, 51, 39, 174, DateTimeKind.Utc).AddTicks(5362), "iury@gsmail.com", true, "Desenvolvedor do Sistema", true, true, false, null, "Admin Iury", "IURY@GSMAIL.COM", "IURY", "AQAAAAIAAYagAAAAEDZxyh9+8SeVJimrg62LlWtLbwmWtx0VXdyBbCror9JalavvYwNhR1jvrgWHmvW4Mw==", null, false, "cbdfb863-ba8b-49ac-8bb1-f184705db90b", "62987654321", false, "admin.iury", "62987654321" }
                });

            migrationBuilder.InsertData(
                table: "Racas",
                columns: new[] { "Id", "Especie", "Nome", "PesoMaximoReferencia", "PesoMinimoReferencia" },
                values: new object[,]
                {
                    { 1, 1, "Affenpinscher", null, null },
                    { 2, 1, "Afghan Hound", null, null },
                    { 3, 1, "Airedale Terrier", null, null },
                    { 4, 1, "Akita", null, null },
                    { 5, 1, "Alaskan Malamute", null, null },
                    { 6, 1, "American Staffordshire Terrier", null, null },
                    { 7, 1, "Australian Shepherd", null, null },
                    { 8, 1, "Basset Hound", null, null },
                    { 9, 1, "Beagle", null, null },
                    { 10, 1, "Bernese Mountain Dog", null, null },
                    { 11, 1, "Bichon Frise", null, null },
                    { 12, 1, "Border Collie", null, null },
                    { 13, 1, "Boston Terrier", null, null },
                    { 14, 1, "Boxer", null, null },
                    { 15, 1, "Bulldog", null, null },
                    { 16, 1, "Bull Terrier", null, null },
                    { 17, 1, "Cavalier King Charles Spaniel", null, null },
                    { 18, 1, "Chihuahua", null, null },
                    { 19, 1, "Chow Chow", null, null },
                    { 20, 1, "Cocker Spaniel", null, null },
                    { 21, 1, "Collie", null, null },
                    { 22, 1, "Dachshund", null, null },
                    { 23, 1, "Dalmatian", null, null },
                    { 24, 1, "Doberman Pinscher", null, null },
                    { 25, 1, "English Springer Spaniel", null, null },
                    { 26, 1, "French Bulldog", null, null },
                    { 27, 1, "German Shepherd", null, null },
                    { 28, 1, "Golden Retriever", null, null },
                    { 29, 1, "Great Dane", null, null },
                    { 30, 1, "Irish Setter", null, null },
                    { 31, 1, "Jack Russell Terrier", null, null },
                    { 32, 1, "Labrador Retriever", null, null },
                    { 33, 1, "Lhasa Apso", null, null },
                    { 34, 1, "Maltese", null, null },
                    { 35, 1, "Mastiff", null, null },
                    { 36, 1, "Miniature Schnauzer", null, null },
                    { 37, 1, "Newfoundland", null, null },
                    { 38, 1, "Pomeranian", null, null },
                    { 39, 1, "Poodle", null, null },
                    { 40, 1, "Pug", null, null },
                    { 41, 1, "Rottweiler", null, null },
                    { 42, 1, "Saint Bernard", null, null },
                    { 43, 1, "Samoyed", null, null },
                    { 44, 1, "Scottish Terrier", null, null },
                    { 45, 1, "Shetland Sheepdog", null, null },
                    { 46, 1, "Shiba Inu", null, null },
                    { 47, 1, "Shih Tzu", null, null },
                    { 48, 1, "Siberian Husky", null, null },
                    { 49, 1, "Weimaraner", null, null },
                    { 50, 1, "Welsh Corgi (Cardigan)", null, null },
                    { 51, 1, "Welsh Corgi (Pembroke)", null, null },
                    { 52, 1, "West Highland White Terrier", null, null },
                    { 53, 1, "Whippet", null, null },
                    { 54, 1, "Yorkshire Terrier", null, null },
                    { 55, 1, "Sem Raça Definida (SRD)", null, null },
                    { 101, 2, "Abyssinian", null, null },
                    { 102, 2, "American Bobtail", null, null },
                    { 103, 2, "American Curl", null, null },
                    { 104, 2, "American Shorthair", null, null },
                    { 105, 2, "American Wirehair", null, null },
                    { 106, 2, "Balinese", null, null },
                    { 107, 2, "Bengal", null, null },
                    { 108, 2, "Birman", null, null },
                    { 109, 2, "Bombay", null, null },
                    { 110, 2, "British Shorthair", null, null },
                    { 111, 2, "Burmese", null, null },
                    { 112, 2, "Chartreux", null, null },
                    { 113, 2, "Cornish Rex", null, null },
                    { 114, 2, "Devon Rex", null, null },
                    { 115, 2, "Egyptian Mau", null, null },
                    { 116, 2, "Exotic Shorthair", null, null },
                    { 117, 2, "Havana Brown", null, null },
                    { 118, 2, "Himalayan", null, null },
                    { 119, 2, "Japanese Bobtail", null, null },
                    { 120, 2, "Korat", null, null },
                    { 121, 2, "LaPerm", null, null },
                    { 122, 2, "Maine Coon", null, null },
                    { 123, 2, "Manx", null, null },
                    { 124, 2, "Norwegian Forest Cat", null, null },
                    { 125, 2, "Ocicat", null, null },
                    { 126, 2, "Oriental", null, null },
                    { 127, 2, "Persian", null, null },
                    { 128, 2, "Ragamuffin", null, null },
                    { 129, 2, "Ragdoll", null, null },
                    { 130, 2, "Russian Blue", null, null },
                    { 131, 2, "Scottish Fold", null, null },
                    { 132, 2, "Selkirk Rex", null, null },
                    { 133, 2, "Siamese", null, null },
                    { 134, 2, "Siberian", null, null },
                    { 135, 2, "Singapura", null, null },
                    { 136, 2, "Somali", null, null },
                    { 137, 2, "Sphynx", null, null },
                    { 138, 2, "Tonkinese", null, null },
                    { 139, 2, "Turkish Angora", null, null },
                    { 140, 2, "Turkish Van", null, null },
                    { 141, 2, "Sem Raça Definida (SRD)", null, null }
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
                name: "IX_DietasPersonalizadas_AcompanhamentoIndId",
                table: "DietasPersonalizadas",
                column: "AcompanhamentoIndId");

            migrationBuilder.CreateIndex(
                name: "IX_DietasPersonalizadas_RacaoId",
                table: "DietasPersonalizadas",
                column: "RacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Racoes_ClinicaId",
                table: "Racoes",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_Relatorios_AcompanhamentoIndId",
                table: "Relatorios",
                column: "AcompanhamentoIndId");

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
                name: "Relatorios");

            migrationBuilder.DropTable(
                name: "TutoresAnimais");

            migrationBuilder.DropTable(
                name: "TutoresClinicas");

            migrationBuilder.DropTable(
                name: "UsuariosPerfisClinicas");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

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
                name: "Clinicas");

            migrationBuilder.DropTable(
                name: "Racas");
        }
    }
}
