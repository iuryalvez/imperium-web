using backend.Dtos.TutorAnimalClinica;
using backend.Models;

namespace backend.Mappers
{
    public static class TutorAnimalClinicaMapper
    {
        public static TutorAnimalClinicaDto ToTutorAnimalClinicaDto(this TutorAnimalClinica tutorAnimalClinicaModel)
        {
            return new TutorAnimalClinicaDto
            {
                Id = tutorAnimalClinicaModel.Id,
                TutorId = tutorAnimalClinicaModel.TutorId,
                AnimalId = tutorAnimalClinicaModel.AnimalId,
                ClinicaId = tutorAnimalClinicaModel.ClinicaId
            };
        }

        public static TutorAnimalClinica ToTutorAnimalClinicaFromDto(this TutorAnimalClinicaDto tutorAnimalClinicaDto)
        {
            return new TutorAnimalClinica
            {
                Id = tutorAnimalClinicaDto.Id,
                TutorId = tutorAnimalClinicaDto.TutorId,
                AnimalId = tutorAnimalClinicaDto.AnimalId,
                ClinicaId = tutorAnimalClinicaDto.ClinicaId
            };
        }
    }
}
