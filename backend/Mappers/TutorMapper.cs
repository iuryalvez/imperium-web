using backend.Dtos.Tutor;
using backend.Models;

namespace backend.Mappers
{
    public static class TutorMapper
    {
        public static TutorDto ToTutorDto(this Tutor tutorModel)
        {
            return new TutorDto
            {
                Id = tutorModel.Id,
                Nome = tutorModel.Nome,
                Cpf = tutorModel.Cpf,
                Endereco = tutorModel.Endereco,
                Cep = tutorModel.Cep,
                Whatsapp = tutorModel.Whatsapp,
                Email = tutorModel.Email,
                DataCad = tutorModel.DataCad
            };
        }
        public static Tutor ToTutorFromCreateDTO (this CreateTutorRequestDto tutorDto) 
        {
            return new Tutor
            {
                Nome = tutorDto.Nome,
                Cpf = tutorDto.Cpf,
                Endereco = tutorDto.Endereco,
                Cep = tutorDto.Cep,
                Whatsapp = tutorDto.Whatsapp,
                Email = tutorDto.Email
            };
        }
    }
}