using backend.Dtos.Animal;
using backend.Models;

namespace backend.Mappers
{
    public static class AnimalMapper
    {
        public static AnimalDto ToAnimalDto(this Animal animalModel)
        {
            return new AnimalDto
            {
                Id = animalModel.Id,
                Nome = animalModel.Nome,
                Especie = animalModel.Especie,
                Nascimento = animalModel.Nascimento,
                DataCad = animalModel.DataCad,
                Sexo = animalModel.Sexo,
                Castrado = animalModel.Castrado,
                Peso = animalModel.Peso,
                CondicaoCorporal = animalModel.CondicaoCorporal,
                GrauAtividade = animalModel.GrauAtividade
            };
        }
        public static Animal ToAnimalFromCreateDTO (this CreateAnimalRequestDto animalDto) 
        {
            return new Animal
            {
                Nome = animalDto.Nome,
                Especie = animalDto.Especie,
                DataCad = animalDto.DataCad,
                Sexo = animalDto.Sexo
            };
        }
    }
}