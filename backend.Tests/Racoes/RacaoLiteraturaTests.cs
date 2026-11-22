using System.Net;
using System.Net.Http.Json;
using backend.Domain.SaudeAnimal.DTOs.Racao;
using backend.Tests.Infra;
using Xunit;
using static backend.Tests.Infra.ApiDeTeste;

namespace backend.Tests.Racoes
{
    /// <summary>
    /// Casos da literatura para a energia metabolizável da ração (NRC 2006, quatro passos), pela rota real.
    /// IDs e fontes em .claude/skills/teste-visual/casos-literatura.md. Tolerância de 1 % (as fontes arredondam).
    /// </summary>
    [Collection(ApiCollection.Nome)]
    public class RacaoLiteraturaTests
    {
        private const string Rota = "/api/clinica/racoes";

        private readonly Cenario _cenario;

        public RacaoLiteraturaTests(ImperiumApiFactory factory)
        {
            _cenario = new Cenario(factory);
        }

        // A-EM-01: nrc:2007 p.3, carciofi:apostila p.7, abinpet:2024 p.22 (gato)
        // A-EM-02: nrc:2007 p.4, carciofi:apostila p.8, abinpet:2024 p.21, fediaf:2025 p.50 (cão)
        // A-EM-03: nrc:2007 p.12/15, carciofi:apostila p.15/102 (exercício de cão)
        // A-EM-04: nrc:2007 p.13–14/16, carciofi:apostila p.16–17/103 (exercício de gato)
        [Theory]
        [InlineData("A-EM-01", Especie.Gato, 10.0, 32.0, 15.0, 8.0, 3.0, 3.72)]
        [InlineData("A-EM-02", Especie.Cao, 10.0, 24.0, 12.0, 8.0, 3.0, 3.53)]
        [InlineData("A-EM-03", Especie.Cao, 10.0, 26.0, 16.0, 7.0, 3.0, 3.77)]
        [InlineData("A-EM-04", Especie.Gato, 10.0, 35.0, 20.0, 6.5, 2.5, 4.04)]
        public async Task Cadastrar_RacaoDoExemploDaLiteratura_EnergiaMetabolizavelBateComAFonte(
            string caso, int especie, double umidade, double proteina, double extratoEtereo, double materiaMineral, double fibra, double esperado)
        {
            var racao = await CadastrarEObterAsync(especie, umidade, proteina, extratoEtereo, materiaMineral, fibra);

            var diferenca = Math.Abs(racao.EnergiaMetabolizavel!.Value - esperado) / esperado;
            Assert.True(diferenca <= 0.01, $"{caso}: esperado {esperado} kcal/g, sistema deu {racao.EnergiaMetabolizavel} ({diferenca:P2}).");
        }

        // A-EM-05: nrc:2007 p.5 — distribuição por Atwater modificado. A fonte soma 346,7 com um erro de conta
        // (25 × 3,5 escrito como 87,7); o total certo é 346,5 kcal/100 g.
        [Fact]
        public async Task Cadastrar_RacaoDoExemploDeAtwater_DistribuicaoEnergeticaBateComAFonte()
        {
            var racao = await CadastrarEObterAsync(Especie.Cao, 10.0, 25.0, 14.0, 8.0, 3.0);

            Assert.Equal(87.5, racao.EnergiaProteina);
            Assert.Equal(346.5, racao.EnergiaTotalEstimativa);
        }

        private async Task<GetRacoesDto> CadastrarEObterAsync(int especie, double umidade, double proteina, double extratoEtereo, double materiaMineral, double fibra)
        {
            var clinica = await _cenario.CriarClinicaAsync("Lit");
            var gestor = (await _cenario.UsuarioDaClinicaAsync(clinica, Perfis.Gestor)).Cliente;
            var resposta = await gestor.PostAsJsonAsync($"{Rota}/cadastrar", new
            {
                nome = $"Ração da literatura {Cenario.Unico()}",
                especie,
                isPorcentagem = true,
                umidade,
                proteinaBruta = proteina,
                extratoEtereo,
                materiaMineral,
                fibraBruta = fibra,
            });
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            var id = await LerIdDoDataAsync(resposta, "id");
            var detalhe = await gestor.GetAsync($"{Rota}/{id}");
            Assert.Equal(HttpStatusCode.OK, detalhe.StatusCode);
            return await LerAsync<GetRacoesDto>(detalhe);
        }
    }
}
