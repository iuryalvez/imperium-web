namespace backend.Dtos.Clinica {
    public class EditarClinicaAdminDto {
        public int ClinicaId { get; set; }
        public string NomeClinica { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int EndNumero { get; set; }
        public string EndEstado { get; set; } = string.Empty;
        public string EndCidade { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Telefone1 { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
