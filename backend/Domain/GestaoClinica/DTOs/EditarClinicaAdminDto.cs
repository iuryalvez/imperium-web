using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Domain.GestaoClinica.DTOs {
    public class EditarClinicaAdminDto {
        public int Id { get; set; } // id da clínica

        [Required(ErrorMessage = "O nome da clínica é obrigatório.")]
        [StringLength(100, ErrorMessage = "A clínica deve ter um nome entre 0 e 100 caracteres.")]
        public string? NomeClinica { get; set; }

        [StringLength(100, ErrorMessage = "O nome fantasia da clínica deve ter no máximo 100 caracteres.")]
        public string? NomeFantasia { get; set; }
        
        [StringLength(14, ErrorMessage = "O CNPJ deve ter no máximo 14 dígitos.")]
        public string? Cnpj { get; set; }

        [StringLength(300, ErrorMessage = "O endereço deve ter no máximo 300 caracteres.")]
        public string? Endereco { get; set; }
        public int? EndNumero { get; set; }
        public string? EndEstado { get; set; }
        public string? EndCidade { get; set; }

        [StringLength(9, ErrorMessage = "O CEP deve ter no máximo 9 dígitos.")]
        public string? Cep { get; set; }
        
        [StringLength(11, ErrorMessage = "O telefone deve ter no máximo 11 dígitos.")]
        public string? Telefone1 { get; set; }

        [StringLength(11, ErrorMessage = "O telefone deve ter no máximo 11 dígitos.")]
        public string? Whatsapp { get; set; }
        
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string? Email { get; set; }

        [StringLength(30, ErrorMessage = "O instagram da clínica deve ter no máximo 30 caracteres.")]
        public string? Instagram { get; set; }
    
        [Required(ErrorMessage = "O status da clínica é obrigatório.")]
        public bool IsAtiva { get; set; }
    }
}
