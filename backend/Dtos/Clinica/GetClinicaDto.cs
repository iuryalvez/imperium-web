using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Clinica
{
    public class GetClinicaDto
    {
        public int Id { get; set; }
        public string NomeClinica { get; set; } = string.Empty;
        public DateTime DataCad { get; set; }
    }
}