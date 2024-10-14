using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Dtos.TutorAnimalClinica
{
    public class CreateTutorAnimalClinicaRequestDto
    {
        public int Id { get; set; }
        public int TutorId { get; set; }
        public int AnimalId { get; set; }
        public int ClinicaId { get; set; }
    }
}