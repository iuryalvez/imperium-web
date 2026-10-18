using Microsoft.AspNetCore.Mvc;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using Microsoft.AspNetCore.Authorization;
using backend.Domain.AtendimentoAnimal.Services;
using backend.Domain.Usuarios.Interfaces;
using backend.Domain.Auth.Extensions;

[ApiController]
// Padronizado para o plural, que é a convenção REST para coleções de recursos.
[Route("api/clinica/acompanhamentos")]
[Authorize]
public class AcompanhamentoController : ControllerBase
{
    private readonly IAcompanhamentoService _acompanhamentoService;

    public AcompanhamentoController(IAcompanhamentoService acompanhamentoService)
    {
        _acompanhamentoService = acompanhamentoService;
    }

    /// <summary>
    /// Retorna os perfis metabólicos disponíveis
    /// </summary>
    [HttpGet("perfis-metabolicos")]
    [Authorize(Policy = "GerenciaAnimais")]
    public IActionResult GetPerfisMetabolicos()
    {
        var perfis = Enum.GetValues(typeof(backend.Domain.SaudeAnimal.Enums.PerfilMetabolico))
            .Cast<backend.Domain.SaudeAnimal.Enums.PerfilMetabolico>()
            .Select(p => 
            {
                var memberInfo = p.GetType().GetMember(p.ToString())[0];
                var displayAttribute = memberInfo.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                                                 .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                                                 .FirstOrDefault();
                
                var tipoDietaRefAttribute = memberInfo.GetCustomAttributes(typeof(backend.Domain.SaudeAnimal.Attributes.TipoDietaRefAttribute), false)
                                                      .Cast<backend.Domain.SaudeAnimal.Attributes.TipoDietaRefAttribute>()
                                                      .FirstOrDefault();

                // Busca o K e P correspondente
                backend.Domain.SaudeAnimal.Services.PerfilMetabolicoDados.Dados.TryGetValue(p, out var dados);

                return new
                {
                    Value = (int)p,
                    Label = displayAttribute?.Name ?? p.ToString(),
                    TiposDieta = tipoDietaRefAttribute?.TiposDieta.Select(t => (int)t).ToArray() ?? Array.Empty<int>(),
                    K = dados.multiplicador > 0 ? dados.multiplicador : (double?)null,
                    P = dados.potencia > 0 ? dados.potencia : (double?)null,
                    Especie = (int?)dados.especie,
                    Sexo = (int?)dados.sexo
                };
            });

        return Ok(perfis);
    }

    /// <summary>
    /// Obtém o histórico completo de um animal a partir do ID do acompanhamento.
    /// </summary>
    [HttpGet("{acompanhamentoId}/historico-completo")]
    [ProducesResponseType(typeof(ServiceResult), 200)]
    [ProducesResponseType(typeof(ServiceResult), 404)]
    public async Task<IActionResult> GetHistoricoCompleto([FromRoute] int acompanhamentoId)
    {
        var clinicaId = User.GetClaimValue<int>("clinicaId");
        var result = await _acompanhamentoService.ObterHistoricoCompletoAnimalAsync(acompanhamentoId, clinicaId);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        // Se o histórico não for encontrado (animal inválido, etc.), retorna 404.
        return NotFound(result);
    }


    /// <summary>
    /// Retorna os níveis de restrição disponíveis para dietas de emagrecimento
    /// </summary>
    [HttpGet("niveis-restricao")]
    [Authorize(Policy = "GerenciaAnimais")]
    public IActionResult GetNiveisRestricao()
    {
        var niveis = Enum.GetValues(typeof(backend.Domain.SaudeAnimal.Enums.NivelRestricao))
            .Cast<backend.Domain.SaudeAnimal.Enums.NivelRestricao>()
            .Select(n => 
            {
                var memberInfo = n.GetType().GetMember(n.ToString())[0];
                var displayAttribute = memberInfo.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false)
                                                 .Cast<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                                                 .FirstOrDefault();
                
                var dados = backend.Domain.SaudeAnimal.Enums.NivelRestricaoDados.Obter(n);

                return new
                {
                    Value = (int)n,
                    Label = displayAttribute?.Name ?? n.ToString(),
                    Fator = dados.fator,
                    PercentualPerdaSemanal = dados.perdaSemanal
                };
            });

        return Ok(niveis);
    }
}