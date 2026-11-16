using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using backend.Domain.AtendimentoAnimal.DTOs.AcompanhamentoDto;
using backend.Domain.SaudeAnimal.Enums;
using backend.Tests.Infra;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Acompanhamentos
{
    /// <summary>
    /// Testes de integração das rotas api/clinica/acompanhamentos: histórico completo do animal (dados do animal, tutores,
    /// consultas, avaliação e dieta aninhadas), listas de apoio, isolamento entre clínicas e o que cada perfil vê.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class AcompanhamentoTests
    {
        private const string Rota = "/api/clinica/acompanhamentos";

        private static readonly Dictionary<string, string> RotulosFaseVida = new()
        {
            ["Desconhecida"] = "Desconhecida",
            ["Filhote"] = "Filhote",
            ["Jovem"] = "Jovem",
            ["Adulto"] = "Adulto",
            ["Senior"] = "Sênior",
            ["Geriatrico"] = "Geriátrico",
        };

        private readonly ImperiumApiFactory _factory;
        private readonly Cenario _cenario;

        public AcompanhamentoTests(ImperiumApiFactory factory)
        {
            _factory = factory;
            _cenario = new Cenario(factory);
        }

        #region Histórico completo

        [Fact]
        public async Task Historico_AnimalSemConsultas_RetornaOsDadosDoAnimal()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var nomeRaca = $"Raça {Cenario.Unico()}";
            var racaId = await CadastrarRacaDaClinicaAsync(gestor, nomeRaca);
            var nomeAnimal = $"Animal {Cenario.Unico()}";
            var animalId = await CadastrarAnimalAsync(gestor, racaId, nome: nomeAnimal);
            var acompanhamentoId = await AcompanhamentoDoAnimalAsync(_factory, animalId);

            var resposta = await gestor.GetAsync($"{Rota}/{acompanhamentoId}/historico-completo");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var resultado = await LerAsync<ResultadoComDados<AnimalCompletoDto>>(resposta);
            Assert.True(resultado.Succeeded);
            var animal = resultado.Data;
            Assert.Equal(animalId, animal.Id);
            Assert.Equal(nomeAnimal, animal.Nome);
            Assert.Equal("Cao", animal.Especie);
            Assert.Equal("Macho", animal.Sexo);
            Assert.Equal(NascimentoPadrao, animal.Nascimento.ToUniversalTime());
            Assert.False(animal.IsCastrado);
            Assert.Equal(IdadeFormatada(), animal.IdadeFormatada);
            Assert.True(RotulosFaseVida.ContainsKey(animal.FaseVidaAtual));
            Assert.NotEqual("Desconhecida", animal.FaseVidaAtual);
            Assert.Equal(RotulosFaseVida[animal.FaseVidaAtual], animal.FaseVidaAtualLabel);
            Assert.Null(animal.PesoAtual);
            Assert.Null(animal.CondicaoCorporalAtual);
            Assert.Equal("Não informado", animal.CondicaoCorporalAtualLabel);
            Assert.Equal(racaId, animal.Raca.Id);
            Assert.Equal(nomeRaca, animal.Raca.Nome);
            Assert.Equal(acompanhamentoId, animal.Acompanhamento.Id);
            Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), animal.Acompanhamento.DataInicio);
            Assert.NotNull(animal.Tutores);
            Assert.Empty(animal.Tutores!);
            Assert.NotNull(animal.Consultas);
            Assert.Empty(animal.Consultas!);
        }

        [Fact]
        public async Task Historico_ComAvaliacao_RetornaOEstadoAtualDoAnimal()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var resposta = await a.Gestor.PutAsJsonAsync($"/api/clinica/consultas/{a.ConsultaId}/avaliacao", new
            {
                peso = 19.7,
                condicaoCorporal = 7,
                grauAtividade = 1,
                racaoId = a.RacaoId,
                tipoDieta = 3,
                isLactanteNumeroFilhotes = 4,
                semanaLactacao = 2,
                semanaGestacao = 0,
            });
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

            var animal = await HistoricoAsync(a.Gestor, await AcompanhamentoDoAnimalAsync(_factory, a.AnimalId));

            Assert.Equal(19.7, animal.PesoAtual);
            Assert.Equal("AcimaDoIdeal", animal.CondicaoCorporalAtual);
            Assert.Equal("7 - Sobrepeso", animal.CondicaoCorporalAtualLabel);
            Assert.Equal(4, animal.IsLactanteNumeroFilhotes);
            Assert.Equal(2, animal.SemanaLactacao);
            Assert.Equal(0, animal.SemanaGestacao);
        }

        [Fact]
        public async Task Historico_Tutores_RetornaNomeTelefoneEEmailDeCadaUm()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var (tutor1, nome1, email1) = await CadastrarTutorComContatoAsync(gestor, "62999990001");
            var (tutor2, nome2, email2) = await CadastrarTutorComContatoAsync(gestor, "62999990002");
            var racaId = await CadastrarRacaDaClinicaAsync(gestor);
            var animalId = await CadastrarAnimalAsync(gestor, racaId, tutorIds: new[] { tutor1, tutor2 });

            var animal = await HistoricoAsync(gestor, await AcompanhamentoDoAnimalAsync(_factory, animalId));

            var tutores = animal.Tutores!.OrderBy(t => t.Id).ToList();
            Assert.Equal(new[] { tutor1, tutor2 }, tutores.Select(t => t.Id));
            Assert.Equal(nome1, tutores[0].Nome);
            Assert.Equal("62999990001", tutores[0].Telefone);
            Assert.Equal(email1, tutores[0].Email);
            Assert.Equal(nome2, tutores[1].Nome);
            Assert.Equal("62999990002", tutores[1].Telefone);
            Assert.Equal(email2, tutores[1].Email);
        }

        [Fact]
        public async Task Historico_ConsultaPendente_VemSemAvaliacao()
        {
            var a = await MontarAtendimentoAsync(_cenario);

            var animal = await HistoricoAsync(a.Gestor, await AcompanhamentoDoAnimalAsync(_factory, a.AnimalId));

            var consulta = Assert.Single(animal.Consultas!);
            Assert.Equal(a.ConsultaId, consulta.Id);
            Assert.Equal(a.DataConsulta, consulta.DataConsulta.ToUniversalTime());
            Assert.Equal(a.TutorId, consulta.TutorId);
            Assert.Equal(await NomeDoUsuarioAsync(a.UsuarioGestor.UsuarioId), consulta.ProfissionalResponsavel);
            Assert.Equal("Pendente", consulta.Status);
            Assert.Null(consulta.MotivoCancelamento);
            Assert.Null(consulta.Avaliacao);
        }

        [Fact]
        public async Task Historico_ConsultaSemTutor_TrazTutorIdZero()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var semTutor = await CadastrarConsultaAsync(a.Gestor, a.AnimalId);

            var animal = await HistoricoAsync(a.Gestor, await AcompanhamentoDoAnimalAsync(_factory, a.AnimalId));

            Assert.Equal(0, animal.Consultas!.Single(c => c.Id == semTutor).TutorId);
        }

        [Fact]
        public async Task Historico_ConsultaCancelada_TrazStatusEMotivo()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            await a.Gestor.PutAsJsonAsync($"/api/clinica/consultas/{a.ConsultaId}/cancelar", new { motivo = "Tutor desmarcou" });

            var animal = await HistoricoAsync(a.Gestor, await AcompanhamentoDoAnimalAsync(_factory, a.AnimalId));

            var consulta = Assert.Single(animal.Consultas!);
            Assert.Equal("Cancelada", consulta.Status);
            Assert.Equal("Tutor desmarcou", consulta.MotivoCancelamento);
        }

        [Fact]
        public async Task Historico_ConsultaComAvaliacaoSemDieta_TrazAAvaliacaoDetalhada()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);

            var animal = await HistoricoAsync(a.Gestor, salva.AcompanhamentoId);

            var consulta = Assert.Single(animal.Consultas!);
            Assert.Equal("EmAndamento", consulta.Status);
            var avaliacao = consulta.Avaliacao!;
            Assert.Equal(salva.Id, avaliacao.Id);
            Assert.Equal(20.5, avaliacao.Peso);
            Assert.Null(avaliacao.PesoIdeal);
            Assert.Equal(5, avaliacao.CondicaoCorporal);
            Assert.Equal(2, avaliacao.GrauAtividade);
            Assert.Equal(IdadeEmAnos(), avaliacao.Idade);
            Assert.Equal(a.RacaoId, avaliacao.RacaoId);
            Assert.Equal(a.NomeRacao, avaliacao.NomeRacao);
            Assert.Equal("Duas refeições ao dia", avaliacao.DescricaoDieta);
            Assert.Equal(1, avaliacao.TipoDieta);
            Assert.False(avaliacao.IsGestante);
            Assert.Equal(0, avaliacao.IsLactanteNumeroFilhotes);
            Assert.Equal(0, avaliacao.SemanaLactacao);
            Assert.Equal(0, avaliacao.SemanaGestacao);
            Assert.Null(avaliacao.Dieta);
        }

        [Fact]
        public async Task Historico_AvaliacaoSemCamposOpcionais_TrazNulos()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            await a.Gestor.PutAsJsonAsync($"/api/clinica/consultas/{a.ConsultaId}/avaliacao", new { peso = 20, racaoId = a.RacaoId, tipoDieta = 1 });

            var animal = await HistoricoAsync(a.Gestor, await AcompanhamentoDoAnimalAsync(_factory, a.AnimalId));

            var avaliacao = Assert.Single(animal.Consultas!).Avaliacao!;
            Assert.Null(avaliacao.CondicaoCorporal);
            Assert.Null(avaliacao.GrauAtividade);
            Assert.Null(avaliacao.DescricaoDieta);
        }

        [Fact]
        public async Task Historico_ConsultaComDieta_TrazADietaResumidaComOsMesmosValoresDoCalculo()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);
            var calculada = await CriarDietaAsync(a.Gestor, salva);

            var animal = await HistoricoAsync(a.Gestor, salva.AcompanhamentoId);

            var consulta = Assert.Single(animal.Consultas!);
            Assert.Equal("Concluida", consulta.Status);
            var dieta = consulta.Avaliacao!.Dieta;
            Assert.Equal(calculada.Id, dieta.Id);
            Assert.Equal(1, dieta.TipoDieta);
            Assert.Equal("Manutenção", dieta.TipoDietaLabel);
            Assert.Equal(calculada.QtdAlimento, dieta.QuantidadeAlimentoGramas);
            Assert.Equal(calculada.EnergiaMetabolizavel, dieta.NecessidadeEnergeticaKcal);
            Assert.Equal(a.NomeRacao, dieta.RacaoUtilizada);
            Assert.Equal("Dieta de teste", dieta.Observacoes);
            Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), dieta.DataCriacao);
            Assert.Equal(20.5, dieta.PesoAtual);
            Assert.Null(dieta.PesoObjetivo);
            Assert.Null(dieta.PesoIdeal);
            Assert.Equal(130, dieta.FatorK);
            Assert.Equal(0.75, dieta.Expoente);
            Assert.Equal(calculada.PerfilNutricional, dieta.PerfilNutricional);
            Assert.Equal(PerfilMetabolico.CaoAtivo, dieta.PerfilMetabolico);
            Assert.Equal(130, dieta.MultiplicadorEnergetico);
            Assert.Equal(0.75, dieta.Potencia);
            Assert.Equal(calculada.IsCustomizado, dieta.IsCustomizado);
            Assert.False(dieta.HasAdicionalGestacaoCao);
            Assert.Null(dieta.NumeroFilhotes);
            Assert.Null(dieta.SemanaLactacao);
            Assert.Null(dieta.SemanaGestacao);
            Assert.Null(dieta.DuracaoEstimada);
            Assert.Equal((int)StatusDieta.Aberta, dieta.Status);
            Assert.Equal("Aberta", dieta.StatusLabel);
            Assert.Null(dieta.DataInicio);
            Assert.Null(dieta.DataConclusao);
            Assert.Null(dieta.SemanaAtual);
            Assert.Null(dieta.NivelRestricao);
            Assert.Null(dieta.NivelRestricaoLabel);
            Assert.Null(dieta.FatorRestricao);
            Assert.Null(dieta.PercentualPerdaSemanal);
            Assert.Null(dieta.PerdaSemanalKg);
            Assert.Empty(dieta.RegistrosPeso);
        }

        [Fact]
        public async Task Historico_DietaDeEmagrecimento_TrazObjetivoERestricao()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);
            var calculada = await CriarDietaAsync(a.Gestor, salva, NovaDietaEmagrecimento(salva, pesoAtual: 30, pesoObjetivo: 25, nivelRestricao: 1));

            var animal = await HistoricoAsync(a.Gestor, salva.AcompanhamentoId);

            var dieta = Assert.Single(animal.Consultas!).Avaliacao!.Dieta;
            Assert.Equal(5, dieta.TipoDieta);
            Assert.Equal("Emagrecimento", dieta.TipoDietaLabel);
            Assert.Equal(30, dieta.PesoAtual);
            Assert.Equal(25, dieta.PesoObjetivo);
            Assert.Equal(1, dieta.NivelRestricao);
            Assert.Equal("Severa", dieta.NivelRestricaoLabel);
            Assert.Equal(0.50, dieta.FatorRestricao);
            Assert.Equal(2.0, dieta.PercentualPerdaSemanal);
            Assert.Equal(0.6, dieta.PerdaSemanalKg);
            Assert.Equal(calculada.PrevisoesJson, dieta.PrevisoesJson);
            // A criação da dieta de emagrecimento leva o peso alvo para o peso ideal do animal
            Assert.Equal(30, animal.PesoAtual);
        }

        [Fact]
        public async Task Historico_VariasConsultas_TrazTodasDoAnimalESoDele()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var segunda = await CadastrarConsultaAsync(a.Gestor, a.AnimalId, a.TutorId, new DateTime(2030, 5, 1, 9, 0, 0, DateTimeKind.Utc));
            var outroAnimal = await CadastrarAnimalAsync(a.Gestor, a.RacaId);
            await CadastrarConsultaAsync(a.Gestor, outroAnimal);

            var animal = await HistoricoAsync(a.Gestor, await AcompanhamentoDoAnimalAsync(_factory, a.AnimalId));

            Assert.Equal(new[] { a.ConsultaId, segunda }.OrderBy(id => id), animal.Consultas!.Select(c => c.Id).OrderBy(id => id));
        }

        #endregion

        #region Isolamento e não encontrado

        [Fact]
        public async Task Historico_AcompanhamentoDeOutraClinica_RetornaNotFound()
        {
            var a = await MontarAtendimentoAsync(_cenario, "A");
            var b = await MontarAtendimentoAsync(_cenario, "B");
            var acompanhamentoDeA = await AcompanhamentoDoAnimalAsync(_factory, a.AnimalId);

            var resposta = await b.Gestor.GetAsync($"{Rota}/{acompanhamentoDeA}/historico-completo");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Contains("Acompanhamento não encontrado nesta clínica.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Historico_AcompanhamentoInexistente_RetornaNotFound()
        {
            var a = await MontarAtendimentoAsync(_cenario);

            var resposta = await a.Gestor.GetAsync($"{Rota}/999999999/historico-completo");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Contains("Acompanhamento não encontrado.", await TextoAsync(resposta));
        }

        #endregion

        #region Perfis

        [Theory]
        [InlineData(Perfis.Gestor)]
        [InlineData(Perfis.Veterinario)]
        [InlineData(Perfis.Secretario)]
        public async Task Historico_PerfilQueVeDadosDoTutor_TrazTelefoneEEmail(string perfil)
        {
            var (clinica, acompanhamentoId, nomeTutor, emailTutor) = await AnimalComTutorComContatoAsync();
            var cliente = await _cenario.ClienteAsync(clinica, perfil);

            var animal = await HistoricoAsync(cliente, acompanhamentoId);

            var tutor = Assert.Single(animal.Tutores!);
            Assert.Equal(nomeTutor, tutor.Nome);
            Assert.Equal("62999990003", tutor.Telefone);
            Assert.Equal(emailTutor, tutor.Email);
        }

        [Fact]
        public async Task Historico_PerfilTecnico_VeSoONomeDoTutor()
        {
            var (clinica, acompanhamentoId, nomeTutor, _) = await AnimalComTutorComContatoAsync();
            var tecnico = await _cenario.ClienteAsync(clinica, Perfis.Tecnico);

            var animal = await HistoricoAsync(tecnico, acompanhamentoId);

            var tutor = Assert.Single(animal.Tutores!);
            Assert.Equal(nomeTutor, tutor.Nome);
            Assert.Null(tutor.Telefone);
            Assert.Null(tutor.Email);
        }

        [Fact]
        public async Task Rotas_SemToken_RetornaUnauthorized()
        {
            var anonimo = _cenario.ClienteAnonimo();

            Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync($"{Rota}/1/historico-completo")).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync($"{Rota}/perfis-metabolicos")).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync($"{Rota}/niveis-restricao")).StatusCode);
        }

        #endregion

        #region Listas de apoio

        [Theory]
        [InlineData(Perfis.Gestor)]
        [InlineData(Perfis.Tecnico)]
        public async Task PerfisMetabolicos_QualquerPerfil_RetornaOsDezenovePerfisComRotulo(string perfil)
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var cliente = await _cenario.ClienteAsync(clinica, perfil);

            var resposta = await cliente.GetAsync($"{Rota}/perfis-metabolicos");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            using var json = JsonDocument.Parse(await TextoAsync(resposta));
            var itens = json.RootElement.EnumerateArray().ToList();
            Assert.Equal(19, itens.Count);
            var caoAtivo = itens.Single(i => i.GetProperty("value").GetInt32() == (int)PerfilMetabolico.CaoAtivo);
            Assert.Equal("Cão Ativo ou de Canil - Necessidade média (130 × PC^0,75)", caoAtivo.GetProperty("label").GetString());
            Assert.Equal(130, caoAtivo.GetProperty("k").GetDouble());
            Assert.Equal(0.75, caoAtivo.GetProperty("p").GetDouble());
            Assert.Contains(1, caoAtivo.GetProperty("tiposDieta").EnumerateArray().Select(t => t.GetInt32()));
        }

        [Fact]
        public async Task NiveisRestricao_RetornaOsTresNiveisComFatorEPerda()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var cliente = await _cenario.ClienteAsync(clinica, Perfis.Tecnico);

            var resposta = await cliente.GetAsync($"{Rota}/niveis-restricao");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            using var json = JsonDocument.Parse(await TextoAsync(resposta));
            var itens = json.RootElement.EnumerateArray().ToList();
            Assert.Equal(new[] { 1, 2, 3 }, itens.Select(i => i.GetProperty("value").GetInt32()));
            Assert.Equal(new[] { "Severa", "Moderada", "Branda" }, itens.Select(i => i.GetProperty("label").GetString()));
            Assert.Equal(new[] { 0.50, 0.60, 0.70 }, itens.Select(i => i.GetProperty("fator").GetDouble()));
            Assert.Equal(new[] { 0.020, 0.015, 0.010 }, itens.Select(i => i.GetProperty("percentualPerdaSemanal").GetDouble()));
        }

        #endregion

        #region Auxiliares

        /// <summary>Cadastra um tutor com telefone e devolve id, nome e e-mail.</summary>
        private static async Task<(int Id, string Nome, string Email)> CadastrarTutorComContatoAsync(HttpClient cliente, string telefone)
        {
            var nome = $"Tutor {Cenario.Unico()}";
            var email = $"tutor.{Cenario.Unico()}@teste.local";
            var resposta = await cliente.PostAsJsonAsync("/api/clinica/tutores/cadastrar", new { nome, cpf = GeradorCpf.Novo(), email, telefone });
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return (await LerIdDoDataAsync(resposta, "tutorId"), nome, email);
        }

        /// <summary>Clínica com um animal ligado a um tutor que tem telefone e e-mail.</summary>
        private async Task<(ClinicaDeTeste Clinica, int AcompanhamentoId, string NomeTutor, string EmailTutor)> AnimalComTutorComContatoAsync()
        {
            var clinica = await _cenario.CriarClinicaAsync("A");
            var gestor = await _cenario.ClienteAsync(clinica, Perfis.Gestor);
            var (tutorId, nome, email) = await CadastrarTutorComContatoAsync(gestor, "62999990003");
            var racaId = await CadastrarRacaDaClinicaAsync(gestor);
            var animalId = await CadastrarAnimalAsync(gestor, racaId, tutorIds: new[] { tutorId });
            return (clinica, await AcompanhamentoDoAnimalAsync(_factory, animalId), nome, email);
        }

        private async Task<string> NomeDoUsuarioAsync(int usuarioId)
        {
            await using var contexto = _factory.CriarContexto();
            return (await contexto.Usuarios.AsNoTracking().FirstAsync(u => u.Id == usuarioId)).Nome;
        }

        /// <summary>Idade por extenso do animal de teste, como o histórico devolve ("6 anos e 6 meses").</summary>
        private static string IdadeFormatada()
        {
            var hoje = DateTime.Today;
            var anos = hoje.Year - NascimentoPadrao.Year;
            var meses = hoje.Month - NascimentoPadrao.Month;
            if (hoje.Day < NascimentoPadrao.Day) meses--;
            if (meses < 0)
            {
                anos--;
                meses += 12;
            }
            if (anos == 0) return $"{meses} meses";
            if (meses == 0) return $"{anos} anos";
            return $"{anos} anos e {meses} meses";
        }

        #endregion
    }
}
