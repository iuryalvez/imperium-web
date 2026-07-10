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
    /// Cria ou atualiza uma dieta para um relatório.
    /// </summary>
    [HttpPost("{acompanhamentoId}/dieta")]
    [Authorize(Policy = "GerenciaAnimais")] // Exemplo de política de autorização
    [ProducesResponseType(typeof(ServiceResult), 200)]
    [ProducesResponseType(typeof(ServiceResult), 400)]
    public async Task<IActionResult> CriarDieta(
        [FromRoute] int acompanhamentoId,
        [FromBody] CreateDietaDto dto)
    {
        var clinicaId = User.GetClaimValue<int>("clinicaId");
        var result = await _acompanhamentoService.CriarDietaParaRelatorioAsync(acompanhamentoId, dto, clinicaId);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        // Se a criação falhar por dados inválidos, retorna 400.
        return BadRequest(result);
    }

    [HttpPut("dieta/{dietaId}")]
    [Authorize(Policy = "GerenciaAnimais")]
    public async Task<IActionResult> UpdateDieta([FromRoute] int dietaId, [FromBody] UpdateDietaDto dto)
    {
        var clinicaId = User.GetClaimValue<int>("clinicaId");
        var result = await _acompanhamentoService.UpdateDietaAsync(dietaId, dto, clinicaId);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpDelete("dieta/{dietaId}")]
    [Authorize(Policy = "GerenciaAnimais")]
    public async Task<IActionResult> DeleteDieta([FromRoute] int dietaId)
    {
        var clinicaId = User.GetClaimValue<int>("clinicaId");
        var result = await _acompanhamentoService.DeleteDietaAsync(dietaId, clinicaId);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return NotFound(result); // Ou BadRequest, dependendo do erro
    }
}