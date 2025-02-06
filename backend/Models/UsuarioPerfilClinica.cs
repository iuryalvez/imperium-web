namespace backend.Models {
    public class UsuarioPerfilClinica {
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
