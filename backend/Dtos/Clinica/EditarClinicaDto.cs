using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Clinica {
    public class EditarClinicaDto {
        [Required]
        [StringLength(100, ErrorMessage = "A clínica deve ter um nome entre 0 e 100 caracteres.")]
        public string NomeClinica { get; set; } = string.Empty;

        // [Required]
        // [StringLength(100, ErrorMessage = "A clínica deve ter um nome fantasia entre 0 e 100 caracteres.")]
        public string NomeFantasia { get; set; } = string.Empty;

        // [Required]
        // [StringLength(14, ErrorMessage = "O CNPJ deve ter 14 dígitos.")]
        public string Cnpj { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public int EndNumero { get; set; }
        public string EndEstado { get; set; } = string.Empty;
        public string EndCidade { get; set; } = string.Empty;

        // [Required]
        // [StringLength(8, ErrorMessage = "O CEP deve ter 8 dígitos.")]
        public string Cep { get; set; } = string.Empty;

        // [Required]
        // [StringLength(11, ErrorMessage = "O telefone deve ter 11 dígitos (DDD)+9XXXX-XXXX")]
        public string Telefone1 { get; set; } = string.Empty;

        // [Required]
        // [StringLength(11, ErrorMessage = "O telefone deve ter 11 dígitos (DDD)+9XXXX-XXXX")]
        public string Whatsapp { get; set; } = string.Empty;
        
        // [Required]
        // [EmailAdress(ErrorMessage = "O e-mail é obrigatório.")]
        public string Email { get; set; } = string.Empty;
    }
}
