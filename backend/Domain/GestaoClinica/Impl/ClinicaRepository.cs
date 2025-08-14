using backend.Domain.GestaoClinica.DTOs;
using backend.Domain.GestaoClinica.Entities;
using backend.Domain.GestaoClinica.Interface;
using backend.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace backend.Domain.GestaoClinica.Impl
{
    public class ClinicaRepository : IClinicaRepository
    {
        private readonly AppDbContext _context;
        public ClinicaRepository(AppDbContext context) { _context = context; }

        public async Task<Clinica?> GetByIdAsync(int clinicaId) => await _context.Clinicas.FindAsync(clinicaId);

        public async Task<IEnumerable<Clinica>> GetAllAsync() =>
            await _context.Clinicas.AsNoTracking().ToListAsync();

        public async Task<bool> CnpjExistsForAnotherClinicAsync(string cnpj, int excludeClinicaId)
        {
            return await _context.Clinicas.AnyAsync(c => c.Cnpj == cnpj && c.Id != excludeClinicaId);
        }

        public async Task<IEnumerable<object>> SearchByNameAsync(string nome)
        {
            return await _context.Clinicas
                .Where(c => EF.Functions.ILike(c.NomeClinica, $"%{nome}%"))
                .Select(c => new { value = c.Id, label = c.NomeClinica })
                .ToListAsync();
        }

        public async Task AddAsync(Clinica clinica) => await _context.Clinicas.AddAsync(clinica);

        public void Delete(Clinica clinica) => _context.Clinicas.Remove(clinica);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}