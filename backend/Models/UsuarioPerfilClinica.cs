using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models {
    public class UsuarioPerfilClinica {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public int PerfilId { get; set; }
        public Perfil? Perfil { get; set; }
        public int ClinicaId { get; set; }
        public Clinica? Clinica { get; set; }
        public bool Habilitado { get; set; }
    }
}
