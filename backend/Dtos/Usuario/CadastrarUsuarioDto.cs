namespace backend.Dtos.Usuario
{
    public class CadastrarUsuarioDto
    {
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Nome { get; set; }
        public string? Senha { get; set; }
        public DateTime DataCad { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public bool IsAdmin { get; set; }
        public int ClinicaId { get; set; }
        public List<string>? Perfis { get; set; }
    }
}