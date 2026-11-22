using System.Net;
using System.Net.Http.Json;
using backend.Domain.AtendimentoAnimal.DTOs.Avaliacao;
using backend.Domain.AtendimentoAnimal.DTOs.Consulta;
using backend.Domain.AtendimentoAnimal.Enums;
using backend.Tests.Infra;
using Microsoft.EntityFrameworkCore;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Avaliacoes
{
    /// <summary>
    /// Testes de integração da avaliação de uma consulta (api/clinica/consultas/{id}/avaliacao e api/clinica/avaliacoes/{id}):
    /// salvar, ler, salvar de novo, excluir, efeito no animal e na consulta, validação, isolamento e perfis.
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class AvaliacaoTests
    {
        private readonly ImperiumApiFactory _factory;
        private readonly Cenario _cenario;

        public AvaliacaoTests(ImperiumApiFactory factory)
        {
            _factory = factory;
            _cenario = new Cenario(factory);
        }

        private static string Rota(int consultaId) => $"/api/clinica/consultas/{consultaId}/avaliacao";

        #region Caminho feliz

        [Fact]
        public async Task Salvar_PrimeiraVez_CriaAvaliacaoERetornaOsCampos()
        {
            var a = await MontarAtendimentoAsync(_cenario);

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), new
            {
                peso = 22.4,
                condicaoCorporal = 6,
                grauAtividade = 3,
                racaoId = a.RacaoId,
                descricaoDieta = "Ração seca duas vezes ao dia",
                tipoDieta = 4,
                isGestante = true,
                isLactanteNumeroFilhotes = 3,
                semanaLactacao = 2,
                semanaGestacao = 5,
            });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var resultado = await LerAsync<ResultadoComDados<GetAvaliacaoDto>>(resposta);
            Assert.True(resultado.Succeeded);
            Assert.Equal("Avaliação salva e animal atualizado com sucesso.", resultado.Message);
            var dto = resultado.Data;
            Assert.True(dto.Id > 0);
            Assert.Equal(a.ConsultaId, dto.ConsultaId);
            Assert.Equal(await AcompanhamentoDoAnimalAsync(_factory, a.AnimalId), dto.AcompanhamentoId);
            Assert.Equal(22.4, dto.Peso);
            Assert.Equal(CondicaoCorporal.PoucoAcimaDoIdeal, dto.CondicaoCorporal);
            Assert.Equal(GrauAtividade.Ativo, dto.GrauAtividade);
            Assert.Equal(a.RacaoId, dto.RacaoId);
            Assert.Equal("Ração seca duas vezes ao dia", dto.DescricaoDieta);
            Assert.Equal(4, dto.TipoDieta);
            Assert.True(dto.IsGestante);
            Assert.Equal(3, dto.IsLactanteNumeroFilhotes);
            Assert.Equal(2, dto.SemanaLactacao);
            Assert.Equal(5, dto.SemanaGestacao);
            Assert.Equal(IdadeEmAnos(), dto.Idade);
            Assert.Equal(Sexo.Macho, dto.SexoAnimal);
            Assert.Equal(a.DataConsulta, dto.DataConsulta.ToUniversalTime());
            Assert.Equal(a.NomeRacao, dto.NomeRacao);
            Assert.Null(dto.Dieta);
        }

        [Fact]
        public async Task Obter_AvaliacaoSalva_RetornaOsCamposComRacaoEConsulta()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);

            var dto = await ObterAsync(a.Gestor, a.ConsultaId);

            Assert.Equal(salva.Id, dto.Id);
            Assert.Equal(a.ConsultaId, dto.ConsultaId);
            Assert.Equal(salva.AcompanhamentoId, dto.AcompanhamentoId);
            Assert.Equal(20.5, dto.Peso);
            Assert.Equal(CondicaoCorporal.Ideal, dto.CondicaoCorporal);
            Assert.Equal(GrauAtividade.Moderado, dto.GrauAtividade);
            Assert.Equal(a.RacaoId, dto.RacaoId);
            Assert.Equal(a.NomeRacao, dto.NomeRacao);
            Assert.Equal("Duas refeições ao dia", dto.DescricaoDieta);
            Assert.Equal(1, dto.TipoDieta);
            Assert.False(dto.IsGestante);
            Assert.Equal(IdadeEmAnos(), dto.Idade);
            Assert.Equal(Sexo.Macho, dto.SexoAnimal);
            Assert.Equal(a.DataConsulta, dto.DataConsulta.ToUniversalTime());
            Assert.Null(dto.Dieta);
        }

        [Fact]
        public async Task Obter_AvaliacaoComDieta_TrazADietaAninhada()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);
            var dieta = await CriarDietaAsync(a.Gestor, salva);

            var dto = await ObterAsync(a.Gestor, a.ConsultaId);

            Assert.NotNull(dto.Dieta);
            Assert.Equal(dieta.Id, dto.Dieta!.Id);
            Assert.Equal(salva.Id, dto.Dieta.AvaliacaoId);
            Assert.Equal(dieta.QtdAlimento, dto.Dieta.QtdAlimento);
            Assert.Equal(dieta.EnergiaMetabolizavel, dto.Dieta.EnergiaMetabolizavel);
            Assert.Equal("Aberta", dto.Dieta.StatusLabel);
        }

        [Fact]
        public async Task Salvar_SegundaVez_AtualizaAMesmaAvaliacao()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var primeira = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), NovaAvaliacao(a.RacaoId, peso: 25, condicaoCorporal: 7, grauAtividade: 1));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var dto = await ObterAsync(a.Gestor, a.ConsultaId);
            Assert.Equal(primeira.Id, dto.Id);
            Assert.Equal(25, dto.Peso);
            Assert.Equal(CondicaoCorporal.AcimaDoIdeal, dto.CondicaoCorporal);
            Assert.Equal(GrauAtividade.Inativo, dto.GrauAtividade);
            await using var contexto = _factory.CriarContexto();
            Assert.Equal(1, await contexto.Avaliacoes.CountAsync(x => x.ConsultaId == a.ConsultaId));
        }

        /// <summary>
        /// Comportamento atual: o corpo substitui a avaliação inteira, então campo opcional ausente fica nulo
        /// (na avaliação e no estado do animal). Não é edição parcial.
        /// </summary>
        [Fact]
        public async Task Salvar_SegundaVezSemCamposOpcionais_LimpaOsCamposNaAvaliacaoENoAnimal()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var primeira = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), new { peso = 21, racaoId = a.RacaoId, tipoDieta = 1 });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var dto = await ObterAsync(a.Gestor, a.ConsultaId);
            Assert.Null(dto.CondicaoCorporal);
            Assert.Null(dto.GrauAtividade);
            Assert.Null(dto.DescricaoDieta);
            var historico = await HistoricoAsync(a.Gestor, primeira.AcompanhamentoId);
            Assert.Equal(21, historico.PesoAtual);
            Assert.Null(historico.CondicaoCorporalAtual);
            Assert.Equal("Não informado", historico.CondicaoCorporalAtualLabel);
        }

        [Fact]
        public async Task Salvar_AtualizaOEstadoDoAnimal()
        {
            var a = await MontarAtendimentoAsync(_cenario);

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), new
            {
                peso = 18.2,
                condicaoCorporal = 4,
                grauAtividade = 3,
                racaoId = a.RacaoId,
                tipoDieta = 3,
                isGestante = true,
                isLactanteNumeroFilhotes = 4,
                semanaLactacao = 3,
                semanaGestacao = 6,
            });

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            await using var contexto = _factory.CriarContexto();
            var animal = await contexto.Animais.AsNoTracking().FirstAsync(x => x.Id == a.AnimalId);
            Assert.Equal(18.2, animal.Peso);
            Assert.Equal(CondicaoCorporal.PoucoAbaixoDoIdeal, animal.CondicaoCorporal);
            Assert.Equal(GrauAtividade.Ativo, animal.GrauAtividade);
            Assert.True(animal.IsGestante);
            Assert.Equal(4, animal.IsLactanteNumeroFilhotes);
            Assert.Equal(3, animal.SemanaLactacao);
            Assert.Equal(6, animal.SemanaGestacao);
            // O que a avaliação não traz continua como estava
            Assert.Equal(a.RacaId, animal.RacaId);
            Assert.Equal(a.Clinica.Id, animal.ClinicaId);
            Assert.True(animal.IsAtivo);
            Assert.StartsWith("Animal ", animal.Nome);
        }

        [Fact]
        public async Task Salvar_GravaOUsuarioQuePreencheuEAIdadeDoAnimal()
        {
            var a = await MontarAtendimentoAsync(_cenario);

            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);

            await using var contexto = _factory.CriarContexto();
            var avaliacao = await contexto.Avaliacoes.AsNoTracking().FirstAsync(x => x.Id == salva.Id);
            Assert.Equal(a.UsuarioGestor.UsuarioId, avaliacao.UsuarioResponsavelId);
            Assert.Equal(IdadeEmAnos(), avaliacao.Idade);
            Assert.Null(avaliacao.PesoIdeal);
        }

        [Fact]
        public async Task Salvar_ConsultaPassaParaEmAndamentoComAvaliacaoId()
        {
            var a = await MontarAtendimentoAsync(_cenario);

            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);

            var consulta = await LerAsync<DetalhesConsultaDto>(await a.Gestor.GetAsync($"/api/clinica/consultas/{a.ConsultaId}"));
            Assert.Equal("EmAndamento", consulta.Status);
            Assert.Equal(salva.Id, consulta.AvaliacaoId);
        }

        [Fact]
        public async Task Excluir_AvaliacaoExistente_RetornaNoContentEConsultaVoltaAPendente()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);

            var resposta = await a.Gestor.DeleteAsync($"/api/clinica/avaliacoes/{salva.Id}");

            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await a.Gestor.GetAsync(Rota(a.ConsultaId))).StatusCode);
            var consulta = await LerAsync<DetalhesConsultaDto>(await a.Gestor.GetAsync($"/api/clinica/consultas/{a.ConsultaId}"));
            Assert.Equal("Pendente", consulta.Status);
            Assert.Null(consulta.AvaliacaoId);
        }

        #endregion

        #region Regras e validação

        [Fact]
        public async Task Obter_ConsultaSemAvaliacao_RetornaNotFoundComMensagem()
        {
            var a = await MontarAtendimentoAsync(_cenario);

            var resposta = await a.Gestor.GetAsync(Rota(a.ConsultaId));

            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
            Assert.Contains("Nenhum relatório encontrado para esta consulta.", await TextoAsync(resposta));
        }

        [Fact]
        public async Task Salvar_ConsultaCancelada_RetornaBadRequestENaoCria()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            await a.Gestor.PutAsJsonAsync($"/api/clinica/consultas/{a.ConsultaId}/cancelar", new { motivo = "Engano" });

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), NovaAvaliacao(a.RacaoId));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("Consulta cancelada não recebe avaliação. Reabra-a antes.", await TextoAsync(resposta));
            await AssertSemAvaliacaoAsync(a.ConsultaId);
        }

        [Fact]
        public async Task Salvar_RacaoInexistente_RetornaBadRequestENaoAlteraOAnimal()
        {
            var a = await MontarAtendimentoAsync(_cenario);

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), NovaAvaliacao(racaoId: 999_999_999, peso: 33));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            await AssertSemAvaliacaoAsync(a.ConsultaId);
            await using var contexto = _factory.CriarContexto();
            Assert.Null((await contexto.Animais.AsNoTracking().FirstAsync(x => x.Id == a.AnimalId)).Peso);
        }

        [Fact]
        public async Task Salvar_SemanaDeGestacaoAcimaDe9_RetornaBadRequestENaoCria()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var corpo = new
            {
                peso = 20.5, condicaoCorporal = 5, grauAtividade = 2, racaoId = a.RacaoId, descricaoDieta = "Duas refeições ao dia",
                tipoDieta = 4, isGestante = true, isLactanteNumeroFilhotes = 0, semanaLactacao = 0, semanaGestacao = 10,
            };

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), corpo);

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            Assert.Contains("A semana de gestação deve estar entre 1 e 9", await TextoAsync(resposta));
            await AssertSemAvaliacaoAsync(a.ConsultaId);
        }

        [Fact]
        public async Task Salvar_RacaoInativa_RetornaBadRequest()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            await a.Gestor.PutAsync($"/api/clinica/racoes/{a.RacaoId}/status?ativo=false", null);

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), NovaAvaliacao(a.RacaoId));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            await AssertSemAvaliacaoAsync(a.ConsultaId);
        }

        [Fact]
        public async Task Salvar_MantendoRacaoQueFicouInativa_RetornaOk()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);
            await a.Gestor.PutAsync($"/api/clinica/racoes/{a.RacaoId}/status?ativo=false", null);

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), NovaAvaliacao(a.RacaoId, peso: 23));

            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(23, (await ObterAsync(a.Gestor, a.ConsultaId)).Peso);
        }

        [Fact]
        public async Task Salvar_DescricaoAcimaDe2000Caracteres_RetornaBadRequest()
        {
            var a = await MontarAtendimentoAsync(_cenario);

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), new { peso = 20, racaoId = a.RacaoId, tipoDieta = 1, descricaoDieta = new string('x', 2001) });

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            await AssertSemAvaliacaoAsync(a.ConsultaId);
        }

        [Fact]
        public async Task Rotas_ConsultaOuAvaliacaoInexistente_NaoEncontra()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            const int inexistente = 999_999_999;

            Assert.Equal(HttpStatusCode.NotFound, (await a.Gestor.GetAsync(Rota(inexistente))).StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, (await a.Gestor.PutAsJsonAsync(Rota(inexistente), NovaAvaliacao(a.RacaoId))).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await a.Gestor.DeleteAsync($"/api/clinica/avaliacoes/{inexistente}")).StatusCode);
        }

        #endregion

        #region Isolamento entre clínicas

        [Fact]
        public async Task Rotas_ConsultaDeOutraClinica_NaoLeNaoSalvaNemExclui()
        {
            var a = await MontarAtendimentoAsync(_cenario, "A");
            var b = await MontarAtendimentoAsync(_cenario, "B");
            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);

            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.GetAsync(Rota(a.ConsultaId))).StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, (await b.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), NovaAvaliacao(b.RacaoId, peso: 99))).StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, (await b.Gestor.DeleteAsync($"/api/clinica/avaliacoes/{salva.Id}")).StatusCode);

            var dto = await ObterAsync(a.Gestor, a.ConsultaId);
            Assert.Equal(salva.Id, dto.Id);
            Assert.Equal(20.5, dto.Peso);
            Assert.Equal(a.RacaoId, dto.RacaoId);
        }

        [Fact]
        public async Task Salvar_RacaoDeOutraClinica_RetornaBadRequest()
        {
            var a = await MontarAtendimentoAsync(_cenario, "A");
            var b = await MontarAtendimentoAsync(_cenario, "B");

            var resposta = await a.Gestor.PutAsJsonAsync(Rota(a.ConsultaId), NovaAvaliacao(b.RacaoId));

            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
            await AssertSemAvaliacaoAsync(a.ConsultaId);
        }

        #endregion

        #region Perfis

        [Theory]
        [InlineData(Perfis.Gestor)]
        [InlineData(Perfis.Veterinario)]
        public async Task SalvarEExcluir_PerfilComGerenciaAcompanhamento_Permite(string perfil)
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var cliente = await _cenario.ClienteAsync(a.Clinica, perfil);

            var salva = await SalvarAvaliacaoAsync(cliente, a.ConsultaId, a.RacaoId);

            Assert.Equal(HttpStatusCode.NoContent, (await cliente.DeleteAsync($"/api/clinica/avaliacoes/{salva.Id}")).StatusCode);
        }

        [Theory]
        [InlineData(Perfis.Secretario)]
        [InlineData(Perfis.Tecnico)]
        public async Task SalvarEExcluir_PerfilSemGerenciaAcompanhamento_RetornaForbiddenENaoAltera(string perfil)
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var salva = await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);
            var cliente = await _cenario.ClienteAsync(a.Clinica, perfil);

            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.PutAsJsonAsync(Rota(a.ConsultaId), NovaAvaliacao(a.RacaoId, peso: 99))).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await cliente.DeleteAsync($"/api/clinica/avaliacoes/{salva.Id}")).StatusCode);

            Assert.Equal(20.5, (await ObterAsync(a.Gestor, a.ConsultaId)).Peso);
        }

        [Theory]
        [InlineData(Perfis.Gestor, HttpStatusCode.OK)]
        [InlineData(Perfis.Veterinario, HttpStatusCode.OK)]
        [InlineData(Perfis.Secretario, HttpStatusCode.OK)]
        [InlineData(Perfis.Tecnico, HttpStatusCode.Forbidden)]
        public async Task Obter_PorPerfil_SoQuemVisualizaAtendimento(string perfil, HttpStatusCode esperado)
        {
            var a = await MontarAtendimentoAsync(_cenario);
            await SalvarAvaliacaoAsync(a.Gestor, a.ConsultaId, a.RacaoId);
            var cliente = await _cenario.ClienteAsync(a.Clinica, perfil);

            Assert.Equal(esperado, (await cliente.GetAsync(Rota(a.ConsultaId))).StatusCode);
        }

        [Fact]
        public async Task Rotas_SemToken_RetornaUnauthorized()
        {
            var a = await MontarAtendimentoAsync(_cenario);
            var anonimo = _cenario.ClienteAnonimo();

            Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync(Rota(a.ConsultaId))).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.PutAsJsonAsync(Rota(a.ConsultaId), NovaAvaliacao(a.RacaoId))).StatusCode);
        }

        #endregion

        #region Auxiliares

        private static async Task<GetAvaliacaoDto> ObterAsync(HttpClient cliente, int consultaId)
        {
            var resposta = await cliente.GetAsync(Rota(consultaId));
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            return (await LerAsync<ResultadoComDados<GetAvaliacaoDto>>(resposta)).Data;
        }

        private async Task AssertSemAvaliacaoAsync(int consultaId)
        {
            await using var contexto = _factory.CriarContexto();
            Assert.False(await contexto.Avaliacoes.AnyAsync(x => x.ConsultaId == consultaId));
        }

        #endregion
    }
}
