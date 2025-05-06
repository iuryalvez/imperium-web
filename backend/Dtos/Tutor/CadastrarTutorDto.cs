using System;
using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Tutor
{
    public class TutorDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, ErrorMessage = "O CPF deve ter 11 caracteres.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "O endereço é obrigatório.")]
        [StringLength(300, ErrorMessage = "O endereço deve ter no máximo 300 caracteres.")]
        public string Endereco { get; set; }

        [Required(ErrorMessage = "O CEP é obrigatório.")]
        [StringLength(8, ErrorMessage = "O CEP deve ter 8 caracteres.")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "O WhatsApp é obrigatório.")]
        // [Phone(ErrorMessage = "O número de WhatsApp não é válido.")]
        [StringLength(11, ErrorMessage = "O telefone deve ter 11 dígitos.")]
        public string Whatsapp { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail não é válido.")]
        public string Email { get; set; }

        public DateTime DataCad { get; set; }
    }
}
