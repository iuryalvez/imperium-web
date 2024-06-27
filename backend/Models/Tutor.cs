using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Models
{
    public class Tutor
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public uint End_numero { get; set; } 
        public string End_estado { get; set; } = string.Empty;
        public string End_cidade { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Telefone1 { get; set; } = string.Empty; 
        public string Telefone2 { get; set; } = string.Empty;
        public string Whatsapp { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime Data_cad { get; set; } = DateTime.Now;
        public virtual ICollection<Animal> Animais { get; set; } = new List<Animal>();
        public virtual ICollection<Consulta> Consultas { get; set; } = new List<Consulta>();

    }
}