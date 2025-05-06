using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Clinica
{
    public class GetClinicaDto
    {
        public int Id { get; set; }
        public string? NomeClinica { get; set; }
        public bool IsAtiva { get; set; }
        public DateTime DataCad { get; set; }
    }
}