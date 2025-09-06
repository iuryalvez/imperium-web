using Microsoft.AspNetCore.Mvc;
using backend.Domain.SaudeAnimal.DTOs.Dieta;
using Microsoft.AspNetCore.Authorization;
using backend.Domain.AtendimentoAnimal.Services;

[ApiController]
[Route("api/clinica/{clinicaId}/acompanhamentos")] // Rota base para ações de acompanhamento
public class AcompanhamentoController : ControllerBase
{
    private readonly IAcompanhamentoService _acompanhamentoService;

    public AcompanhamentoController(IAcompanhamentoService acompanhamentoService)
    {
        _acompanhamentoService = acompanhamentoService;
    }

    /// <summary>
    /// Cria ou atualiza uma dieta personalizada para um relatório específico dentro de um acompanhamento.
    /// </summary>
    /// <param name="clinicaId">ID da clínica (da rota).</param>
    /// <param name="acompanhamentoId">ID do acompanhamento (da rota).</param>
    /// <param name="dto">Dados da dieta a ser criada (do corpo da requisição).</param>
    /// <returns>O resultado da operação contendo a dieta criada.</returns>
    [HttpPost("{acompanhamentoId}/dieta")] // Rota: POST api/clinica/1/acompanhamentos/15/dietas
    [Authorize(Policy = "GerenciaAnimais")]
    public async Task<IActionResult> CriarDieta(
        [FromRoute] int clinicaId, 
        [FromRoute] int acompanhamentoId, 
        [FromBody] CreateDietaDto dto)
    {
        // A lógica de negócio está perfeitamente encapsulada no serviço.
        // O controller apenas orquestra a chamada.
        var result = await _acompanhamentoService.CriarDietaParaRelatorioAsync(acompanhamentoId, dto, clinicaId);

        if (result.Succeeded)
        {
            // Retorna um status 200 OK com os dados da dieta criada
            return Ok(result);
        }

        // Retorna um status 400 Bad Request com a mensagem de erro vinda do serviço
        return BadRequest(result);
    }
}