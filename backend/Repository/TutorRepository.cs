using backend.Interfaces;
using backend.Models;
using backend.Data;
using Microsoft.EntityFrameworkCore;
using backend.Dtos.Tutor;

namespace backend.Repository
{
    public class TutorRepository : ITutorRepository
    {
        private readonly AppDbContext _context;
        public TutorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tutor> CreateAsync(Tutor tutorModel)
        {
            await _context.Tutores.AddAsync(tutorModel);
            await _context.SaveChangesAsync();
            return tutorModel;
        }

        public async Task<Tutor?> DeleteAsync(int id)
        {
            var tutorModel = await _context.Tutores.FirstOrDefaultAsync(x => x.Id == id);
            if (tutorModel == null)
            {
                return null;
            }
            _context.Tutores.Remove(tutorModel);
            await _context.SaveChangesAsync();
            return tutorModel;
        }

        public async Task<List<Tutor>> GetAllAsync()
        {
            return await _context.Tutores.ToListAsync();
        }

        public async Task<Tutor?> GetByIdAsync(int id)
        {
            return await _context.Tutores.FindAsync(id);
        }

        public async Task<Tutor?> UpdateAsync(int id, UpdateTutorRequestDto tutorDto)
        {
            var existingTutor = await _context.Tutores.FirstOrDefaultAsync(x => x.Id == id);
            if (existingTutor == null) 
            {
                return null;
            }
            existingTutor.Nome = tutorDto.Nome;
            existingTutor.Cpf = tutorDto.Cpf;
            existingTutor.Endereco = tutorDto.Endereco;
            existingTutor.Cep = tutorDto.Cep;
            existingTutor.Whatsapp = tutorDto.Whatsapp;
            existingTutor.Email = tutorDto.Email;

            await _context.SaveChangesAsync();
            return existingTutor;
        }
    }
}