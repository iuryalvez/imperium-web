using Microsoft.AspNetCore.Mvc;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using Microsoft.AspNetCore.Authorization;
using backend.Domain.AtendimentoAnimal.Services;
using backend.Domain.GestaoDeAcesso.Interfaces;
[ApiController]
// Padronizado para o plural, que é a convenção REST para coleções de recursos.
[Route("api/clinica/{clinicaId}/acompanhamentos")]
[Authorize]
public class AcompanhamentoController : ControllerBase
{
    private readonly IAcompanhamentoService _acompanhamentoService;

    public AcompanhamentoController(IAcompanhamentoService acompanhamentoService)
    {
        _acompanhamentoService = acompanhamentoService;
    }

    /// <summary>
    /// Obtém o histórico completo de um animal a partir do ID do acompanhamento.
    /// </summary>
    [HttpGet("{acompanhamentoId}/historico-completo")]
    [ProducesResponseType(typeof(ServiceResult), 200)]
    [ProducesResponseType(typeof(ServiceResult), 404)]
    public async Task<IActionResult> GetHistoricoCompleto([FromRoute] int clinicaId, [FromRoute] int acompanhamentoId)
    {
        // CORREÇÃO: A variável correta da rota é 'acompanhamentoId'.
        var result = await _acompanhamentoService.ObterHistoricoCompletoAnimalAsync(acompanhamentoId, clinicaId);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        // Se o histórico não for encontrado (animal inválido, etc.), retorna 404.
        return NotFound(result);
    }

    /// <summary>
    /// Cria ou atualiza uma dieta para um relatório.
    /// </summary>
    [HttpPost("{acompanhamentoId}/dieta")]
    [Authorize(Policy = "GerenciaAnimais")] // Exemplo de política de autorização
    [ProducesResponseType(typeof(ServiceResult), 200)]
    [ProducesResponseType(typeof(ServiceResult), 400)]
    public async Task<IActionResult> CriarDieta(
        [FromRoute] int clinicaId,
        [FromRoute] int acompanhamentoId,
        [FromBody] CreateDietaDto dto)
    {
        var result = await _acompanhamentoService.CriarDietaParaRelatorioAsync(acompanhamentoId, dto, clinicaId);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        // Se a criação falhar por dados inválidos, retorna 400.
        return BadRequest(result);
    }
}