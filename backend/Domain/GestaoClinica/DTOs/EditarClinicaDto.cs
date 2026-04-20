using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Domain.GestaoClinica.DTOs
{
    public class EditarClinicaDto
    {
        [Required(ErrorMessage = "O nome da clínica é obrigatório.")]
        [StringLength(100, ErrorMessage = "A clínica deve ter um nome entre 0 e 100 caracteres.")]
        public string NomeClinica { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "O nome fantasia da clínica deve ter no máximo 100 caracteres.")]
        public string? NomeFantasia { get; set; }

        [StringLength(14, ErrorMessage = "O CNPJ deve ter no máximo 14 dígitos.")]
        public string? Cnpj { get; set; }

        [StringLength(11, ErrorMessage = "O telefone deve ter no máximo 11 dígitos.")]
        public string? Telefone { get; set; }

        [StringLength(11, ErrorMessage = "O telefone deve ter no máximo 11 dígitos.")]
        public string? Whatsapp { get; set; }

        [StringLength(30, ErrorMessage = "O instagram da clínica deve ter no máximo 30 caracteres.")]
        public string? Instagram { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string? Email { get; set; }

        // Endereço
        [StringLength(8, ErrorMessage = "O CEP deve ter no máximo 8 dígitos.")]
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }

        [StringLength(2, ErrorMessage = "Unidade Federativa deve ter no máximo 2 letras.")]
        public string? Uf { get; set; }
    }
}
