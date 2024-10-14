using backend.Interfaces;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Dtos.Animal;

namespace backend.Repository
{
    public class AnimalRepository : IAnimalRepository
    {
        private readonly AppDbContext _context;
        public AnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Animal> CreateAsync(Animal animalModel)
        {
            await _context.Animais.AddAsync(animalModel);
            await _context.SaveChangesAsync();
            return animalModel;
        }

        public async Task<Animal?> DeleteAsync(int id)
        {
            var animalModel = await _context.Animais.FirstOrDefaultAsync(x => x.Id == id);
            if (animalModel == null)
            {
                return null;
            }
            _context.Animais.Remove(animalModel);
            await _context.SaveChangesAsync();
            return animalModel;
        }

        public async Task<List<Animal>> GetAllAsync()
        {
            return await _context.Animais.ToListAsync();
        }

        public async Task<Animal?> GetByIdAsync(int id)
        {
            return await _context.Animais.FindAsync(id);
        }

        public async Task<Animal?> UpdateAsync(int id, UpdateAnimalRequestDto animalDto)
        {
            var existingAnimal = await _context.Animais.FirstOrDefaultAsync(x => x.Id == id);
            if (existingAnimal == null) 
            {
                return null;
            }
            existingAnimal.Nome = animalDto.Nome;
            existingAnimal.Castrado = animalDto.Castrado;
            existingAnimal.Especie = animalDto.Especie;
            existingAnimal.Nascimento = animalDto.Nascimento;
            existingAnimal.DataCadModificado = DateTime.UtcNow;
            existingAnimal.Sexo = animalDto.Sexo;
            existingAnimal.Peso = animalDto.Peso;
            existingAnimal.CondicaoCorporal = animalDto.CondicaoCorporal;
            existingAnimal.GrauAtividade = animalDto.GrauAtividade;

            await _context.SaveChangesAsync();
            
            return existingAnimal;
        }
    }
}