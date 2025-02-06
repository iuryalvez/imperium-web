namespace backend.Dtos.Usuario
{
    public class GetUsuariosDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public string Perfil { get; set; } = string.Empty;
    }
}