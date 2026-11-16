using System.Net;
using System.Net.Http.Json;
using backend.Domain.AtendimentoAnimal.DTOs.AcompanhamentoDto;
using backend.Domain.AtendimentoAnimal.DTOs.Avaliacao;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using backend.Domain.SaudeAnimal.Enums;
using backend.Tests.Infra;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Dietas
{
    /// <summary>
    /// Testes de integração das rotas api/clinica/dietas: calcular, recalcular, atualizar, excluir, iniciar, concluir e
    /// registros semanais de peso, com o que a dieta devolve e grava, regras, isolamento entre clínicas e perfis.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class DietaTests
    {
        private const string Rota = "/api/clinica/dietas";

        private readonly ImperiumApiFactory _factory;
        private readonly Cenario _cenario;

        public DietaTests(ImperiumApiFactory factory)
        {
            _factory = factory;
            _cenario = new Cenario(factory);
        }

        #region Calcular e recalcular

        [Fact]
        public async Task Criar_Manutencao_CalculaERetornaOsCampos()
        {
            var c = await ComAvaliacaoAsync();

            var resposta = await c.Gestor.PostAsJsonAsync(Rota, NovaDieta(c.Avaliacao));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var resultado = await LerAsync<ResultadoComDados<GetDietaDto>>(resposta);
            Assert.Equal("Dieta calculada e salva com sucesso.", resultado.Message);
            var dieta = resultado.Data;
            var energia = Math.Round(Math.Round(130 * Math.Pow(20.5, 0.75), 3), 2);
            Assert.True(dieta.Id > 0);
            Assert.Equal(c.Avaliacao.Id, dieta.AvaliacaoId);
            Assert.Equal(c.RacaoId, dieta.RacaoId);
            Assert.Equal(c.NomeRacao, dieta.NomeRacao);
            Assert.Equal(TipoDieta.Manutencao, dieta.TipoDieta);
            Assert.Equal(energia, dieta.EnergiaMetabolizavel);
            Assert.True(dieta.QtdAlimento > 0);
            Assert.Equal(20.5, dieta.PesoAtual);
            Assert.Equal(0, dieta.PesoObjetivo);
            Assert.Equal(0, dieta.PesoIdeal);
            Assert.Equal(130, dieta.FatorK);
            Assert.Equal(0.75, dieta.Expoente);
            Assert.False(string.IsNullOrWhiteSpace(dieta.PerfilNutricional));
            Assert.Equal(PerfilMetabolico.CaoAtivo, dieta.PerfilMetabolico);
            Assert.Equal(0.75, dieta.Potencia);
            Assert.False(dieta.HasAdicionalGestacaoCao);
            Assert.Null(dieta.NumeroFilhotes);
            Assert.Null(dieta.SemanaLactacao);
            Assert.Null(dieta.SemanaGestacao);
            Assert.Null(dieta.DuracaoEstimada);
            Assert.Equal("Dieta de teste", dieta.Observacoes);
            Assert.Equal(StatusDieta.Aberta, dieta.Status);
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
        public async Task Criar_QuantidadeDeAlimento_EhEnergiaDivididaPelaEnergiaDaRacao()
        {
            var c = await ComAvaliacaoAsync();

            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao);

            await using var contexto = _factory.CriarContexto();
            var racao = await contexto.Racoes.AsNoTracking().FirstAsync(r => r.Id == c.RacaoId);
            var energia = Math.Round(130 * Math.Pow(20.5, 0.75), 3);
            Assert.Equal(Math.Round(energia / racao.EnergiaMetabolizavel!.Value, 2), dieta.QtdAlimento);
        }

        [Fact]
        public async Task Criar_Emagrecimento_RetornaOsDadosDaRestricao()
        {
            var c = await ComAvaliacaoAsync();

            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao, NovaDietaEmagrecimento(c.Avaliacao, pesoAtual: 30, pesoObjetivo: 25, nivelRestricao: 2));

            Assert.Equal(TipoDieta.Emagrecimento, dieta.TipoDieta);
            Assert.Equal(30, dieta.PesoAtual);
            Assert.Equal(25, dieta.PesoObjetivo);
            Assert.Equal(NivelRestricao.Moderada, dieta.NivelRestricao);
            Assert.Equal("Moderada", dieta.NivelRestricaoLabel);
            Assert.Equal(0.60, dieta.FatorRestricao);
            Assert.Equal(1.5, dieta.PercentualPerdaSemanal);
            Assert.Equal(0.45, dieta.PerdaSemanalKg);
            Assert.Equal(Math.Round(Math.Round(130 * Math.Pow(30, 0.75) * 0.60, 3), 2), dieta.EnergiaMetabolizavel);
            Assert.Contains("Restrição", dieta.PerfilNutricional);
            Assert.False(string.IsNullOrWhiteSpace(dieta.PrevisoesJson));
            Assert.Null(dieta.SemanaAtual);
        }

        [Fact]
        public async Task Criar_SegundaVezParaAMesmaAvaliacao_RecalculaAMesmaDieta()
        {
            var c = await ComAvaliacaoAsync();
            var primeira = await CriarDietaAsync(c.Gestor, c.Avaliacao);

            var segunda = await CriarDietaAsync(c.Gestor, c.Avaliacao, NovaDieta(c.Avaliacao, pesoAtual: 28));

            Assert.Equal(primeira.Id, segunda.Id);
            Assert.Equal(28, segunda.PesoAtual);
            Assert.True(segunda.EnergiaMetabolizavel > primeira.EnergiaMetabolizavel);
            await using var contexto = _factory.CriarContexto();
            Assert.Equal(1, await contexto.Dietas.CountAsync(d => d.AvaliacaoId == c.Avaliacao.Id));
        }

        [Fact]
        public async Task Criar_AtualizaOPesoDoAnimalEAConsultaFicaConcluida()
        {
            var c = await ComAvaliacaoAsync();

            await CriarDietaAsync(c.Gestor, c.Avaliacao, NovaDieta(c.Avaliacao, pesoAtual: 27.3));

            var historico = await HistoricoAsync(c.Gestor, c.Avaliacao.AcompanhamentoId);
            Assert.Equal(27.3, historico.PesoAtual);
            Assert.Equal("Concluida", Assert.Single(historico.Consultas!).Status);
        }

        [Fact]
        public async Task Criar_GravaDataDeCriacaoEFaseDeVida()
        {
            var c = await ComAvaliacaoAsync();

            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao);

            await using var contexto = _factory.CriarContexto();
            var gravada = await contexto.Dietas.AsNoTracking().FirstAsync(d => d.Id == dieta.Id);
            Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), gravada.DataCriacao);
            Assert.NotNull(gravada.FaseVida);
            Assert.Equal(130, gravada.MultiplicadorEnergetico);
        }

        #endregion

        #region Atualizar e excluir

        [Fact]
        public async Task Atualizar_NovoPesoEObservacao_RecalculaERetornaADieta()
        {
            var c = await ComDietaAsync();

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/{c.Dieta.Id}", CorpoDeEdicao(c, pesoAtual: 26, observacoes: "Ajustada"));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var resultado = await LerAsync<ResultadoComDados<GetDietaDto>>(resposta);
            Assert.Equal("Dieta atualizada com sucesso.", resultado.Message);
            Assert.Equal(c.Dieta.Id, resultado.Data.Id);
            Assert.Equal(26, resultado.Data.PesoAtual);
            Assert.Equal("Ajustada", resultado.Data.Observacoes);
            Assert.Equal(c.NomeRacao, resultado.Data.NomeRacao);
            Assert.Equal(Math.Round(Math.Round(130 * Math.Pow(26, 0.75), 3), 2), resultado.Data.EnergiaMetabolizavel);
        }

        /// <summary>A atualização recalcula a dieta com o corpo inteiro: campo opcional ausente fica nulo (não é edição parcial).</summary>
        [Fact]
        public async Task Atualizar_SemObservacao_LimpaAObservacao()
        {
            var c = await ComDietaAsync();

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/{c.Dieta.Id}", CorpoDeEdicao(c, pesoAtual: 20.5, observacoes: null));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Null((await DietaNoHistoricoAsync(c)).Observacoes);
        }

        [Fact]
        public async Task Atualizar_EmagrecimentoComPesoAlvoMaiorQueOAtual_RetornaBadRequestENaoAltera()
        {
            var c = await ComDietaAsync();

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/{c.Dieta.Id}", new
            {
                id = c.Dieta.Id, avaliacaoId = c.Avaliacao.Id, racaoId = c.RacaoId, tipoDieta = 5, perfilMetabolico = 18,
                multiplicadorEnergetico = 130.0, potencia = 0.75, pesoAtual = 20, pesoObjetivo = 22, nivelRestricao = 2,
            });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Para emagrecimento, o peso alvo deve ser menor que o peso atual.", await TextoAsync(resposta));
            Assert.Equal(1, (await DietaNoHistoricoAsync(c)).TipoDieta);
        }

        [Fact]
        public async Task Atualizar_EmagrecimentoSemNivelDeRestricao_RetornaBadRequest()
        {
            var c = await ComDietaAsync();

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/{c.Dieta.Id}", new
            {
                id = c.Dieta.Id, avaliacaoId = c.Avaliacao.Id, racaoId = c.RacaoId, tipoDieta = 5, perfilMetabolico = 18,
                multiplicadorEnergetico = 130.0, potencia = 0.75, pesoAtual = 30, pesoObjetivo = 25,
            });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Para emagrecimento, o nível de restrição é obrigatório.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Atualizar_DietaConcluida_RetornaBadRequest()
        {
            var c = await ComDietaAsync();
            await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/iniciar", null);
            await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/concluir", null);

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/{c.Dieta.Id}", CorpoDeEdicao(c, pesoAtual: 40));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Esta dieta está concluída e é somente leitura.", await TextoAsync(resposta));
            Assert.Equal(20.5, (await DietaNoHistoricoAsync(c)).PesoAtual);
        }

        [Fact]
        public async Task Atualizar_TrocandoOTipoDeDietaEmAndamento_RetornaBadRequest()
        {
            var c = await ComDietaAsync();
            await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/iniciar", null);

            var resposta = await c.Gestor.PutAsJsonAsync($"{Rota}/{c.Dieta.Id}", new
            {
                id = c.Dieta.Id, avaliacaoId = c.Avaliacao.Id, racaoId = c.RacaoId, tipoDieta = 5, perfilMetabolico = 18,
                multiplicadorEnergetico = 130.0, potencia = 0.75, pesoAtual = 30, pesoObjetivo = 25, nivelRestricao = 2,
            });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Não é possível alterar o tipo de uma dieta em andamento.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Excluir_DietaExistente_RemoveEAvaliacaoFicaSemDieta()
        {
            var c = await ComDietaAsync();

            var resposta = await c.Gestor.DeleteAsync($"{Rota}/{c.Dieta.Id}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            await using var contexto = _factory.CriarContexto();
            Assert.False(await contexto.Dietas.AnyAsync(d => d.Id == c.Dieta.Id));
            var historico = await HistoricoAsync(c.Gestor, c.Avaliacao.AcompanhamentoId);
            var consulta = Assert.Single(historico.Consultas!);
            Assert.Null(consulta.Avaliacao!.Dieta);
            Assert.Equal("EmAndamento", consulta.Status);
        }

        #endregion

        #region Ciclo de vida

        [Fact]
        public async Task Iniciar_DietaAberta_FicaEmAndamentoComDataDeInicio()
        {
            var c = await ComDietaAsync();

            var resposta = await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/iniciar", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var dieta = await DietaNoHistoricoAsync(c);
            Assert.Equal((int)StatusDieta.EmAndamento, dieta.Status);
            Assert.Equal("Em Andamento", dieta.StatusLabel);
            Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), dieta.DataInicio);
            Assert.Null(dieta.DataConclusao);
            Assert.Null(dieta.SemanaAtual); // manutenção não é dieta semanal
        }

        [Fact]
        public async Task Iniciar_Emagrecimento_ComecaNaSemanaUm()
        {
            var c = await ComAvaliacaoAsync();
            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao, NovaDietaEmagrecimento(c.Avaliacao));

            await c.Gestor.PatchAsync($"{Rota}/{dieta.Id}/iniciar", null);

            var noHistorico = await DietaNoHistoricoAsync(c.Gestor, c.Avaliacao.AcompanhamentoId, dieta.Id);
            Assert.Equal(1, noHistorico.SemanaAtual);
            Assert.Equal(2, noHistorico.NivelRestricao);
            Assert.Equal("Moderada", noHistorico.NivelRestricaoLabel);
            Assert.Equal(0.60, noHistorico.FatorRestricao);
            Assert.Equal(1.5, noHistorico.PercentualPerdaSemanal);
            Assert.Equal(0.45, noHistorico.PerdaSemanalKg);
        }

        [Fact]
        public async Task Iniciar_ComOutraDietaEmAndamentoNoAcompanhamento_ConcluiAAnterior()
        {
            var c = await ComDietaAsync();
            await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/iniciar", null);
            var segundaConsulta = await CadastrarConsultaAsync(c.Gestor, c.AnimalId, c.TutorId);
            var segundaAvaliacao = await SalvarAvaliacaoAsync(c.Gestor, segundaConsulta, c.RacaoId);
            var segundaDieta = await CriarDietaAsync(c.Gestor, segundaAvaliacao);

            var resposta = await c.Gestor.PatchAsync($"{Rota}/{segundaDieta.Id}/iniciar", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var anterior = await DietaNoHistoricoAsync(c);
            Assert.Equal((int)StatusDieta.Concluida, anterior.Status);
            Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), anterior.DataConclusao);
            Assert.Equal((int)StatusDieta.EmAndamento, (await DietaNoHistoricoAsync(c.Gestor, c.Avaliacao.AcompanhamentoId, segundaDieta.Id)).Status);
        }

        [Fact]
        public async Task Iniciar_DietaJaIniciada_RetornaBadRequest()
        {
            var c = await ComDietaAsync();
            await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/iniciar", null);

            var resposta = await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/iniciar", null);

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Apenas dietas 'Abertas' podem ser iniciadas.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Concluir_DietaEmAndamento_FicaConcluidaComData()
        {
            var c = await ComDietaAsync();
            await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/iniciar", null);

            var resposta = await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/concluir", null);

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var dieta = await DietaNoHistoricoAsync(c);
            Assert.Equal((int)StatusDieta.Concluida, dieta.Status);
            Assert.Equal("Concluída", dieta.StatusLabel);
            Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), dieta.DataConclusao);
        }

        [Fact]
        public async Task Concluir_DietaAberta_RetornaBadRequest()
        {
            var c = await ComDietaAsync();

            var resposta = await c.Gestor.PatchAsync($"{Rota}/{c.Dieta.Id}/concluir", null);

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Apenas dietas 'Em Andamento' podem ser concluídas.", await TextoAsync(resposta));
            Assert.Equal((int)StatusDieta.Aberta, (await DietaNoHistoricoAsync(c)).Status);
        }

        #endregion

        #region Registros de peso

        [Fact]
        public async Task RegistrarPeso_Emagrecimento_ApareceNoHistoricoOrdenadoPorSemana()
        {
            var c = await ComAvaliacaoAsync();
            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao, NovaDietaEmagrecimento(c.Avaliacao));
            var data = new DateOnly(2030, 4, 1);

            var semana2 = await c.Gestor.PostAsJsonAsync($"{Rota}/{dieta.Id}/registros-peso", new { semana = 2, pesoReal = 29.1, observacao = "Boa adesão" });
            var semana1 = await c.Gestor.PostAsJsonAsync($"{Rota}/{dieta.Id}/registros-peso", new { semana = 1, pesoReal = 29.6, dataRegistro = data });

            Assert.Equal(HttpStatusCode.OK, semana2.StatusCode);
            Assert.Equal(HttpStatusCode.OK, semana1.StatusCode);
            var registros = (await DietaNoHistoricoAsync(c.Gestor, c.Avaliacao.AcompanhamentoId, dieta.Id)).RegistrosPeso;
            Assert.Equal(new[] { 1, 2 }, registros.Select(r => r.Semana));
            Assert.True(registros[0].Id > 0);
            Assert.Equal(29.6, registros[0].PesoReal);
            Assert.Equal(data, registros[0].DataRegistro);
            Assert.Null(registros[0].Observacao);
            Assert.Equal(29.1, registros[1].PesoReal);
            Assert.Equal("Boa adesão", registros[1].Observacao);
            Assert.Equal(DateOnly.FromDateTime(DateTime.UtcNow), registros[1].DataRegistro);
        }

        [Fact]
        public async Task RegistrarPeso_AvaliacaoTrazOsRegistrosNaDieta()
        {
            var c = await ComAvaliacaoAsync();
            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao, NovaDietaEmagrecimento(c.Avaliacao));
            await c.Gestor.PostAsJsonAsync($"{Rota}/{dieta.Id}/registros-peso", new { semana = 1, pesoReal = 29.6 });

            var resposta = await c.Gestor.GetAsync($"/api/clinica/consultas/{c.ConsultaId}/avaliacao");

            var avaliacao = (await LerAsync<ResultadoComDados<GetAvaliacaoDto>>(resposta)).Data;
            Assert.Equal(dieta.Id, avaliacao.Dieta!.Id);
            Assert.Equal(NivelRestricao.Moderada, avaliacao.Dieta.NivelRestricao);
            Assert.Equal("Moderada", avaliacao.Dieta.NivelRestricaoLabel);
            Assert.Equal(0.60, avaliacao.Dieta.FatorRestricao);
        }

        [Fact]
        public async Task RegistrarPeso_SemanaRepetida_RetornaBadRequest()
        {
            var c = await ComAvaliacaoAsync();
            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao, NovaDietaEmagrecimento(c.Avaliacao));
            await c.Gestor.PostAsJsonAsync($"{Rota}/{dieta.Id}/registros-peso", new { semana = 3, pesoReal = 29 });

            var resposta = await c.Gestor.PostAsJsonAsync($"{Rota}/{dieta.Id}/registros-peso", new { semana = 3, pesoReal = 28 });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Já existe um registro para a semana 3.", await TextoAsync(resposta));
            Assert.Single((await DietaNoHistoricoAsync(c.Gestor, c.Avaliacao.AcompanhamentoId, dieta.Id)).RegistrosPeso);
        }

        [Fact]
        public async Task RegistrarPeso_DietaQueNaoEhDeEmagrecimento_RetornaBadRequest()
        {
            var c = await ComDietaAsync();

            var resposta = await c.Gestor.PostAsJsonAsync($"{Rota}/{c.Dieta.Id}/registros-peso", new { semana = 1, pesoReal = 20 });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Registro de peso periódico é permitido apenas para dietas de emagrecimento.", await TextoAsync(resposta));
        }

        [Theory]
        [InlineData(0, 20.0, "A semana deve estar entre 1 e 15.")]
        [InlineData(16, 20.0, "A semana deve estar entre 1 e 15.")]
        [InlineData(1, 0.0, "Informe um peso válido.")]
        public async Task RegistrarPeso_ValoresForaDoLimite_RetornaBadRequestComMensagem(int semana, double pesoReal, string mensagem)
        {
            var c = await ComAvaliacaoAsync();
            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao, NovaDietaEmagrecimento(c.Avaliacao));

            var resposta = await c.Gestor.PostAsJsonAsync($"{Rota}/{dieta.Id}/registros-peso", new { semana, pesoReal });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains(mensagem, await TextoAsync(resposta));
        }

        [Fact]
        public async Task RemoverRegistroPeso_Existente_RemoveSoEle()
        {
            var c = await ComAvaliacaoAsync();
            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao, NovaDietaEmagrecimento(c.Avaliacao));
            await c.Gestor.PostAsJsonAsync($"{Rota}/{dieta.Id}/registros-peso", new { semana = 1, pesoReal = 29.6 });
            await c.Gestor.PostAsJsonAsync($"{Rota}/{dieta.Id}/registros-peso", new { semana = 2, pesoReal = 29.1 });
            var registros = (await DietaNoHistoricoAsync(c.Gestor, c.Avaliacao.AcompanhamentoId, dieta.Id)).RegistrosPeso;

            var resposta = await c.Gestor.DeleteAsync($"{Rota}/{dieta.Id}/registros-peso/{registros[0].Id}");

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var restante = Assert.Single((await DietaNoHistoricoAsync(c.Gestor, c.Avaliacao.AcompanhamentoId, dieta.Id)).RegistrosPeso);
            Assert.Equal(2, restante.Semana);
        }

        [Fact]
        public async Task RemoverRegistroPeso_Inexistente_RetornaNotFound()
        {
            var c = await ComDietaAsync();

            var resposta = await c.Gestor.DeleteAsync($"{Rota}/{c.Dieta.Id}/registros-peso/999999999");

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Contains("Registro não encontrado.", await TextoAsync(resposta));
        }

        #endregion

        #region Validação e ração

        [Fact]
        public async Task Criar_SemKEP_RetornaBadRequestComMensagem()
        {
            var c = await ComAvaliacaoAsync();

            var resposta = await c.Gestor.PostAsJsonAsync(Rota, new
            {
                avaliacaoId = c.Avaliacao.Id, acompanhamentoId = c.Avaliacao.AcompanhamentoId, racaoId = c.RacaoId, tipoDieta = 1, pesoAtual = 20,
            });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("O Multiplicador (K) e a Potência (P) são obrigatórios para o cálculo.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Criar_TipoDeDietaInvalido_RetornaBadRequest()
        {
            var c = await ComAvaliacaoAsync();

            var resposta = await c.Gestor.PostAsJsonAsync(Rota, new
            {
                avaliacaoId = c.Avaliacao.Id, acompanhamentoId = c.Avaliacao.AcompanhamentoId, racaoId = c.RacaoId, tipoDieta = 99,
            });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            await AssertSemDietaAsync(c.Avaliacao.Id);
        }

        [Fact]
        public async Task Criar_RacaoInativa_RetornaBadRequest()
        {
            var c = await ComAvaliacaoAsync();
            var outraRacao = await CadastrarRacaoDaClinicaAsync(c.Gestor);
            await c.Gestor.PutAsync($"/api/clinica/racoes/{outraRacao}/status?ativo=false", null);

            var resposta = await c.Gestor.PostAsJsonAsync(Rota, new
            {
                avaliacaoId = c.Avaliacao.Id, acompanhamentoId = c.Avaliacao.AcompanhamentoId, racaoId = outraRacao, tipoDieta = 1,
                perfilMetabolico = 4, multiplicadorEnergetico = 130.0, potencia = 0.75, pesoAtual = 20,
            });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            await AssertSemDietaAsync(c.Avaliacao.Id);
        }

        [Fact]
        public async Task Criar_AvaliacaoDeOutroAcompanhamento_RetornaBadRequest()
        {
            var c = await ComAvaliacaoAsync();
            var outroAnimal = await CadastrarAnimalAsync(c.Gestor, c.RacaId);
            var outroAcompanhamento = await AcompanhamentoDoAnimalAsync(_factory, outroAnimal);

            var resposta = await c.Gestor.PostAsJsonAsync(Rota, new
            {
                avaliacaoId = c.Avaliacao.Id, acompanhamentoId = outroAcompanhamento, racaoId = c.RacaoId, tipoDieta = 1,
                perfilMetabolico = 4, multiplicadorEnergetico = 130.0, potencia = 0.75, pesoAtual = 20,
            });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Avaliação não encontrada ou não pertence a este acompanhamento.", await TextoAsync(resposta));
            await AssertSemDietaAsync(c.Avaliacao.Id);
        }

        [Fact]
        public async Task Rotas_DietaInexistente_RetornaNotFound()
        {
            var c = await ComDietaAsync();
            const int inexistente = 999_999_999;

            var respostas = new[]
            {
                await c.Gestor.PutAsJsonAsync($"{Rota}/{inexistente}", CorpoDeEdicao(c, pesoAtual: 20)),
                await c.Gestor.DeleteAsync($"{Rota}/{inexistente}"),
                await c.Gestor.PatchAsync($"{Rota}/{inexistente}/iniciar", null),
                await c.Gestor.PatchAsync($"{Rota}/{inexistente}/concluir", null),
                await c.Gestor.PostAsJsonAsync($"{Rota}/{inexistente}/registros-peso", new { semana = 1, pesoReal = 20 }),
                await c.Gestor.DeleteAsync($"{Rota}/{inexistente}/registros-peso/1"),
            };

            foreach (var resposta in respostas)
            {
                Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
                Assert.Contains("Dieta não encontrada.", await TextoAsync(resposta));
            }
        }

        #endregion

        #region Isolamento entre clínicas

        [Fact]
        public async Task Rotas_DietaDeOutraClinica_RetornaNotFoundENaoAlteraNemRemove()
        {
            var a = await ComDietaAsync("A");
            var b = await ComDietaAsync("B");
            var id = a.Dieta.Id;

            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.PutAsJsonAsync($"{Rota}/{id}", new
            {
                id, avaliacaoId = a.Avaliacao.Id, racaoId = b.RacaoId, tipoDieta = 1, perfilMetabolico = 4,
                multiplicadorEnergetico = 130.0, potencia = 0.75, pesoAtual = 99,
            })).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.PatchAsync($"{Rota}/{id}/iniciar", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.PatchAsync($"{Rota}/{id}/concluir", null)).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.PostAsJsonAsync($"{Rota}/{id}/registros-peso", new { semana = 1, pesoReal = 20 })).StatusCode);
            var exclusao = await b.Gestor.DeleteAsync($"{Rota}/{id}");
            Assert.Equal(HttpStatusCode.NotFound, exclusao.StatusCode);
            Assert.Contains("Dieta não encontrada.", await TextoAsync(exclusao));

            var dieta = await DietaNoHistoricoAsync(a);
            Assert.Equal((int)StatusDieta.Aberta, dieta.Status);
            Assert.Equal(20.5, dieta.PesoAtual);
            Assert.Empty(dieta.RegistrosPeso);
        }

        [Fact]
        public async Task Criar_AcompanhamentoDeOutraClinica_RetornaBadRequestENaoCria()
        {
            var a = await ComAvaliacaoAsync("A");
            var b = await ComAvaliacaoAsync("B");

            var resposta = await b.Gestor.PostAsJsonAsync(Rota, new
            {
                avaliacaoId = a.Avaliacao.Id, acompanhamentoId = a.Avaliacao.AcompanhamentoId, racaoId = b.RacaoId, tipoDieta = 1,
                perfilMetabolico = 4, multiplicadorEnergetico = 130.0, potencia = 0.75, pesoAtual = 99,
            });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            await AssertSemDietaAsync(a.Avaliacao.Id);
            Assert.Equal(20.5, (await HistoricoAsync(a.Gestor, a.Avaliacao.AcompanhamentoId)).PesoAtual);
        }

        [Fact]
        public async Task Criar_RacaoDeOutraClinica_RetornaBadRequest()
        {
            var a = await ComAvaliacaoAsync("A");
            var b = await ComAvaliacaoAsync("B");

            var resposta = await a.Gestor.PostAsJsonAsync(Rota, new
            {
                avaliacaoId = a.Avaliacao.Id, acompanhamentoId = a.Avaliacao.AcompanhamentoId, racaoId = b.RacaoId, tipoDieta = 1,
                perfilMetabolico = 4, multiplicadorEnergetico = 130.0, potencia = 0.75, pesoAtual = 20,
            });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            await AssertSemDietaAsync(a.Avaliacao.Id);
        }

        #endregion

        #region Perfis

        [Fact]
        public async Task TodasAsRotas_PerfilVeterinario_Permite()
        {
            var c = await ComAvaliacaoAsync();
            var veterinario = await _cenario.ClienteAsync(c.Clinica, Perfis.Veterinario);

            var dieta = await CriarDietaAsync(veterinario, c.Avaliacao, NovaDietaEmagrecimento(c.Avaliacao));

            Assert.Equal(HttpStatusCode.OK, (await veterinario.PatchAsync($"{Rota}/{dieta.Id}/iniciar", null)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await veterinario.PostAsJsonAsync($"{Rota}/{dieta.Id}/registros-peso", new { semana = 1, pesoReal = 29.5 })).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await veterinario.PatchAsync($"{Rota}/{dieta.Id}/concluir", null)).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await veterinario.DeleteAsync($"{Rota}/{dieta.Id}")).StatusCode);
        }

        [Theory]
        [InlineData(Perfis.Secretario)]
        [InlineData(Perfis.Tecnico)]
        public async Task TodasAsRotas_PerfilSemGerenciaAcompanhamento_RetornaForbiddenENaoAltera(string perfil)
        {
            var c = await ComDietaAsync();
            var cliente = await _cenario.ClienteAsync(c.Clinica, perfil);
            var id = c.Dieta.Id;

            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PostAsJsonAsync(Rota, NovaDieta(c.Avaliacao, pesoAtual: 99))).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PutAsJsonAsync($"{Rota}/{id}", CorpoDeEdicao(c, pesoAtual: 99))).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PatchAsync($"{Rota}/{id}/iniciar", null)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PatchAsync($"{Rota}/{id}/concluir", null)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PostAsJsonAsync($"{Rota}/{id}/registros-peso", new { semana = 1, pesoReal = 20 })).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.DeleteAsync($"{Rota}/{id}/registros-peso/1")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.DeleteAsync($"{Rota}/{id}")).StatusCode);

            var dieta = await DietaNoHistoricoAsync(c);
            Assert.Equal((int)StatusDieta.Aberta, dieta.Status);
            Assert.Equal(20.5, dieta.PesoAtual);
        }

        [Fact]
        public async Task Criar_SemToken_RetornaUnauthorized()
        {
            var c = await ComAvaliacaoAsync();

            var resposta = await _cenario.ClienteAnonimo().PostAsJsonAsync(Rota, NovaDieta(c.Avaliacao));

            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        }

        #endregion

        #region Auxiliares

        private record ComAvaliacao(Atendimento Atendimento, GetAvaliacaoDto Avaliacao)
        {
            public HttpClient Gestor => Atendimento.Gestor;
            public ClinicaDeTeste Clinica => Atendimento.Clinica;
            public int RacaoId => Atendimento.RacaoId;
            public int RacaId => Atendimento.RacaId;
            public int AnimalId => Atendimento.AnimalId;
            public int TutorId => Atendimento.TutorId;
            public int ConsultaId => Atendimento.ConsultaId;
            public string NomeRacao => Atendimento.NomeRacao;
        }

        private record ComDieta(Atendimento Atendimento, GetAvaliacaoDto Avaliacao, GetDietaDto Dieta) : ComAvaliacao(Atendimento, Avaliacao);

        /// <summary>Atendimento com a avaliação já salva (peso 20,5 kg), pronta para receber dieta.</summary>
        private async Task<ComAvaliacao> ComAvaliacaoAsync(string apelido = "A")
        {
            var atendimento = await MontarAtendimentoAsync(_cenario, apelido);
            var avaliacao = await SalvarAvaliacaoAsync(atendimento.Gestor, atendimento.ConsultaId, atendimento.RacaoId);
            return new ComAvaliacao(atendimento, avaliacao);
        }

        /// <summary>Atendimento com avaliação e dieta de manutenção aberta.</summary>
        private async Task<ComDieta> ComDietaAsync(string apelido = "A")
        {
            var c = await ComAvaliacaoAsync(apelido);
            var dieta = await CriarDietaAsync(c.Gestor, c.Avaliacao);
            return new ComDieta(c.Atendimento, c.Avaliacao, dieta);
        }

        private static object CorpoDeEdicao(ComDieta c, double pesoAtual, string? observacoes = "Dieta de teste") => new
        {
            id = c.Dieta.Id,
            avaliacaoId = c.Avaliacao.Id,
            racaoId = c.RacaoId,
            tipoDieta = 1,
            perfilMetabolico = 4,
            multiplicadorEnergetico = 130.0,
            potencia = 0.75,
            pesoAtual,
            observacoes,
        };

        private static Task<DietaResumidaDto> DietaNoHistoricoAsync(ComDieta c) =>
            DietaNoHistoricoAsync(c.Gestor, c.Avaliacao.AcompanhamentoId, c.Dieta.Id);

        /// <summary>A dieta como aparece no histórico completo do animal (não há rota GET só da dieta).</summary>
        private static async Task<DietaResumidaDto> DietaNoHistoricoAsync(HttpClient cliente, int acompanhamentoId, int dietaId)
        {
            var historico = await HistoricoAsync(cliente, acompanhamentoId);
            return historico.Consultas!
                .Where(consulta => consulta.Avaliacao?.Dieta != null)
                .Select(consulta => consulta.Avaliacao!.Dieta)
                .Single(dieta => dieta.Id == dietaId);
        }

        private async Task AssertSemDietaAsync(int avaliacaoId)
        {
            await using var contexto = _factory.CriarContexto();
            Assert.False(await contexto.Dietas.AnyAsync(d => d.AvaliacaoId == avaliacaoId));
        }

        #endregion
    }
}
