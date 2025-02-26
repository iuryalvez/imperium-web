namespace backend.Dtos.Usuario
{
    public class GetUsuariosDto
    {
        public int Id { get; set;}
        public int UpcId { get; set; }
        public string? Email { get; set;}
        public string? UserName { get; set; }
        public string? Nome { get; set; }
        public DateTime DataCadastro { get; set; }
        public DateTime DataUltimoAcesso { get; set; }
        public string? Perfil { get; set; }
        public string? NomeClinica { get; set; }
        public bool Habilitado { get; set; }    
    }
}