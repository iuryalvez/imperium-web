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

        public async Task<Clinica?> GetByIdWithDetailsAsync(int clinicaId) {
            return await _context.Clinicas
                .Include(c => c.Racoes)              // Carrega a coleção de rações
                .Include(c => c.Consultas)           // Carrega a coleção de consultas
                .Include(c => c.TutoresClinicas)     // Carrega a coleção de tutores
                .Include(c => c.UsuariosPerfisClinicas)
                .FirstOrDefaultAsync(c => c.Id == clinicaId);
        }

        public async Task<IEnumerable<Clinica>> GetAllAsync() =>
            await _context.Clinicas.AsNoTracking().ToListAsync();

        public async Task<bool> CnpjExistsForAnotherClinicAsync(string cnpj, int excludeClinicaId)
        {
            return await _context.Clinicas.AnyAsync(c => c.Cnpj == cnpj && c.Id != excludeClinicaId);
        }

        public async Task AddAsync(Clinica clinica) => await _context.Clinicas.AddAsync(clinica);

        public void Delete(Clinica clinica) => _context.Clinicas.Remove(clinica);

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}