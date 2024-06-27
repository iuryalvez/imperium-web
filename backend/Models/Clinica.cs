using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Clinica
    {
        public int Id { get; set; }
        public string Nome_clinica { get; set; } = string.Empty;
        public string Nome_fantasia { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public uint End_numero { get; set; }
        public string End_estado { get; set; } = string.Empty;
        public string End_cidade { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Telefone1 { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public virtual ICollection<Animal>? Animais { get; set; } = new List<Animal>();
    }
}