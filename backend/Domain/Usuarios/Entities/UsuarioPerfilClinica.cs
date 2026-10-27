using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.Usuarios.Entities;

namespace backend.Domain.Usuarios.Entities;

public class UsuarioPerfilClinica {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O id do usuário é obrigatório.")]
    public int UsuarioId { get; set; }

    [ForeignKey("UsuarioId")]
    public Usuario? Usuario { get; set; }

    [Required(ErrorMessage = "O id da clínica é obrigatório.")]
    public int ClinicaId { get; set; }

    [ForeignKey("ClinicaId")]
    public Clinica? Clinica { get; set; }
    public List<string>? Perfis { get; set; } // o usuário só possui o isAtivo = false se todos seus vínculos estiverem com um perfil INATIVO
}
