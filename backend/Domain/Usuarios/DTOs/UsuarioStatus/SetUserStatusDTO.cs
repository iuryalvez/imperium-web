using System.ComponentModel.DataAnnotations;

namespace backend.Domain.Usuarios.DTOs.UsuarioStatus;

public class SetUserStatusDto
{
    // Este ID pode ser o Usuario.Id ou o UsuarioPerfilClinica.Id (upcId)
    [Required]
    public int Id { get; set; }

    // Flag que define qual lógica será executada
    [Required]
    public bool IsAdmin { get; set; }

    // Define a ação a ser tomada: true para ativar, false para inativar
    [Required]
    public bool Activate { get; set; }

    // Clínica do token (rotas de clínica). Nulo nas rotas do Sistema (admin).
    public int? ClinicaId { get; set; }

    // Vínculo de quem está fazendo a ação (token de vínculo). Nulo para o admin.
    public int? UpcIdAtual { get; set; }
}