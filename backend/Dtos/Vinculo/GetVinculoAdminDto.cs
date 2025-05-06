using System.ComponentModel.DataAnnotations;

namespace backend.Dtos.Vinculo
{
    public class GetVinculoAdminDto
    {
        public int ClinicaId { get; set; }

        public List<string>? Perfis { get; set; } // Definir no controlador.
    }
}