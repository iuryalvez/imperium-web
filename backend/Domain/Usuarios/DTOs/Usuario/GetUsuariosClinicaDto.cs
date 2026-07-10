namespace backend.Domain.Usuarios.DTOs
{
    public class GetUsuariosClinicaDto
    {
        public int Id { get; set; }
        public int ClinicaId { get; set; }
        public int UpcId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Nome { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public List<string>? Perfis { get; set; }
        public bool IsAtivoNaClinica { get; set; }
        public string? Cpf { get; set; }
        public string? Formacao { get; set; }
        public string? Telefone { get; set; }
        public string? Whatsapp { get; set; }
    }
}