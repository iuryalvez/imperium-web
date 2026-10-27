using System.ComponentModel.DataAnnotations;

namespace backend.Domain.Usuarios.DTOs.Usuario
{
    public class EditarUsuarioClinicaDto
    {
        [Required]
        public int UpcId { get; set; } // O ID do vínculo é a chave principal aqui

        // Campos principais
        [Required]
        public string Nome { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public List<string>? Perfis { get; set; }

        // Campos opcionais
        public string? Senha { get; set; }
        public string? Cpf { get; set; }
        public string? Formacao { get; set; }
        public string? Telefone { get; set; }
        public string? Whatsapp { get; set; }

        // Os campos Id e ClinicaId podem ser obtidos no backend a partir do UpcId
        // Não precisam vir do frontend
    }
}