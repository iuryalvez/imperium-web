using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Clinica {
    public class EditarClinicaAdminDto {
        public int ClinicaId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "A clínica deve ter um nome entre 0 e 100 caracteres.")]
        public string NomeClinica { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "A clínica deve ter um nome fantasia entre 0 e 100 caracteres.")]
        public string NomeFantasia { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int EndNumero { get; set; }
        public string EndEstado { get; set; } = string.Empty;
        public string EndCidade { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Telefone1 { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;
        
        // [Required(ErrorMessage = "O e-mail é obrigatório.")]
        // [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string Email { get; set; } = string.Empty;
        public bool IsAtiva { get; set; }
    }
}
