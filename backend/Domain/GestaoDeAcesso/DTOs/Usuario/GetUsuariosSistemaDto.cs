namespace backend.Domain.GestaoDeAcesso.DTOs
{
    public class GetUsuariosSistemaDto 
    {
        public string CompositeKey { get; set; } // NOVO CAMPO PARA O ROWKEY
        public int Id { get; set; }
        public int UpcId { get; set; }
        public int ClinicaId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? NomeClinica { get; set; }
        public string? Nome { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime? DataUltimoAcesso { get; set; }
        public List<string> Perfis { get; set; } = new();
        public bool IsAdmin { get; set; }
        public bool IsAtivo { get; set; } // IsAtivo no sistema
    }
}