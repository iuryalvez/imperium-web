using System.ComponentModel.DataAnnotations;

public class EditarUsuarioSistemaDto
{
    // IDs essenciais para encontrar o registro certo
    [Required]
    public int Id { get; set; }
    public int UpcId { get; set; } // ID do vínculo UsuarioPerfilClinica

    // Campos principais
    [Required]
    public string Nome { get; set; }
    
    [Required, EmailAddress]
    public string Email { get; set; }
    public List<string>? Perfis { get; set; }
    
    // Status
    public bool IsAdmin { get; set; } // Determina o fluxo da lógica
    public bool IsAtivo { get; set; }

    // --- NOVOS CAMPOS OPCIONAIS ---
    public string? Senha { get; set; } // Opcional: preencha apenas se for alterar
    public string? Cpf { get; set; }
    public string? Formacao { get; set; }
    public string? Telefone1 { get; set; }
    public string? Whatsapp { get; set; }
}