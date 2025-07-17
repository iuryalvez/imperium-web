using System.ComponentModel.DataAnnotations;

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
}