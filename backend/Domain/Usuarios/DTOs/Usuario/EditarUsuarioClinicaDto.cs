using System.ComponentModel.DataAnnotations;

namespace backend.Domain.Usuarios.DTOs.Usuario
{
    /// <summary>
    /// Edição de usuário feita pela clínica: só altera o vínculo (perfis).
    /// Dados pessoais e senha são editados pelo próprio usuário (Meu Perfil) ou pelo admin do sistema.
    /// </summary>
    public class EditarUsuarioClinicaDto
    {
        [Required]
        public int UpcId { get; set; } // O ID do vínculo é a chave principal aqui

        [Required(ErrorMessage = "Os perfis são obrigatórios.")]
        public List<string>? Perfis { get; set; }

        // UsuarioId e ClinicaId são obtidos no backend a partir do UpcId e do token
    }
}
