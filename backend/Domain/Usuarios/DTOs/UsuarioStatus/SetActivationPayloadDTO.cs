using System.ComponentModel.DataAnnotations;

namespace backend.Domain.Usuarios.DTOs.UsuarioStatus;

public class SetActivationPayload
{
    [Required]
    public bool Activate { get; set; }
}