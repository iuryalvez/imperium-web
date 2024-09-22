namespace backend.Models {
    public class Senha {
        public int Id { get; set; }
        public string SenhaHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
