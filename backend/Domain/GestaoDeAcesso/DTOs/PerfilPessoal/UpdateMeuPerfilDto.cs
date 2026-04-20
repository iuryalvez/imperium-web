namespace backend.Domain.GestaoDeAcesso.DTOs.PerfilPessoal
{
    public class UpdateMeuPerfilDto
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Formacao { get; set; }
        public string? Cpf { get; set; }
        public string? Telefone { get; set; }
        public string? Whatsapp { get; set; }
        
        // Propriedades para alteração de senha
        public string? SenhaAtual { get; set; }
        public string? NovaSenha { get; set; }
    }
}