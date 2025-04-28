using System;
using System.ComponentModel.DataAnnotations;

public class CadastrarUsuarioSistemaDto
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
    public string Email { get; set; }
    public string? UserName { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter até 100 caracteres.")]
    public string Nome { get; set; }
    public string? Senha { get; set; }
    public DateTime DataCad { get; set; }
    public DateTime DataUltimoAcesso { get; set; }
    public bool IsAdmin { get; set; }
    public List<int>? ClinicaIds { get; set; }

    public List<string>? Perfis { get; set; }
}
