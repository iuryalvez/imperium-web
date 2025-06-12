using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoDeAcesso.Entities;

public class UsuarioPerfilClinica {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "O id do usuário é obrigatório.")]
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    [Required(ErrorMessage = "O id da clínica é obrigatório.")]
    public int ClinicaId { get; set; }
    public Clinica? Clinica { get; set; }
    public List<string>? Perfis { get; set; }
}
