using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using System.Text.Json;
using backend.Models;

public class UsuarioPerfilClinica {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public int ClinicaId { get; set; }
    public Clinica? Clinica { get; set; }

    // Dicionário para armazenar os perfis do usuário na clínica
    // Dicionário de perfis com valores booleanos
    public Dictionary<string, bool>? Perfis { get; set; }
}
